using UnityEngine;
// ----------------------------------------------------------------------------------------
// CLASS:       PlauUISound
// DESCRIPTION: Plays a UI sound effect when the mouse hovers over a button, and another
//              sound effect is played when the user clicks a button. This script also automatically
//              adds a trigger collider to the button in question so I don'thave to do it myself
// ----------------------------------------------------------------------------------------

[RequireComponent (typeof(BoxCollider2D))]
public class PlayUISound : MonoBehaviour
{
    AudioSource _enterSoundEffect, _clickSoundEffect;
    public bool PauseMenu = true; // I use this same script to facilitate playing UI Audio for both start/end game menu and in-game pause menu
    // So it has a boolean switch to manage that

    //----------------------------------------------------------------------------------
    // Method: Start()
    // Desc:   Cache audioSource components to the globabl varialbles. Make sure the 2D
    //         box collider is a trigger collider. I store audio in the PauseGame object,
    //         because some buttons the user clicks get deactivated and I want the audio clip
    //         to play in full and not be cut off after a user clicks a button. The pause menu
    //         canvas object does not get turned off
    //---------------------------------------------------------------------------------
    private void Start()
    {
        GetComponent<BoxCollider2D>().isTrigger = true; // make sure the collider is a trigger

        // If this is a button from the pause menu, we will find a PauseGame.cs file in the parent canvas
        if (PauseMenu)
        {
            _enterSoundEffect = GetComponentInParent<PauseGame>().MouseOverSound;
            _clickSoundEffect = GetComponentInParent<PauseGame>().MouseClickSound;
        }
        else
        {
            // Otherwise it's simply two audio sources stored as components in the parent canvas:
            AudioSource[] sources = GetComponentsInParent<AudioSource>();
            _enterSoundEffect = sources[0];
            _clickSoundEffect = sources[1];
        }
    }


    // OnMouseEnter()
    // Play a sound
    public void OnMouseEnter()
    {
        _enterSoundEffect.PlayOneShot(_enterSoundEffect.clip);
    }

    // OnMouseDown()
    // Play a sound
    public void OnMouseDown()
    {
        _clickSoundEffect.PlayOneShot(_clickSoundEffect.clip);
    }

}
