using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;
using UnityEngine.UI;
using TMPro;

// ------------------------------------------------------------------------------------------------
// CLASS:       LevelManager.cs
// DESCRIPTION: Controls loading of levels, restarting levels etc.
// --------------------------------------------------------------------------------------------------
public class LevelManager : MonoBehaviour
{
    // Private fields
    [SerializeField] private TextMeshProUGUI _gameTimer;    // Display timer
    [SerializeField] private Camera         _levelEndCamera;// Camera for displaying final score
    [SerializeField] private Canvas         _endLevelCanvas;// Canvas for displaying final score
    [SerializeField] private GameObject     _loadingText;   // Text that appears on loading screen
    private GameObject   _player;
    private EnemySpawner _spawnScript;


    // Public fields
    [HideInInspector] public float secondsElapsed;
    public Image AlphaFade;

    // ----------------------------------------------------------------
    // Method:      Start()
    // Description: set timer to 0, get the player and spawner objects,
    //              and start the GameTimer IEnumerator
    // ----------------------------------------------------------------
    void Start ()
    {
        secondsElapsed = 0.0f;
        _player = FindObjectOfType<MoveScript>().gameObject;
        _spawnScript = FindObjectOfType<EnemySpawner>();
        StartCoroutine(GameTimer());
       // AnalyticsEvent.GameStart();
	}

    // ----------------------------------------------------------------
    // Method:      Update()
    // Description: Times the game
    // ----------------------------------------------------------------
    private void Update()
    {
        secondsElapsed += Time.deltaTime;

        if ( Input.GetKeyDown(KeyCode.Backspace))
        {       
            ReloadScene();
        }
    }

    // ----------------------------------------------------------------
    // IEnumerator: GameTimer()
    // Description: Updates the time display every second
    // ----------------------------------------------------------------
    private IEnumerator GameTimer()
    {
        while (PlayerStats.PlayerHealth > 0.0f)
        {
            yield return new WaitForSeconds(1.0f);
            string minutes = ((int)secondsElapsed / 60).ToString();
            float seconds = (secondsElapsed % 60);
            if (seconds < 10)
            {
                string secondsString = "0" + seconds.ToString("F0");
                _gameTimer.text = minutes + ":" + secondsString;

            }
            else
            {
                string secondsString = seconds.ToString("F0");
                _gameTimer.text = minutes + ":" + secondsString;
            }
        }
        yield return null;
    }

    // ----------------------------------------------------------------
    // Method:      ReloadScene()
    // Description: reloads the scene
    // ----------------------------------------------------------------
    public void ReloadScene()
    {
        //AnalyticsEvent.Custom("Player Reloaded the Level");
        StartCoroutine(RestartLevel());
    }

    // ----------------------------------------------------------------
    // Method:      BackToMainMenu()
    // Description: Loads the main menu "StartScene"
    // ----------------------------------------------------------------
    public void BackToMainMenu()
    {
        StartCoroutine(BackToMain());
    }


    // ----------------------------------------------------------------
    // IEnumerator: EndLevelSequence()
    // Description: Stops enemies spawning, disables user input,
    //              ragdolls all active enemies, fades to the score counter
    //              screen and disables user cameras
    // ----------------------------------------------------------------
    public IEnumerator EndLevelSequence()
    {
        _spawnScript.StopAllCoroutines();
        _spawnScript.CancelInvoke();
        DisableUserInput();

        RagdollManager[] enemies = FindObjectsOfType<RagdollManager>();
        foreach (RagdollManager enemy in enemies)
        {
            enemy.EnableRagdoll(); // enable each enemy ragdoll
            yield return new WaitForEndOfFrame(); // put 1 frame between each ragdoll
        }

        Camera[] playerCams = new Camera[2];
        playerCams = _player.GetComponentsInChildren<Camera>();

        AlphaFade.GetComponent<Animator>().SetTrigger("Fade");
        yield return new WaitUntil(() => AlphaFade.color.a == 1); // fade screen to black

        _player.gameObject.SetActive(false);
        _levelEndCamera.GetComponent<AudioListener>().enabled = true;
        _levelEndCamera.enabled = true;
        playerCams[0].enabled = false;
        playerCams[1].enabled = false;

        AlphaFade.GetComponent<Animator>().SetTrigger("Fade");
        yield return new WaitUntil(() => AlphaFade.color.a == 0); // fade screen to clear
        _endLevelCanvas.gameObject.SetActive(true); // activate the score counter canvas object

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // ------------------------------------------------------------------
    // IEnumerator: RestartLevel()
    // Description: Stops enemies spawning, disables user input,
    //              ragdolls all active enemies, reload the current scene
    // ------------------------------------------------------------------
    private IEnumerator RestartLevel()
    {
        _spawnScript.StopAllCoroutines();
        _spawnScript.CancelInvoke();
        DisableUserInput();

        RagdollManager[] enemies = FindObjectsOfType<RagdollManager>();
        foreach (RagdollManager enemy in enemies)
        {
            enemy.EnableRagdoll();
            yield return new WaitForEndOfFrame();
        }

        AlphaFade.GetComponent<Animator>().SetTrigger("Fade");
        yield return new WaitUntil(() => AlphaFade.color.a == 1);

        _endLevelCanvas.gameObject.SetActive(false);
        ResetStats();
        _loadingText.SetActive(true);

        yield return new WaitForSeconds(3.0f); // Wait for three seconds otherwise users will not be able to read the loading hint

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    // ------------------------------------------------------------------
    // IEnumerator: BackToMain()
    // Description: Stops enemies spawning, disables user input,
    //              ragdolls all active enemies, reloads start scene
    // ------------------------------------------------------------------
    private IEnumerator BackToMain()
    {
        // Disable gameplay
        _spawnScript.StopAllCoroutines();
        _spawnScript.CancelInvoke();
        DisableUserInput();

        // Ragdoll enemies
        RagdollManager[] enemies = FindObjectsOfType<RagdollManager>();
        foreach (RagdollManager enemy in enemies)
        {
            enemy.EnableRagdoll();
            yield return new WaitForEndOfFrame();
        }

        AlphaFade.GetComponent<Animator>().SetTrigger("Fade");
        yield return new WaitUntil(() => AlphaFade.color.a == 1);
        _endLevelCanvas.gameObject.SetActive(false);
        ResetStats();

        // Load start scene
        _loadingText.SetActive(true);
        yield return new WaitForEndOfFrame();
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(0);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }


    // ----------------------------------------------------------------
    // Method:      DisableUserInput()
    // Description: Disables user input
    // ----------------------------------------------------------------
    private void DisableUserInput()
    {
        _player.GetComponent<MoveScript>().enabled = false;
        _player.GetComponentInChildren<MouseLook>().enabled = false;
        _player.GetComponentInChildren<Animator>().enabled = false;
        _player.GetComponentInChildren<FPSRigAnimator>().enabled = false;
    }

    // ----------------------------------------------------------------
    // Method:      ResetStats()
    // Description: Reset gameplay statistics
    // ----------------------------------------------------------------
    private void ResetStats()
    {
        PlayerStats.EnemiesKilled = 0;
        PlayerStats.Score = 0;
        PlayerStats.ActiveEnemies = 0;
        PlayerStats.ActiveArmouredEnemies = 0;
        PlayerStats.ActiveFlamerEnemies = 0;
    }

}
