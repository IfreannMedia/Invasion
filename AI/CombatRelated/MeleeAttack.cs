using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// -----------------------------------------------------------------------------------
// CLASS: MeleeAttack
// DESCRIPTION: This class performs an ontriggerenter function to detract from players health
// -----------------------------------------------------------------------------------
public class MeleeAttack : MonoBehaviour {
    // Public fields
    public float Damage = 10f;

    // Private fields
   // public Collider AttackCollider;
    private AudioSource BeastGrowl;
    private PlayerLifecycle _playerLifecycle;
    private Collider _collider;
    private Camera mainCam;
    private GameObject[] enemiesOncreen;
    private SkinnedMeshRenderer _body;
    private TrailRenderer _trail;


    private void Awake()
    {
        _trail = GetComponent<TrailRenderer>();
        _trail.enabled = false;
        BeastGrowl = GetComponent<AudioSource>();
        _collider = GetComponent<Collider>();

    }

    // Get reference to player through the state controller
    private void Start()
    {
        StateController controller = GetComponentInParent<StateController>();
        _playerLifecycle = controller.Player.GetComponent<PlayerLifecycle>();
        _body = controller.GetComponentInChildren<SkinnedMeshRenderer>();
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag(TagsHashIDs.Player))
        {
            _playerLifecycle.TakeDamage(Damage);
            ToggleAttackColliderOff(); // Ensure we do not erroneously call TakeDamage incosistently
        }
    }

    public void ToggleAttackColliderOn()
    {
        if (_body.isVisible)
        {
            int randomChance = Random.Range(0, 100);
            if (randomChance <=25) // 1 in 4 chance we make an attack noise if we are on screen
            {
                BeastGrowl.pitch = Random.Range(1, 1.3f);
                BeastGrowl.PlayOneShot(BeastGrowl.clip);
            }
        }
        else
        {
            BeastGrowl.pitch = Random.Range(1, 1.3f);
            BeastGrowl.PlayOneShot(BeastGrowl.clip);
        }

        // If collider is disabled we are not attacking:
        if (_collider.enabled == false)
        {

            _collider.enabled = true;

        }

    }
    public void ToggleAttackColliderOff()
    {
        if (_collider.enabled == true)
            _collider.enabled = false;
    }


    public void ToggleTrailRendererOn()
    {
        _trail.enabled = true;
    }

    public void ToggleTrailRendererOff()
    {
        _trail.enabled = false;
    }
}
