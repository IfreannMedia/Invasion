using UnityEngine;
// ------------------------------------------------------------------------------------------------
// CLASS:       Target.cs
// DESCRIPTION: Controls the floating red targets in the start screen
//              offers the public method GetShot(), which is called by gun.cs (or any derived class)
// --------------------------------------------------------------------------------------------------
public class Target : MonoBehaviour
{

    private ParticleSystem[] _getShotParticles = new ParticleSystem[2];
    private AudioSource[]    _audioSources     = new AudioSource[2];
    private MeshRenderer     _meshRend;
    private Collider         _collider;

    // ------------------------------------
    // Method:     Start()
    // escription: Caches global variables
    // ------------------------------------
    private void Start()
    {
        _getShotParticles = GetComponentsInChildren<ParticleSystem>();
        _meshRend         = GetComponent<MeshRenderer>();
        _collider          = GetComponent<Collider>();
        _audioSources     = GetComponents<AudioSource>(); // Because there's only two audio sources, I keep them on this Target object as components

    }

    // ----------------------------------------------------------------------------
    // Method:      GetShot()
    // Description: Plays audio, disables mesh and collider, plays particle systems
    //              sets target to reanable after 15 seconds
    // ----------------------------------------------------------------------------
    public void GetShot()
    {
        _audioSources[0].Play();
        _meshRend.enabled = false;
        _collider.enabled  = false;

        foreach (ParticleSystem ps in _getShotParticles)
        {
            ps.Play();
        }

        Invoke("ReEnableTarget", 15.0f);
    }


    // -------------------------------
    // Method:      ReEnableTarget()
    // Description: Reenables target 
    // -------------------------------
    private void ReEnableTarget()
    {
        _audioSources[1].Play();
        _meshRend.enabled = true;
        _collider.enabled = true;
    }
}
