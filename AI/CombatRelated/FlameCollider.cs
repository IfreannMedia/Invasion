using UnityEngine;
//------------------------------------------------------------------------------------------------------------------------
// CLASS:       FlameCollider
// Description: This class deals damage to the player, using onTrigger methods. Used by the flamer enemy when he attacks
//-------------------------------------------------------------------------------------------------------------------------
public class FlameCollider : MonoBehaviour
{

   
    private float Damage = 15f; // This script is attached to each hand, so each flamethrower deals this amount of damage
    private Collider _collider;
    private PlayerLifecycle _playerLifecycle; // The playerLifecycle.cs script is stored as a variable in the StateController.cs script, access through there instead of calling a FindObjectOfType()
    

    //-----------------------------------------------------------------------------------
    // Method: Awake
    // Desc:   Used to cache component references in this object's heirarchy
    //-----------------------------------------------------------------------------------
    private void Awake()
    {
        _collider = GetComponent<Collider>();

    }

    //-----------------------------------------------------------------------------------
    // Method: Start
    // Desc:   Used to cache component references not in this object's heirarchy
    //-----------------------------------------------------------------------------------
    private void Start()
    {
        _playerLifecycle = GetComponentInParent<StateController>().Player.GetComponent<PlayerLifecycle>();
    }

    //-----------------------------------------------------------------------------------
    // Method: OnEnable
    // Desc:   We only want the attack collider's to be enabled when the enemy is in an
    //         attack animation. We want to avoid erroneous calls of DealDamage() if the 
    //         the enemy is running at the player (for example) and not in attack anim
    //-----------------------------------------------------------------------------------
    private void OnEnable()
    {
        ToggleAttackColliderOn();
    }

    private void OnDisable()
    {
        CancelInvoke(); // Just in case the DealDamage() method call was not yet disabled
    }
    //-----------------------------------------------------------------------------------
    // Method: OnTriggerEnter
    // Desc:   Begins to take damage from player every half a second
    //-----------------------------------------------------------------------------------
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag(TagsHashIDs.Player))
        {
            InvokeRepeating("DealDamage", 0.0f, 0.50f);
            // Take Damage and ensure we are not calling TakeDamage multiple times for one attack
            //_playerLifecycle.TakeDamage(Damage);
           // ToggleAttackColliderOff();
        }
    }

    //-----------------------------------------------------------------------------------
    // Method: OnTriggerExit
    // Desc:   Stops taking damage from the player
    //-----------------------------------------------------------------------------------
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(TagsHashIDs.Player))
        {
            CancelInvoke("DealDamage");
        }
    }

    private void DealDamage()
    {
        _playerLifecycle.TakeDamage(Damage);
    }

    //-----------------------------------------------------------------------------------
    // Method: ToggleAttackColliderOn
    // Desc:   Turns on the attack trigger collider
    //-----------------------------------------------------------------------------------
    public void ToggleAttackColliderOn()
    {
        if (GetComponent<AudioSource>())
        {
            GetComponent<AudioSource>().Play();
        }
        if (!_collider.enabled)
        {
            _collider.enabled = true;

        }
    }

    //-----------------------------------------------------------------------------------
    // Method: ToggleAttackColliderOff
    // Desc:   Turns off the attack trigger collider
    //-----------------------------------------------------------------------------------
    public void ToggleAttackColliderOff()
    {
        if (GetComponent<AudioSource>())
        {
            GetComponent<AudioSource>().Stop();
        }
        if (_collider.enabled == true)
        {
            _collider.enabled = false;
            CancelInvoke(); // Just in case the DealDamage() method call was not yet disabled
        }
    }


}
