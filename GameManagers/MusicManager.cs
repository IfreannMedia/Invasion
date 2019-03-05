using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//---------------------------------------------------------------------------------------------------------
// CLASS:       MusicManager
// DESCRIPTION: This class dynamically fades between three audio tracks during runtime. It does this based
//              in-game level of tension. The level of tension is decided by the player's level of health and
//              the number of active enemies in the game
//---------------------------------------------------------------------------------------------------------
public class MusicManager : MonoBehaviour
{
    // Private fields; audio sources:
    private AudioSource _lowTensionMusic;
    private AudioSource _midTensionMusic;
    private AudioSource _highTensionMusic;
    private AudioSource _flamerEnemyMusic;
    private AudioSource _gameOverMusic;
    private AudioSource _currentMusic;

    private bool isFading = false; // Include a boolean switch just to make sure that a call to fade music is not made
                                   // while two music tracks are in the process of cross-fading

    // ------------------------------------------------------------------------------
    // Method:      Start()
    // Description: Sets up which audioSource is which. Game always starts on 
    //              Low tension music
    //-------------------------------------------------------------------------------
    private void Start()
    {
        AudioSource[] sources = new AudioSource[5];
        sources = GetComponents<AudioSource>();
        _lowTensionMusic = sources[0];
        _midTensionMusic = sources[1];
        _highTensionMusic = sources[2];
        _flamerEnemyMusic = sources[3];
        _gameOverMusic = sources[4];
        _currentMusic = _lowTensionMusic;
        _lowTensionMusic.Play();
    }

    // ---------------------------------------------------------------------------------------------------
    // Method:      CheckForCrossFade()
    // Description: This can be called by other scripts to begin a crossfade to the appropraite music track
    //              This is decided by the amount of active enemies / the player's health
    //              Returns immediately if two audio tracks are already crossfading
    //              Fades to _flamerEnemyMusic if a flamer enemy is in play
    // ----------------------------------------------------------------------------------------------------
    public void CheckForCrossFade()
    {
        // First case, the player is dead and we have not yet faded to game over music
        // We can start this coroutine even if a crossfade is currently in progress (unliikely but possible)
        if (PlayerStats.PlayerHealth <= 0.0f
            && _currentMusic != _gameOverMusic)
        {
            StartCoroutine(FadeToDeathMusic(_currentMusic, _gameOverMusic));
            return;
        }
        // Second case, check if we are already performing a fade, return if we are:
        if (isFading)
        {
            return;
        }
        // Third case: There is a flamer enemy after spawning and we have not yet faded to the appropriate music track:
        if (PlayerStats.ActiveFlamerEnemies > 0 
            && _currentMusic != _flamerEnemyMusic) 
        {
            StartCoroutine(CrossFadeAudio(_currentMusic, _flamerEnemyMusic));
            return;
        }
        // Or we are already playing the appropriate music, then return:
        if (PlayerStats.ActiveFlamerEnemies > 0
            && _currentMusic == _flamerEnemyMusic)
        {
            return;
        }

        // Fourth Case:
        // There is no flamer enemy in play, and the player is alive, so fade to appropriate gameplay music based on amount of enemies:

        if (PlayerStats.ActiveEnemies > 3 && PlayerStats.ActiveEnemies < 15
            && _currentMusic != _midTensionMusic)        // Between 10 and 18 enemies fade to mid-tension music
        {
            StartCoroutine(CrossFadeAudio(_currentMusic, _midTensionMusic));
        }
        else 
        if (PlayerStats.ActiveEnemies > 15
            && _currentMusic != _highTensionMusic)   // Between 18 and 24 enemies fade to high tension music
        {
            StartCoroutine(CrossFadeAudio(_currentMusic, _highTensionMusic));
        }
        else
        if (PlayerStats.ActiveEnemies < 3
            && _currentMusic != _lowTensionMusic)  // fade back to low tension if enemies go under 10
        {
            StartCoroutine(CrossFadeAudio(_currentMusic, _lowTensionMusic));
        }
    }

    // --------------------------------------------------------------------------------
    // IEnumerator: CrossfadeAudio(fromTrack, toTrack)
    // Description: Fades fromTrack to toTrack, ie performs a crossfade of audio
    //---------------------------------------------------------------------------------
    IEnumerator CrossFadeAudio(AudioSource fromTrack, AudioSource toTrack)
    {
        isFading = true;
        toTrack.volume = 0.0f;
        toTrack.Play();

        while (toTrack.volume < 1.0f)
        {
            fromTrack.volume    -= 0.1f;
            toTrack.volume      += 0.1f;
            yield return new WaitForSeconds(0.25f);
        }
        fromTrack.volume = 0.0f;
        fromTrack.Stop();
        _currentMusic = toTrack;
        isFading = false;
        yield return null;
    }

    IEnumerator FadeToDeathMusic(AudioSource fromTrack, AudioSource toTrack)
    {
        // we must fade to the death screen music every time the player times, ragardless if a cross- fade was already in progress
        // So make sure we disable any music that's playing, then perform the fade:

        if (_lowTensionMusic != fromTrack 
            && _lowTensionMusic.isPlaying) // in this case, the _lowTensionMusic is playing, despite not having yet been assigned as the _currentMusic:
        {
            _lowTensionMusic.Stop();
        }
        if (_midTensionMusic != fromTrack
            && _midTensionMusic.isPlaying)
        {
            _midTensionMusic.Stop();
        }
        if (_highTensionMusic != fromTrack
            && _highTensionMusic.isPlaying)
        {
            _highTensionMusic.Stop();
        } 
        if (_flamerEnemyMusic != fromTrack
            && _flamerEnemyMusic.isPlaying)
        {
            _flamerEnemyMusic.Stop();
        }

        isFading = true;
        toTrack.volume = 0.0f;
        toTrack.Play();

        while (toTrack.volume < 1.0f)
        {
            fromTrack.volume -= 0.1f;
            toTrack.volume += 0.1f;
            yield return new WaitForSeconds(0.25f);
        }

        fromTrack.volume = 0.0f;
        fromTrack.Stop();
        _currentMusic = toTrack;
        isFading = false;
        yield return null;
    }
}
