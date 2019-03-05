using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// ----------------------------------------------------------------------------------------------------------
// CLASS:       PauseGame.cs
// DESCRIPTION: listens for user input to pause the game, pauses the game using a time-slow down effect
//              and camera zoom effect. Pause menu buttons can call methods contained in this script.
//              Audio sources are stored on this object, the pause menu canvas, which is always set to active
// ----------------------------------------------------------------------------------------------------------
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(AudioSource))] 
public class PauseGame : MonoBehaviour
{

    [Header ("Pause Menu Items")]
    public GameObject[] MenuItems;     // an array for the menu items that should appear when the user pauses play
    [Header("Controls Menu Items")]
    public GameObject[] ControlsItems; // an array for the menu items to be toggled on/off when the user wants to view the control setup

    private bool _pauseMenuActive = false;
    private FPSRigAnimator _playerWeaponScript; // reference to the player
    public AudioListener Listener;              // reference to the audio listener for adjusting gloabl volume
    public Slider VolumeSlider;

    private AudioSource _enableSoundEffect, _disableSoundEffect, _slowDownSoundEffect, _speedUpSoundEffect;
    [HideInInspector] public AudioSource MouseOverSound, MouseClickSound;
    private float originalFOV; // Reference to original field of view
    private bool isPausing = false;  // boolean to detect if we are pausing the game
    private string _pause = "Pause"; // sting for mapping user input

    // ----------------------------------------------------------------------------------
    // Method:      Awake()
    // Description: Assign and set up audio sources
    // ----------------------------------------------------------------------------------
    private void Awake()
    {
        AudioSource[] sources = GetComponents<AudioSource>();
        _enableSoundEffect = sources[0];
        _disableSoundEffect = sources[1];
        _slowDownSoundEffect = sources[2];
        _speedUpSoundEffect = sources[3];
        MouseOverSound = sources[4];
        MouseClickSound = sources[5];
        // Make sure that each audio source is configured correctly:
        foreach (AudioSource item in sources)
        {
            item.playOnAwake = false;
            item.spatialBlend = 0.0f;
            item.bypassReverbZones = true;
        }
    }

    // ----------------------------------------------------------------------------------
    // Method:      Start()
    // Description: Cache components
    // ----------------------------------------------------------------------------------
    private void Start()
    {
        _playerWeaponScript = FindObjectOfType<FPSRigAnimator>();
        PlayerStats.CanPauseGame = true; // make sure we re-enable the ability to pause the game if this object becomes active in a scene
                                         // This is because pausing the game during the scene load can lead to glitches like a black screen/things not loading correctly

        originalFOV = Camera.main.fieldOfView;
    }

    // ------------------------------------------------------------------------------------
    // Method:      Update()
    // Description: Listen for player input, and (un)pause the game if appropriate to do so
    // ------------------------------------------------------------------------------------
    void Update () {

        if (Input.GetButtonDown(_pause) && !_pauseMenuActive)
        {
            if (PlayerStats.PlayerHealth > 0 || !PlayerStats.CanPauseGame) // We cannot pause the game if the game has ended and we are looking at the final score
            {
                StartCoroutine(PauseTheGame());
            }
        }
        else if (Input.GetButtonDown(_pause) && _pauseMenuActive)
        {
            StartCoroutine(UnpauseTheGame());
        }
	}


    // Public method for pausing the game
    public void EnablePauseMenu()
    {
        StartCoroutine(PauseTheGame());
    }
    // public method for unpausing the game, called from pause UI buttons that need to disable the pause menu, for example, the retry button
    public void DisablePauseMenu()
    {
        StartCoroutine(UnpauseTheGame());
    }
    
    // ---------------------------------------------------------------------------------------------
    // IEnumerator: PauseTheGame
    // Description: Pauses the game by incrementally slowing the timescale until it reaches epsiolon,
    //              then activates pause canvas UI elements, also plays SFX. INcrementally decreases the main 
    //              camera's FOV to mimic a zooming in effect as the timescale is slowed
    // ---------------------------------------------------------------------------------------------
    public IEnumerator PauseTheGame()
    {
        // Boolean switch to prevent starting the coroutine if it is already running:
        if (isPausing)
        {
            yield return null;
        }
        isPausing = true;

        _slowDownSoundEffect.PlayOneShot(_slowDownSoundEffect.clip);
        Camera cam = Camera.main;
        float targetFOV = cam.fieldOfView - 8.0f; // the target FOV to move towards
        float targetTimeScale = Mathf.Epsilon; // the target timescalse - I found issues were caused when setting it to 0, so I use epsilon

        // Tried a few different ways to facillitate a slow-down and zoom-in effect like lerping and smoothStepping
        // eventually settled on using a stepcounter loop with the Mathf.SmoothStep method, then snapping the value just in case:
        int stepCounter = 0;
        while (stepCounter != 6)
        {
            cam.fieldOfView = Mathf.SmoothStep(cam.fieldOfView, targetFOV, 0.25f);
            Time.timeScale = Mathf.SmoothStep(Time.timeScale, targetTimeScale, 0.25f);
            stepCounter += 1;
            yield return new WaitForSeconds(0.01f);
        }
        // snap the timescale and fov just in case
        Time.timeScale = Mathf.Epsilon;
        _enableSoundEffect.PlayOneShot(_enableSoundEffect.clip);

        // In the start area scene, the player game object is inactive when this script's Start() method is called
        // So perform a findobjectoftype call just in case we do not already have a reference to the player
        if (!_playerWeaponScript) 
        {
            _playerWeaponScript = FindObjectOfType<FPSRigAnimator>();
            // If we still cannot get a reference to the player something has gone wrong and we should return 
            // It's highly unlikely we'lll enter into this conditional though
            if (!_playerWeaponScript)
            {
                yield return null;
            }
        }
        _playerWeaponScript.CanFire = false;
        _pauseMenuActive = true;

        foreach (GameObject item in MenuItems)
        {

            item.SetActive(true);
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isPausing = false; // reset the boolean switch
        yield return null;
    }

    // ---------------------------------------------------------------------------------------------
    // IEnumerator: UnPauseTheGame
    // Description: Does the same thing as the PauseTheGame IEnumerator, but in reverse order
    // ---------------------------------------------------------------------------------------------
    public IEnumerator UnpauseTheGame()
    {
        if (isPausing)
        {
            yield return null; 
        }
        isPausing = true;

        _speedUpSoundEffect.PlayOneShot(_speedUpSoundEffect.clip);
        Camera cam = Camera.main;
        float targetFOV = originalFOV;
        float targetTimeScale = 1.0f;
        int stepCounter = 0;
        // deactivate menu items before speeding up time and zomming back out:
        foreach (GameObject item in MenuItems)
        {
            item.SetActive(false);
        }
        foreach (GameObject item in ControlsItems)
        {
            if (item.activeSelf)
            {
                item.SetActive(false);
            }
        }

        _disableSoundEffect.PlayOneShot(_disableSoundEffect.clip);
        _speedUpSoundEffect.PlayOneShot(_speedUpSoundEffect.clip);
        // Update the timescale and main cam FOV
        while (stepCounter != 6)
        {
            cam.fieldOfView = Mathf.SmoothStep(cam.fieldOfView, targetFOV, 0.25f);
            Time.timeScale = Mathf.SmoothStep(Time.timeScale, targetTimeScale, 0.25f);
            stepCounter += 1;
            yield return new WaitForSeconds(0.01f);
        }
        // snap the timescale and fov just in case
        Time.timeScale = 1.0f;
        cam.fieldOfView = originalFOV;

        _playerWeaponScript.CanFire = true;
        _pauseMenuActive = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        isPausing = false;
        yield return null;
    }

    /*
     *  Code used to open survey when it was included in build
    public void OpenSurveyPage()
    {
        Application.OpenURL("https://warrenroche.typeform.com/to/ua6Dil");
    }
     */

    // Quit the game
    public void QuitGame()
    {
        Application.Quit();
    }

    // adjust game volume
    public void AdjustGlobalVolume()
    {
        AudioListener.volume = VolumeSlider.value;
    }

    // ----------------------------------------------------------------------------------
    // Method:      ToggleControls()
    // Description: simple toggle, checks for the active state of the gameobject in the 
    //              ControlsItems array then switches it's active state using a ternery operator
    // ----------------------------------------------------------------------------------
    public void ToggleControls()
    {
        foreach (GameObject item in ControlsItems)
        {
            item.SetActive(item.activeSelf ? false : true);
        }
    }
}
