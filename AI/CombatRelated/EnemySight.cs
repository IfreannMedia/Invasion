using UnityEngine;
using UnityEngine.AI;

// ----------------------------------------------------------------------------------------------------------------------------------
// CLASS: EnemySight
// DESC:  This script alllows enemies to "see" - the chosen method is to first detect if the player is in sight range using 
//        the trigger collider sphere which is centred on the enemies head. Then check if the player is in an enemies calculated FOV
//        If the player is in the enemies' field of view, perform a raycast to check if the enemy has a direct line of sight to the player
//        If the enemy loses sight, store the vector3 position as the last known position of the player
// ----------------------------------------------------------------------------------------------------------------------------------
public class EnemySight : MonoBehaviour
{
    
    // Public Fields

    [HideInInspector] public bool     PlayerInSight      = false;         // Can we see the player currently
    [HideInInspector] public Vector3  _lastKnownPosition = Vector3.zero;  // Store the last known position here
    
    // Serialized Fields
    [SerializeField] [Range(20f, 270f)] float _fov    = 110; // The field of view of this enemy

    // Private Fields
    private GameObject       _player            = null; // Assign in the inspector to avoid using FindObjectOfType()
    private SphereCollider  _sphereCollider     = null; // The trigger collider of enemy
    private TagsHashIDs     _LayerMasks;                // Get access to bit-shifted layermasks here
    private int             _AITriggerLayerMask;        // Layer mask so that raycast does not detect AITriggers
    private float           _timer = 0.0f;              // I use a timer to avoid updating too often in the ontriggerstaymethod
    private Animator        _anim;

    //-------------------------------------------------------------
    // Method: Awake()
    // Desc:   Get connections to components
    //-------------------------------------------------------------
    private void Awake()
    {
        _sphereCollider = GetComponent<SphereCollider>();
        _anim           = GetComponentInParent<Animator>();
    }

    //-------------------------------------------------------------------------------------------------------------------------
    // Method: Start()
    // Desc:   Get connections to components on other gameobjects, which would have been set up in their own Awake() method
    //         ie StateController.cs grabs the player in it's awake() method, so we get that reference in the Start method 
    //         to avoid a null reference.
    //--------------------------------------------------------------------------------------------------------------------------
    private void Start()
    {
        _LayerMasks = GetComponentInParent<StateController>().TagsHash;
        _AITriggerLayerMask = _LayerMasks.AITriggerLayerMask | _LayerMasks.EnemyLayerMask | _LayerMasks.IgnoreRaycastLayerMask; 
        // We have to include the ignore raycast layer,
        // because spawn trigger colliders are on this layer and if the sight script casts against them the player will never be seen
        // I don't include the enemy layer in the mask, because the enemy would not see the player
        // As his sight was being blocked by (presumably) his own collider - at least this is what happened suring testing
        _AITriggerLayerMask = ~_AITriggerLayerMask;

        // Only get a reference to the player if he is alive
        // Avoid a null reference if an enemy spawns after the player has been killed - should not happen but just in case
        if (PlayerStats.PlayerHealth > 0.0f)
            _player = GetComponentInParent<StateController>().Player.gameObject;
    }

    //-------------------------------------------------------------------------------------------------------------------------
    // Method: OnTriggerStay()
    // Desc:   detect if the player is in the field of view of the enemy. if the player is, then perform a raycast to detect the
    //         line of sight. do not perform a raycast every update frame, but rather, use a timer to perform it every tenth of a second
    //         which is probably more than enought to simulate responsive enemy behaviour
    //--------------------------------------------------------------------------------------------------------------------------
    private void OnTriggerStay(Collider other)
    {
        _timer += Time.deltaTime;

        if (_timer < 0.1f)
            return;
        // If a tenth of a sec has passed and the player is in the detection range:
        else if (other.gameObject == _player && _timer >= 0.1f)
        {

            PlayerInSight = false;
            if (_LayerMasks)    // check for layermasks as it was giving a null reference exception sometimes
            _anim.SetBool(_LayerMasks.SeesPlayer, PlayerInSight);
            // Get the direction and angle
            Vector3 direction = other.transform.position - transform.position;
            float angle = Vector3.Angle(direction, transform.forward); // get the angle between the direction to the player and the forward vec

            // multiply by 0.5f to get half of the field of view angle
            if (angle < _fov * 0.5f)
            {
                // Now test if the enemy has a clear line of sight towards player raycasting direction, distance = trigger sphere radius
                RaycastHit hit;

                if (Physics.Raycast(transform.position, direction, out hit, _sphereCollider.radius, _AITriggerLayerMask))
                {
                    if (hit.collider.gameObject == _player)
                    {
                        PlayerInSight = true;
                        _anim.SetBool(_LayerMasks.SeesPlayer, PlayerInSight); // also tell the animator that the player is in sight
                        _lastKnownPosition = _player.transform.position; // Set the last known position every time we raycast, keep it if we lose sight
                    }
                } 
            }
            _timer = 0.0f; // reset the timer
        }

    }
}
