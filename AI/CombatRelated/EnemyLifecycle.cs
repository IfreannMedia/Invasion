using UnityEngine;
using TMPro;
//---------------------------------------------------------------------------------------------------------
// CLASS:       EnemyLifeCycle, implements IDamageable and IKillable
// DESCRIPTION: This class handles damaging and killing enemies. It's methods are called by the player's
//              Gun.cs abstract base class, and by the PlayerMeleeAttack.cs script when he punches.
//              This script also calls methods in the RagdollManager.cs script, which ragdolls enemies.
//              Code that updats the player's score through code and in the HUD is also handled here.
//---------------------------------------------------------------------------------------------------------

public class EnemyLifecycle : MonoBehaviour, IDamageable, IKillable {

    public float EnemyHealth;
    public enum ScoreAmount {punchScore, bodyShotScore, HeadShotScore, KillMeleeEnemy, KillArmouredEnemy, KillFlamerEnemy }
    private Animator _animator;
    // This script needs access to animator params before the Start event method is called
    // And accessing them through the TagsHashIDs.cs script in the first OnEnable method call will
    // Always result an error, either a nullreference or hash int returning 0 (because the HashID script has not yet
    // completed caching the hash ID values)
    private int  _hitTrigger, _aliveBool;
    private AudioSource _beastScream;
    public AudioSource SpawnNoise;
    private int _punchScore = 5, _bodyShotScore = 10, 
                _headShotScore = 20, _killScore = 50,
                _armouredEnemyScore = 100, _flamerEnemyScore = 150; // score amounts for hitting enemies
    private Pickup[] _healthPickup; // Empty array container for possible goodies enemies can spawn when killed
    public TextMeshProUGUI ScoreText;
    private ParticleSystem[] _scoreParticles = new ParticleSystem[6]; // The six score particle systems

    //-----------------------------------------------------------------------------------
    // Method: Awake
    // Desc:   Cache components, and convert animator strings to hashIDs
    //-----------------------------------------------------------------------------------
    void Awake()
    {
        _animator       = GetComponent<Animator>();
        _hitTrigger     = Animator.StringToHash("Hit");
        _aliveBool      = Animator.StringToHash("Alive");
        _beastScream    = GetComponent<AudioSource>();
        _healthPickup   = GetComponentsInChildren<Pickup>(true); // Get health pickups contained in enemies heirarchy
        _scoreParticles = ScoreText.GetComponentsInChildren<ParticleSystem>();
    }


    //-------------------------------------------------------------
    // Method: OnEnable
    // Desc:   Set animator parameter and play the spawn in noise
    //-------------------------------------------------------------
    private void OnEnable()
    {
        _animator.SetBool(_aliveBool, true);
        SpawnNoise.PlayOneShot(SpawnNoise.clip);
    }

    //---------------------------------------------------------------------------------------------------------------------
    // Method: TakeDamage
    // Desc:   Take damage from the enemies life, called from player gun scripts and melee attack scripts
    //         can also be called from the EnemyGun.cs script.This method is basic version and not currently implemented
    //---------------------------------------------------------------------------------------------------------------------
    public void TakeDamage(float damage)
    {
        EnemyHealth -= damage;
        _animator.SetTrigger(_hitTrigger);
        if (EnemyHealth <= 0)
        {
            KillEntity();
        } 
    }

    //--------------------------------------------------------------------------------------------------------
    // Method: TakeDamage, overload I
    // Desc:   takes damage, also can apply rigidbody force to the enemy, which is used for ragdolling effect
    //--------------------------------------------------------------------------------------------------------
    public void TakeDamage(float damage, Vector3 direction, float force)
    {
        EnemyHealth -= damage;
        _animator.SetTrigger(_hitTrigger);
        if (EnemyHealth <= 0)
        {
            KillEntity(direction, force); // we pass in the direction and force params when killing an entity
        }
    }

    //--------------------------------------------------------------------------------------------------------
    // Method: TakeDamage, overload II
    // Desc:   takes damage, also can apply rigidbody force to a specific collider, which is used for 
    //         a more precise ragdolling effect
    //--------------------------------------------------------------------------------------------------------
    public void TakeDamage(float damage, Vector3 direction, float force, Collider hitCol)
    {
        EnemyHealth -= damage;

        if (transform.CompareTag(TagsHashIDs.MeleeEnemy) || transform.CompareTag(TagsHashIDs.EnemyHead)
            || transform.CompareTag(TagsHashIDs.EnergyPack))
        _animator.SetTrigger(_hitTrigger); // we do not want a hit animation state to begin if we hit a flamer enemy on his armour, as it does not really damage him

        if (EnemyHealth <= 0)
        {
            KillEntity(direction, force, hitCol);
        }
    }

    //--------------------------------------------------------------------------------------------------------
    // Method: TakeDamage, overload III
    // Desc:   takes damage, also can apply rigidbody force to a specific collider, which is used for 
    //         a more precise ragdolling effect. This method enables scoring to occur
    //--------------------------------------------------------------------------------------------------------
    public void TakeDamage(float damage, Vector3 direction, float force, Collider hitCol, ScoreAmount scoreAmount)
    {
        EnemyHealth -= damage;
        if (transform.CompareTag(TagsHashIDs.MeleeEnemy) || transform.CompareTag(TagsHashIDs.EnemyHead)
            || transform.CompareTag(TagsHashIDs.EnergyPack))
            _animator.SetTrigger(_hitTrigger);

        // Added a switch statement to update the player score and emit score particles:
        // Score particles referring to how the score increment pops up and out of the bottom right of the in game HUD
        switch (scoreAmount)
        {
            case ScoreAmount.punchScore:
                PlayerStats.Score += _punchScore;
                ScoreText.SetText("Score: " + PlayerStats.Score);
                _scoreParticles[(int)scoreAmount].Emit(1);
                break;
            case ScoreAmount.bodyShotScore:
                PlayerStats.Score += _bodyShotScore;
                ScoreText.SetText("Score: " + PlayerStats.Score);
                _scoreParticles[(int)scoreAmount].Emit(1);
                break;

            case ScoreAmount.HeadShotScore:
                PlayerStats.Score += _headShotScore;
                ScoreText.SetText("Score: " + PlayerStats.Score);
                _scoreParticles[(int)scoreAmount].Emit(1);
                break;
            case ScoreAmount.KillMeleeEnemy:
                PlayerStats.Score += _killScore;
                ScoreText.SetText("Score: " + PlayerStats.Score);
                _scoreParticles[(int)scoreAmount].Emit(1);
                break;
            case ScoreAmount.KillArmouredEnemy:
                PlayerStats.Score += _armouredEnemyScore;
                ScoreText.SetText("Score: " + PlayerStats.Score);
                _scoreParticles[(int)scoreAmount].Emit(1);
                break;
            case ScoreAmount.KillFlamerEnemy:
                PlayerStats.Score += _flamerEnemyScore;
                ScoreText.SetText("Score: " + PlayerStats.Score);
                _scoreParticles[(int)scoreAmount].Emit(1);
                break;
            default:
                break;
        }

        // Use a conditional and switch statement to determine the kill score of this enemy, by comparing the hitcol's tag:
        if (EnemyHealth <= 0)
        {
            switch (hitCol.tag)
            {
                case TagsHashIDs.MeleeEnemy:
                    KillEntity(direction, force, hitCol, ScoreAmount.KillMeleeEnemy);
                    break;
                case TagsHashIDs.ArmouredEnemy:
                    KillEntity(direction, force, hitCol, ScoreAmount.KillArmouredEnemy);
                    break;
                case TagsHashIDs.FlamerEnemy:
                    KillEntity(direction, force, hitCol, ScoreAmount.KillFlamerEnemy);
                    break;
                case TagsHashIDs.EnemyHead:
                    KillEntity(direction, force, hitCol, ScoreAmount.KillMeleeEnemy); // The melee enemy is the only one we can headshot
                    break;
                case TagsHashIDs.EnergyPack:
                    KillEntity(direction, force, hitCol, ScoreAmount.KillFlamerEnemy); // The flamer enemy is the only one with an energy pack
                    break;
                default:
                    KillEntity(direction, force, hitCol, scoreAmount); // Default to this if the other tag comparison's fail
                    break;
            }
           
        }
    }

    //---------------------------------------------------------------------------------------------------------------------
    // Method: KillEntity
    // Desc:   basic kill entity method, does not apply rigidbody force or torque, simply kills the enemy and updates score in code
    //---------------------------------------------------------------------------------------------------------------------
    public void KillEntity()
    {
        _beastScream.PlayOneShot(_beastScream.clip);
        GetComponent<RagdollManager>().EnableRagdoll(); // enable the ragdoll with no rigidbody force or torque applied
        PlayerStats.Score += _killScore; // update the score
        PlayerStats.EnemiesKilled++;
        PlayerStats.ActiveEnemies--; // update the enemies killed and active enemies
        Invoke("SpawnGoodies", 0.15f); // spawn goodies after 1 fiteenth of a second
    }

    //---------------------------------------------------------------------------------------------------------------------
    // Method: KillEntity, overload I
    // Desc:   Kills the entity and applies a general force to the main rigidbody collider in the ragdoll heirarchy
    //---------------------------------------------------------------------------------------------------------------------
    public void KillEntity(Vector3 direction, float force)
    {
        _beastScream.PlayOneShot(_beastScream.clip);
        GetComponent<RagdollManager>().EnableRagdoll(direction, force);
        PlayerStats.Score += _killScore;
        PlayerStats.EnemiesKilled++;
        PlayerStats.ActiveEnemies--;
        Invoke("SpawnGoodies", 0.15f);
    }

    //---------------------------------------------------------------------------------------------------------------------
    // Method: KillEntity, overload Ii
    // Desc:   Kills the entity and applies a force to a specific collider in the ragdolls heirarchy, like an arm or leg
    //---------------------------------------------------------------------------------------------------------------------
    public void KillEntity(Vector3 direction, float force, Collider hitCol)
    {
        _beastScream.PlayOneShot(_beastScream.clip);
        GetComponent<RagdollManager>().EnableRagdoll(direction, force, hitCol); // apply specific ragdoll force
        PlayerStats.Score += _killScore;
        ScoreText.SetText("Score: " + PlayerStats.Score);
        _scoreParticles[4].Emit(1);
        PlayerStats.EnemiesKilled++;
        PlayerStats.ActiveEnemies--;
        Invoke("SpawnGoodies", 0.15f);
    }

    //---------------------------------------------------------------------------------------------------------------------
    // Method: KillEntity, overload III
    // Desc:   Kills the entity and applies a force to a specific collider in the ragdolls heirarchy, like an arm or leg
    //         Also updates the score. This is the main killEntity method called by player scripts. previous methods maintained to
    //         maintain functionality
    //---------------------------------------------------------------------------------------------------------------------
    public void KillEntity(Vector3 direction, float force, Collider hitCol, ScoreAmount scoreAmount)
    {
        _beastScream.PlayOneShot(_beastScream.clip);
        GetComponent<RagdollManager>().EnableRagdoll(direction, force, hitCol);
        switch (scoreAmount)
        {
            case ScoreAmount.bodyShotScore:
                break;
            case ScoreAmount.HeadShotScore:
                break;
            case ScoreAmount.KillMeleeEnemy:
                PlayerStats.Score += _killScore;
                ScoreText.SetText("Score: " + PlayerStats.Score);
                _scoreParticles[(int)scoreAmount].Emit(1);
                PlayerStats.EnemiesKilled++;
                PlayerStats.ActiveEnemies--;
                break;
            case ScoreAmount.KillArmouredEnemy:
                PlayerStats.Score += _armouredEnemyScore;
                ScoreText.SetText("Score: " + PlayerStats.Score);
                _scoreParticles[(int)scoreAmount].Emit(1);
                PlayerStats.EnemiesKilled++;
                PlayerStats.ActiveEnemies--;
                PlayerStats.ActiveArmouredEnemies--;
                break;
            case ScoreAmount.KillFlamerEnemy:
                PlayerStats.Score += _flamerEnemyScore;
                ScoreText.SetText("Score: " + PlayerStats.Score);
                _scoreParticles[(int)scoreAmount].Emit(1);
                PlayerStats.EnemiesKilled++;
                PlayerStats.ActiveEnemies--;
                PlayerStats.ActiveFlamerEnemies--;
                break;
            default:
                break;
        }

        Invoke("SpawnGoodies", 0.15f);
    }

    //------------------------------------------------------------------
    // SpawnGoodies()
    // Spawns health pickups when player kills higher-up enemy
    // -----------------------------------------------------------------
    private void SpawnGoodies()
    {
        if (CompareTag(TagsHashIDs.ArmouredEnemy)) // Spawn one health pickup
        {
            if (_healthPickup != null)
            {
                _healthPickup[0].transform.SetParent(null);
                _healthPickup[0].gameObject.SetActive(true);
                _healthPickup[0].respawn = false; // Make sure the health pickup does not resawn after 60 seconds
            }

        }
        else if (CompareTag(TagsHashIDs.FlamerEnemy)) // Spawn as many health pickups as are in the heirarchy
        {
            if (_healthPickup != null)
            {
                for (int i = 0; i < _healthPickup.Length; i++)
                {
                    _healthPickup[i].transform.SetParent(null);
                    _healthPickup[i].gameObject.SetActive(true);
                    _healthPickup[i].respawn = false;
                }
            }
        }
    }
}
