using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.AI;
// --------------------------------------------------------------------------------------------------------------------------------------------
// CLASS:       EnemySpawner
// Description: Spawns enemies into the game based on the time that has passed. The more time that hass passed the more frequent the spawns
//              Previously spawn points were simply an array of transforms littered around the map, now this script generates random points
//              on the navmesh to spawn enemies on, and preferences random points to be inside of 'spawn areas'. The spawn areas are 
//              trigger colliders dotted around the environemnt that segment different areas. This prevents enemies spawning in a room that is 
//              seperate to the room the player is in, but may still be in the player's spawn range.
// --------------------------------------------------------------------------------------------------------------------------------------------
public class EnemySpawner : MonoBehaviour
{

    // Public Fields
    [Header("Enemies:")]
    public GameObject MeleeEnemy;
    public GameObject ArmouredEnemy;
    public GameObject FlamerEnemy; // References to inactive enemies we willbe spawning

    [Header("Spawn Portal Effects:")]
    public GameObject MeleeSpawnParticles;
    public GameObject ArmouredSpawnParticles;
    public GameObject FlamerSpawnParticles; // Parent of spawn-in Particles

    [Header("Score Text:")]
    public TextMeshProUGUI ActiveEnemies;
    [HideInInspector] public Collider UpperFloorSpawnArea; // This spawn area is handled individually
    public MusicManager MusicMan;
    // Private Fields
    private float _endTime = 1200.0f; // twenty minutes, after which point we begin spawning kill 
    private int _maxEnemies = 30; // After testing a build on my machine, performance issues began to creep in after about 34 enemies
    private MoveScript _player; // get a reference to player, only spawn if player is aliive, otherwise enemies' awake method will throw an error
    private Transform _meleeParticlesPool, _armouredParticlesPool, _flamerParticlesPool; // pools for the spawn-in particle effects
    private LevelManager _gameManager;
    private float minMeleeDistance = 4f, maxMeleeDistance = 9.25f,
                  minArmouredDistance = 6f, maxArmouredDistance = 21f,
                  minFlamerDistance = 10f, maxFlamerDistance = 25f; // min and max spawn distances for each enemy

    private int _maxArmouredEnemies = 3, _maxFlamerEnemies = 2; // Hard limit on the armoured and flamer enemies for the first 20 minutes of gameplay, so it's not too difficlt

    //------------------------------------------------------------------------------------------
    // Method: Start()
    // Desc:   Sets up connections to other components, pools the spawn particle effects, starts
    //         the coroutine to spawn enemies throughout the game, and invokes the "SpawnKillWaves"
    //         Method after 20 minutes of gameplay
    //-------------------------------------------------------------------------------------------
	void Start ()
    {
        _player = MeleeEnemy.GetComponent<StateController>().Player; // Access the player through the enemy state controller instead of using FindObjectOfType()
        _gameManager = _player.GetComponent<PlayerLifecycle>().LevelManager;

        InvokeRepeating("SpawnKillWaves", _endTime, 6.0f); // After twenty minutes spawn in armoured enemies every 5 seconds, the game has to end at some point
        Invoke("SpawnArmouredEnemy", 30.24f); // spawn an armoured enemy after about 30 seconds every time

        // Create pools for spawn in particle effects
        // I do not pool the enemies because I ran into errors re-pooling them after ragdolling the meshes
        _meleeParticlesPool = ObjectPooler.CreatePool(MeleeSpawnParticles, 5, "Spawn Particles", false);
        _armouredParticlesPool = ObjectPooler.CreatePool(ArmouredSpawnParticles, 3, "Armoured Spawn particles", false);
        _flamerParticlesPool = ObjectPooler.CreatePool(FlamerSpawnParticles, 3, "Flamer Spawn Particles", false);

        StartCoroutine(SpawnEnemies()); // Start the coroutine that spawns in enemies in an increasing frequency

        InvokeRepeating("SpawnFlamerEnemy", 300.0f, 300.0f); // Every five minutes spawn a flamer enemy
    }

    //-------------------------------------------------------------------------------------------
    // Method: RandomSpawnPoint (MaxSpawnDistance)
    // Desc:   Uses Navmesh.SamplePosition and Random.InsideUnitSphere to generate a random
    //         Vector3 point on the navmesh, can be used for spawning enemies
    //-------------------------------------------------------------------------------------------
    private Vector3 RandomSpawnPoint(float spawnDistance)
    {
        Vector3 random = Random.insideUnitSphere * spawnDistance;
        random += _player.transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(random, out hit, 200.0f, 1); // I pass the max distance of 200 because the length and breadth of height of the game world never exceeds about 160 units
        return hit.position;
    }

    //-------------------------------------------------------------------------------------------------------------------------
    // Method:  GetRandomSpawnPoint (MaxSpawnDistance)
    // Desc:    Uses navmesh.SamplePosition with Random.InsideUnitSphere multiplied by the spawnDostance param to get a point on navmesh
    //          Accesses currently active spawn areas through the DetectSpawnAreas.cs file. This is done to preferentially spawn 
    //          enemies in the same area as the player -> For example we do not want to spawn an enemy in the lower game area when the
    //          player is on an upper floor, as the enemies' navMeshAgent would have a longer path to calculate upon spawning
    //---------------------------------------------------------------------------------------------------------------------------
    private Vector3 GetRandomSpawnPoint(float spawnDistance)
    {
        DetectSpawnAreas spawnAreas = _player.GetComponent<DetectSpawnAreas>();
        // Try ten times to get a vector position inside the spawn collider
        // Do this according to advice in unity docs: https://docs.unity3d.com/540/Documentation/ScriptReference/NavMesh.SamplePosition.html
        // As I am including a SamplePosition radius of spawnDistance and not 1, limit the loop to ten iterations instead of 20
        for (int i = 0; i < 10; i++)
        {
            Vector3 random = Random.insideUnitSphere * spawnDistance;
            random += _player.transform.position; // We basically create a sphere around the player with radius of spawnDistance
            NavMeshHit hit;
            NavMesh.SamplePosition(random, out hit, spawnDistance, 1);                                                    // Loopthrough active spawn areas:

            int randomInt = Random.Range(0, 2); // Generate random int equal to 0 or 1, Random.Range(int,int) is max exclusive

            if (randomInt == 0) // Loop upwards:
            {
                for (int j = 0; j < spawnAreas.ActiveSpawnAreas.Count; j++)
                {
                    // return the hit.position if it is inside an active spawn area:
                    if (spawnAreas.ActiveSpawnAreas[j].bounds.Contains(hit.position))
                    {
                        // Make one more check for the edge case that the spawn position is the player position
                        // Becuse if it is, we will get a NaN error when assigning the Quaternion rotation
                        if (hit.position != _player.transform.position)
                        {
                            return hit.position;
                        }
                    }
                }
            }
            else
            {
                // Sometimes it will be better to loop down through the spawn areas, to make the spawning less predictable:
                for (int k = spawnAreas.ActiveSpawnAreas.Count; k < 0; k--)
                {
                    // return the hit.position if it is inside an active spawn area:
                    if (spawnAreas.ActiveSpawnAreas[k].bounds.Contains(hit.position))
                    {
                        if (hit.position != _player.transform.position)
                        {
                            return hit.position;
                        }
                    }
                }
            }
        }

        // If we failed to get a random spawn point inside an active spawn area, defer to a random spawn point:
        Vector3 randomVec3 = Random.insideUnitSphere * spawnDistance;
        randomVec3 += _player.transform.position; // We basically create a sphere around the player with radius of spawnDistance
        NavMeshHit hit02;
        NavMesh.SamplePosition(randomVec3, out hit02, spawnDistance, 1);
        return hit02.position;
    }

    //-------------------------------------------------------------------------------------------
    // Method: SpawnEnemies, IEnumerator
    // Desc:   Spawns in enemies at random points, staggers the spawning as well so they are not all
    //         calling NavMeshAgent.SetDestination at the same interval. Spawns enemies with increasing frequency as time goes on
    //-------------------------------------------------------------------------------------------
    private IEnumerator SpawnEnemies()
    {
        while (_gameManager.secondsElapsed < 120f) // The first two minutes of gameplay
        {
            float meleeSpawnInterval = Random.Range(6.5f, 10.0f); // Get a random spawn interval
            yield return new WaitForSeconds(meleeSpawnInterval); // wait until that enemy spawns to spawn the next
            Invoke("SpawnMeleeEnemy", 0.0f);
        }
        while (_gameManager.secondsElapsed > 120f
                && _gameManager.secondsElapsed < 300f)                   // First five minutes of gameplay
        {
            _maxArmouredEnemies = 2;                                     // Only two projectile enemies in first five minutes
            float meleeSpawnInterval = Random.Range(6.0f, 8.5f);         // Random spawn interval for melee enemy
            float armouredSpawnInterval = Random.Range(30.0f, 60.0f);    // Random spawn interval for armoured enemy
            Invoke("SpawnMeleeEnemy", meleeSpawnInterval);               // Invoke after min 6 seconds
            Invoke("SpawnArmouredEnemy", armouredSpawnInterval);         // Invoke after min 30 seconds
            yield return new WaitForSeconds(meleeSpawnInterval);         // wait at least six seconds before calling the next invoke
        }
        while (_gameManager.secondsElapsed > 300f
                && _gameManager.secondsElapsed < 600f) // Second five minutes of gameplay
        {
            _maxArmouredEnemies = 3;
            float meleeSpawnInterval = Random.Range(5.5f, 8.0f);
            float armouredSpawnInterval = Random.Range(22.0f, 48.0f);
            Invoke("SpawnMeleeEnemy", meleeSpawnInterval);
            Invoke("SpawnArmouredEnemy", armouredSpawnInterval);
            yield return new WaitForSeconds(meleeSpawnInterval);
        }
        while (_gameManager.secondsElapsed > 600f
                && _gameManager.secondsElapsed < 900f) // Third five minute block of gameplay:
        {
            _maxArmouredEnemies = 4;
            float meleeSpawnInterval = Random.Range(4.0f, 6.5f);
            float armouredSpawnInterval = Random.Range(15.0f, 30.0f);
            Invoke("SpawnMeleeEnemy", meleeSpawnInterval);
            Invoke("SpawnArmouredEnemy", armouredSpawnInterval);
            yield return new WaitForSeconds(meleeSpawnInterval);
        }
        while (_gameManager.secondsElapsed > 900
        && _gameManager.secondsElapsed < 1200f) // Fourth five minute block of gameplay:
        {
            float meleeSpawnInterval = Random.Range(3.5f, 5.5f);
            float armouredSpawnInterval = Random.Range(10.0f, 18.0f);
            Invoke("SpawnMeleeEnemy", meleeSpawnInterval);
            Invoke("SpawnArmouredEnemy", armouredSpawnInterval);
            yield return new WaitForSeconds(meleeSpawnInterval);
        }
            yield return null;
    }
    
    //-------------------------------------------------------------------------------------------
    // Method: SpawnMeleeEnemy()
    // Desc:   Spawns a melee enemy at a random spawn point, facing toward the player's position
    //-------------------------------------------------------------------------------------------
    private void SpawnMeleeEnemy()
    {
        if (PlayerStats.ActiveEnemies >= _maxEnemies || !_player || PlayerStats.PlayerHealth <= 0.0f
            || !Camera.main) // The main camera is disabled after the player has died
        {
            return;
        }
        
        // Generate a random spawn point (preferably inside an active spawn area)
        // Geta quaternion that is facing toward the player
        Vector3 randomSpawnPos = GetRandomSpawnPoint(Random.Range(minMeleeDistance, maxMeleeDistance));
        Quaternion directionToFace = Quaternion.LookRotation(_player.transform.position - randomSpawnPos, MeleeEnemy.transform.up);
        // Instantiate an enemy at that position and with that rotation
        // I did not figure out how to properly re-pool and re-use enemy objects, because of the ragdolling effect
        GameObject enemy = Instantiate(MeleeEnemy, randomSpawnPos, directionToFace);
        // Activate the spawn-in particle effect wherever the enemy is instantiated
        ObjectPooler.UseObject(_meleeParticlesPool, enemy.transform.position);
        enemy.SetActive(true);
        PlayerStats.ActiveEnemies++; // update the active enemies (this is decremented in the EnemyLifeCycle.cs script

        MusicMan.CheckForCrossFade(); // check if it is appropriate to crossfade the audio tracks

        return;    
    }

    //-------------------------------------------------------------------------------------------
    // Method: SpawnArmouredEnemy()
    // Desc:   Spawns an armoured enemy at a random spawn point, facing toward the player's position
    //-------------------------------------------------------------------------------------------
    private void SpawnArmouredEnemy()
    {
        if (PlayerStats.ActiveEnemies >= _maxEnemies || !_player || PlayerStats.PlayerHealth <= 0.0f
            || !Camera.main || PlayerStats.ActiveArmouredEnemies >= _maxArmouredEnemies)
        {
            return;
        }

        Vector3 randomSpawnPos = GetRandomSpawnPoint(Random.Range(minArmouredDistance, maxArmouredDistance));
        Quaternion directionToFace = Quaternion.LookRotation(_player.transform.position - randomSpawnPos, ArmouredEnemy.transform.up);

        GameObject enemy = Instantiate(ArmouredEnemy, randomSpawnPos, directionToFace);
        ObjectPooler.UseObject(_armouredParticlesPool, enemy.transform.position);
        enemy.SetActive(true);
        PlayerStats.ActiveEnemies++;
        PlayerStats.ActiveArmouredEnemies++;
        return;
    }
    //-------------------------------------------------------------------------------------------
    // Method: SpawnFlamerEnemy()
    // Desc:   Spawns a Flamer enemy at a random spawn point, facing toward the player's position
    //-------------------------------------------------------------------------------------------
    private void SpawnFlamerEnemy()
    {
        if (!_player || PlayerStats.PlayerHealth <= 0.0f
            || !Camera.main || PlayerStats.ActiveFlamerEnemies >= _maxFlamerEnemies)
        {
            return;
        }
        if (PlayerStats.ActiveEnemies >= _maxEnemies)
        {
            Invoke("SpawnFlamerEnemy", 0.1f); // If we have max enemies, keep calling this method until we can spawn a flamer enemy
            return;
        }

        Vector3 randomSpawnPos = GetRandomSpawnPoint(Random.Range(minFlamerDistance, maxFlamerDistance));
        Quaternion directionToFace = Quaternion.LookRotation(_player.transform.position - randomSpawnPos, FlamerEnemy.transform.up);

        GameObject enemy = Instantiate(FlamerEnemy, randomSpawnPos, directionToFace);
        ObjectPooler.UseObject(_flamerParticlesPool, enemy.transform.position);
        enemy.SetActive(true);
        PlayerStats.ActiveEnemies++;
        PlayerStats.ActiveFlamerEnemies++;
        return;
    }


    /// <summary>
    /// Spawns enemyType randomly on Navmesh, between minSpawnDistacen and maxSpawndistance from player position
    /// </summary>
    /// <param name="enemyType"></param>
    /// <param name="minSpawnDistance"></param>
    /// <param name="maxSpawnDistance"></param>
    private void SpawnEnemy(GameObject enemyType, float minSpawnDistance, float maxSpawnDistance)
    {
        if (PlayerStats.ActiveEnemies >= _maxEnemies || !_player || PlayerStats.PlayerHealth <= 0.0f)
        {
            return;
        }

        Vector3 randomSpawnPos = RandomSpawnPoint(Random.Range(minSpawnDistance, maxSpawnDistance));
        Quaternion directionToFace = Quaternion.LookRotation(_player.transform.position - randomSpawnPos, enemyType.transform.up);

        GameObject enemy = Instantiate(enemyType, randomSpawnPos, directionToFace);
        ObjectPooler.UseObject(_meleeParticlesPool, enemy.transform.position);
        enemy.SetActive(true);
        PlayerStats.ActiveEnemies++;
        return;
    }

    //------------------------------------------------------------------------------------------------------
    // Method: SpawnKillWaves()
    // Desc:   Cancels all other invokes and coroutines. Randomly spawns an enemy every 6 seconds
    //         Invoked in the Start Method after 20 minutes of gampeplay, gameplay has to end at some point
    //         And only the strongest will survive after 20 minutes
    //------------------------------------------------------------------------------------------------------
    private void SpawnKillWaves()
    {
        CancelInvoke("SpawnMeleeEnemyStaggered");
        CancelInvoke("SpawnArmouredEnemy");
        CancelInvoke("SpawnFlamerEnemy");
        StopAllCoroutines();

        int randomInt = Random.Range(0, 3);
        if (randomInt==0)
        {
            SpawnEnemy(MeleeEnemy,minMeleeDistance,maxMeleeDistance);
        }
        else if (randomInt == 1)
        {
            SpawnEnemy(ArmouredEnemy, minArmouredDistance, maxArmouredDistance);
        }
        else if (randomInt == 2)
        {
            SpawnEnemy(FlamerEnemy, minFlamerDistance, maxFlamerDistance);
        }
        return;
    }
}
