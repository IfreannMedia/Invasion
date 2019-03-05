using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
// -------------------------------------------------------------------------
// CLASS:      PlayerLifecycle, Implements IDamageable and IKillable
// DEFINITION: Control the flow of in-game events depending on player health
//             level ie audio and visual feedback. Enemies damage the player 
//             through public methods and they may also kill the player
// -------------------------------------------------------------------------
public class PlayerLifecycle : MonoBehaviour, IDamageable, IKillable
{
  
    // Public Fields
    public MusicManager MusicManager; // Public access to the Music Manager file, assigned in inspector
    public Transform    PlayerAudio;  // Transform containing player audio sources
    public LevelManager LevelManager; // The level manager

    // Private Fields
    private MoveScript _moveScript;
    private MouseLook  _mouseLook;
    private Animator   _playerAnim;

    // Audio Sources
    private AudioSource PlayerHurt;         // Audio: player gets hurt
    private AudioSource HeartBeatSource;    // Audio: player low on health
    private AudioSource BreathSource;       // Audio: player low on health
    private AudioSource _playerDeathMusic;  // Audio: player gets killed
    private AudioSource _gainHealthAudio;   // Audio: player gains health

    [SerializeField] private Slider _healthSlider;

    // PostProcessing Effects - Adapted code from Unities post-processing Github, had to implement these effects through script
    public Color GainHealthCol;             // Target vignette color for player gaining health
    public Color GetHurtCol;                // Target vignette color for player losing health
    private PostProcessVolume _ppVolume;    // Post Processing volume
    private Vignette _playerVignette;       // Vignette
    private ChromaticAberration _chromAbr;  // Chromatic Aberration

    // -----------------------------------------------------------------------------
    // METHOD:      Awake()
    // DESCRIPTION: Cache global variables, set up audio sources. Creates a 
    //              postProcessing volume with a vignette and chromatic abbration
    //              which are used for extra feedback when player is hit/low on health
    // -------------------------------------------------------------------------------
    private void Awake()
    {
        // Code to get player hurt audio sources:
        AudioSource[] sources = PlayerAudio.GetComponents<AudioSource>();
        PlayerHurt = sources[0];
        HeartBeatSource = sources[1];
        BreathSource = sources[2];
        _playerDeathMusic = sources[3];
        _gainHealthAudio = sources[4];

        _moveScript = GetComponent<MoveScript>();
        _mouseLook = GetComponentInChildren<MouseLook>();
        _playerAnim = GetComponentInChildren<Animator>();
        LevelManager = FindObjectOfType<LevelManager>();
        PlayerStats.PlayerHealth = 100.0f; // Make sure the player is beginning the game with full health


        // Added a conditional for the value assignment because there is no health slider in the start area scene
        if (_healthSlider)
        {
            _healthSlider.value = PlayerStats.PlayerHealth;
        }
        // Set up the post processing volume:
        _playerVignette = ScriptableObject.CreateInstance<Vignette>(); // create the vignette
        _playerVignette.enabled.Override(true);
        _playerVignette.intensity.Override(0.0f);
        _chromAbr = ScriptableObject.CreateInstance<ChromaticAberration>(); // Create and set up chromatic abb
        _chromAbr.enabled.Override(true);
        _chromAbr.intensity.Override(0.0f);
        _ppVolume = PostProcessManager.instance.QuickVolume(gameObject.layer, 100f, _playerVignette, _chromAbr); // assign the pp Volume
    }

    // -------------------------------------------------------------------------
    // METHOD:     TakeDamage
    // DEFINITION: Interface method, takes damage, called from enemy scripts
    //             Also makes the vignette appear (red) and if the player's health is
    //             below 30, low health audio starts to play
    // -------------------------------------------------------------------------
    public void TakeDamage(float damage)
    {
        if (PlayerStats.PlayerHealth <= 0.0f)
        {
            return;
        }
        // First I check if the low-health audio is playing, so it can be temporarily stopped
        // when the 'player-getting-hurt' audio is playing, and then restarted if appropriate
        // This is how it is done in the Doom franchise
        // And also the player should not grunt, if he is breathing raggedly at the same time
        if (HeartBeatSource.isPlaying)
        {
            // Both of these combined are set to play when the player is low on health
            // I use two seperate audio sources because it offered in-Engine control for volume levels
            HeartBeatSource.Pause();
            BreathSource.Pause();
        }

        StartCoroutine(AdjustVignette(GetHurtCol, 0.5f));
        PlayerHurt.pitch = Random.Range(0.9f, 1.1f);
        PlayerHurt.PlayOneShot(PlayerHurt.clip);
        PlayerStats.PlayerHealth -= damage;
        _healthSlider.value = PlayerStats.PlayerHealth;

        if (PlayerStats.PlayerHealth < 30.0f && PlayerStats.PlayerHealth > 0.0f)
        {
            _chromAbr.intensity.Override(0.25f);
            if (PlayerHurt.isPlaying)
            {
                HeartBeatSource.PlayDelayed(PlayerHurt.clip.length);
                BreathSource.PlayDelayed(PlayerHurt.clip.length);
            }
            else
            {
                HeartBeatSource.UnPause();
                BreathSource.UnPause();
                // If the audio was not paused to begin with, this next conditional will return true
                if (!HeartBeatSource.isPlaying)
                {
                    // Meaning we wish to begin playback of the two audio clips
                    HeartBeatSource.Play();
                    BreathSource.Play();
                }
            }
        }
        if (PlayerStats.PlayerHealth <= 0.0f)
            KillEntity();
    }

    // -------------------------------------------------------------------------
    // METHOD:     GainHealth(healthIncrement)
    // DEFINITION: Gives the player health. Called from Pickup.cs when the player 
    //             Picks up some health. Adjusts the vignette(green)
    // -------------------------------------------------------------------------
    public void GainHealth(float healthIncrement)
    {
        StartCoroutine(AdjustVignette(GainHealthCol, 0.4f));
        _gainHealthAudio.PlayOneShot(_gainHealthAudio.clip);
        
        //Add health to player, stop if health reaches 100 points:
        for (int i = 0; i < healthIncrement; i++)
        {
            if (PlayerStats.PlayerHealth < 100)
            {
                PlayerStats.PlayerHealth++;
            }
        }
        _healthSlider.value = PlayerStats.PlayerHealth;
        // If the player's health has risen above 30 points (low health) then disable associated feeback
        if (PlayerStats.PlayerHealth > 30.0f)
        {
            _chromAbr.intensity.Override(0.0f); // chromatic abbration 
            HeartBeatSource.Stop(); // audio
            BreathSource.Stop(); // audio
        }
    }

    // -------------------------------------------------------------------------
    // METHOD:     KillEntity
    // DEFINITION: Interface method, kills the player, called from enemy scripts
    //             Adjusts vignette, also plays death audio. sends analytical data
    //             Starts the EndLevelSequence IENumerator in LevelManager.cs
    // -------------------------------------------------------------------------
    public void KillEntity()
    {
        Analytics.CustomEvent("Player was killed", new Dictionary<string, object>
        {
            { "Time Played", Time.time },
            { "Kills", PlayerStats.EnemiesKilled }
        });
        StartCoroutine(AdjustVignette(GetHurtCol, 1.0f));
        MusicManager.CheckForCrossFade();
        _playerDeathMusic.PlayOneShot(_playerDeathMusic.clip);
        LevelManager.StartCoroutine(LevelManager.EndLevelSequence());
        HeartBeatSource.enabled = false;
        BreathSource.enabled = false;
        _moveScript.enabled = false;
        _mouseLook.enabled = false;
        _playerAnim.enabled = false;
    }

    // -------------------------------------------------------------------------
    // IEnumerator: AdjustVignette(Color, intensity)
    // DEFINITION:  Catch-all coroutine for fading a vignette in and out. can be
    //              any colour and intensity. Used for health gain and damage
    // -------------------------------------------------------------------------
    IEnumerator AdjustVignette(Color toColour, float intensity)
    {

        _playerVignette.color.Override(toColour);

        while (_playerVignette.intensity <= intensity)
        {
            _playerVignette.intensity.Override(_playerVignette.intensity.value += 0.1f);
            yield return new WaitForSeconds(0.01f);
        }
        while (_playerVignette.intensity > 0.0f)
        {
            _playerVignette.intensity.Override(_playerVignette.intensity.value -= 0.05f);
            yield return new WaitForSeconds(0.05f);
        }
        yield return null;

    }

    // -------------------------------------------------------------------------
    // METHOD:     OnDisable
    // DEFINITION: Destroys the post-processing volume we created in Awake()
    // -------------------------------------------------------------------------
    private void OnDisable()
    {
        RuntimeUtilities.DestroyVolume(_ppVolume, true, true);
    }
}
