using UnityEngine;
using UnityEngine.AI;
// ---------------------------------------------------------------------------------------------------------------------------
// CLASS: EnemyAnimation.cs
// DESC:  Responsible for controlling animations, couples the animation and navigation systems for smooth animations. 
//        Agent's use root rotation when the angle they wish to turn is greater than a 120 degree threshold, otherwise this
//        script manually rotates the agent to look at their target. Each agent has their rotation manually updated here if they are in
//        an attack state, so that enemies who fire weapons have their weapons pointed in the direction of the player.
//        Also includes Animation Event methods which are called by enemy attack animations (turning colliders on/off for example)
// ---------------------------------------------------------------------------------------------------------------------------
public class EnemyAnimation : MonoBehaviour
{

    // Public fields
    [Range(3f,80f)]public float DeadZone = 5.0f; // The deadzone prevents the agent from oversteering if the angle the agent wishes to turn is too small
    public Transform _leftHandFlamer, _rightHandFlamer; // parent transform of flamer enemies flame attack (contains hit collider etc)


    // Private fields
    private NavMeshAgent     _navAgent;
    private Animator         _anim;
    private TagsHashIDs      _hashIDs;
    private AnimatorSetup    _animatorSetup; // Helper script used to set up animations, constructed in the Awake() method
    private EnemyGun         _enemyGun;      // Reference to the enemies gun
    private StateController  _controller;    // Reference to enemies state controller

    // Serialized fields - manually assigned in inspector
    [SerializeField] private MeleeAttack _leftHand, _rightHand, _rightFoot; // Reference to melee attack scripts



    // ----------------------------------------------------------------------------------------------
    // METHOD:      Awake()
    // Desciption:  Cahce component references, and convert the DeadZone float from degrees to radians
    // -----------------------------------------------------------------------------------------------
    private void Awake()
    {
        _navAgent   = GetComponent<NavMeshAgent>();
        _anim       = GetComponent<Animator>();
        _enemyGun   = GetComponentInChildren<EnemyGun>(true);
        _controller = GetComponent<StateController>();

        _navAgent.updateRotation = false; // We will manually update the rotation through scripting in most cases

        DeadZone *= Mathf.Deg2Rad;
    }


    // ---------------------------------------------------------------------------------------------------
    // METHOD:      Start()
    // Desciption:  Get a reference to the TagsHashIDs.cs file, and construct a new AnimatorSetup.cs class
    // ---------------------------------------------------------------------------------------------------
    private void Start()
    {
        _hashIDs = _controller.TagsHash;
        _animatorSetup = new AnimatorSetup(_anim, _hashIDs);
    }

    // ---------------------------------------------------------------------------------------------------------------------------------
    // METHOD:       Update()
    // Description:  Detect if the agent is in an attack state, and if they are, smoothly rotate their transform to face the player
    //               using Quaternion.Slerp().
    //               If an agent is not in an attack state they are running toward the player, so Slerp their rotation in a similar fashion,
    //               but toward their own LocalDesiredVelocity vector instead of the player's transform position.
    //               Passes a boolean to the animator controller if the agent is on an off mesh link, to play an animation
    //               Finally calls the NavAnimSetup() method.
    // ----------------------------------------------------------------------------------------------------------------------------------
    private void Update()
    {
        if ((_controller.InMeleeRange
            || _controller.IsRangedAttacking
            || _controller.IsFlameAttacking)) // Any of these returning true means we should manually rotate the agent to face the player
        {
            Quaternion enemyRotation = transform.rotation;                                         // store the Quaternion value
            Vector3 directionVector  = _controller.Player.transform.position - transform.position; // Get the direction to the player
            Quaternion lookRotation  = Quaternion.LookRotation(directionVector, transform.up);     // Rotation to face player
            Vector3 DesiredEuler     = new Vector3(transform.rotation.eulerAngles.x,
                                                   lookRotation.eulerAngles.y, 
                                                   transform.rotation.eulerAngles.z);              // Create a Euler to store the desired rotation
            lookRotation.eulerAngles = DesiredEuler;                                               // Assign the desired euler to the LookRotation
            transform.rotation = Quaternion.Slerp(transform.rotation, 
                                                  lookRotation, 
                                                  10f * Time.deltaTime);                           // Spherically interpolate current roation to the desired rotation       
        }
        else // We are not in an attack state and should not rotate to face the player, but rather rotate to face our next point on NavMesh
        {
            // Get the direction the agent wishes to face in local space:
            Vector3 localDesiredVelocity = transform.InverseTransformDirection(_navAgent.desiredVelocity);

            // Use mathf.atan2 to calculate the radians the agent wishes to turn, and convert to degrees
            // Store this as local variable angle:
            float angle = Mathf.Atan2(localDesiredVelocity.x, localDesiredVelocity.z) * Mathf.Rad2Deg;
            
            // If the angle we wish to turn is less than 120 degrees, and we are in a running animation, then slerp the rotation:
            if (Mathf.Abs(angle) < 120.0f && _navAgent.velocity.magnitude > 1f)
            {
                Quaternion lookRotation = Quaternion.LookRotation(_navAgent.desiredVelocity, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 5.0f * Time.deltaTime);
            }

            // If the navagent is on an offmesh link we want a particular animation state to play:
            if (_navAgent.isOnOffMeshLink)
            {
                _anim.SetBool(_hashIDs.IsJumping, true);
            }
            else if (!_navAgent.isOnOffMeshLink)
            {
                _anim.SetBool(_hashIDs.IsJumping, false);
            }

        }
        // Call the NavAnimSetup function. This helps to smoothly rotate the agents as they run
        NavAnimSetup();
    }


    // -----------------------------------------------------------------------------------
    // METHOD:       OnAnimatorMove()
    // Description:  Called after update every frame. Used to affect the root motion manually
    // -----------------------------------------------------------------------------------
    private void OnAnimatorMove()
    {
        // We check the timeSclae first becuse this method was causing a fatal error when un-pausing the game
        if (Time.timeScale < 1.0f)
        {
            return;
        }
        else if (Time.deltaTime != 0)
        {
            _navAgent.velocity = _anim.deltaPosition / Time.deltaTime; // Smoothly update the velocity
            transform.rotation =  _anim.rootRotation; // Manually update the root rotation
        }
    }

    // ---------------------------------------------------------------------------------
    // METHOD:       NavAnimSetup()
    // Description:  Here we create the speed and angle values which will be passed to the 
    //               agent's animator controller in the _animatorSetup.Setup() method.
    //               If the angle the agent wishes to turn is less than our pre-defined 
    //               DeadZone, then simply set it to zero
    // ---------------------------------------------------------------------------------
    private void NavAnimSetup()
    {
        float speed;
        float angle;

            speed = Vector3.Project(_navAgent.desiredVelocity, transform.forward).magnitude; // Project the desired velocity onto the actual forward vector and get the magnitude
            angle = FindAngle(transform.forward, _navAgent.desiredVelocity, transform.up); // Calculate the angle

            if (Mathf.Abs(angle) < DeadZone)
            {
                transform.LookAt(transform.position + _navAgent.desiredVelocity);
                angle = 0f;
            }

        _animatorSetup.Setup(speed, angle);
    }

    // ---------------------------------------------------------------------------------
    // METHOD:      FindAngle
    // Desciption:  Gets the angle in radians that the agent wishes to turn
    // ---------------------------------------------------------------------------------
    private float FindAngle(Vector3 fromVector, Vector3 toVector, Vector3 upVector)
    {
        if (toVector == Vector3.zero)
            return 0f;
        // FInd the absolute value of the angle:
        float angle = Vector3.Angle(fromVector, toVector);
        Vector3 normal = Vector3.Cross(fromVector, toVector); // Get the normal
        angle *= Mathf.Sign(Vector3.Dot(normal, upVector)); // get the dot product of normal and up vector, sign it, and multiply by angle
        angle *= Mathf.Deg2Rad; // Get angle in radians

        return angle;
    }

    // --------------------------------------------------------------------------------------------
    // METHOD:      ToggleAttackCollider, Animation Event
    // DESCRIPTION: Toggles trigger collider on or off, so enemies can attack, called from attack 
    //              animation as an animation event, with left or right hand passed in as parameter
    //              Passing 2 means the enemy is trying to kick the player
    // --------------------------------------------------------------------------------------------
    public void ToggleAttackColliderOn(int hand)
    {
        if (hand == -1)
            _leftHand.ToggleAttackColliderOn();
        else if (hand == 1)
            _rightHand.ToggleAttackColliderOn();
        else if (hand == 2)
            _rightFoot.ToggleAttackColliderOn();
    }

    //Toggle attack collider off
    public void ToggleAttackColliderOff(int hand)
    {
        if (hand == -1)
            _leftHand.ToggleAttackColliderOff();
        else if (hand == 1)
            _rightHand.ToggleAttackColliderOff();
        else if (hand == 2)
            _rightFoot.ToggleAttackColliderOff();
    }

    // --------------------------------------------------------------------------------------------
    // METHOD:      ToggleFlameAttackOn, Animation Event
    // DESCRIPTION: Toggles the flame attack object on, enabling both the particle effects and 
    //              trigger box collider that damages the player
    // --------------------------------------------------------------------------------------------
    public void ToggleFlameAttackOn(int hand)
    {
            if (hand == -1)
                _leftHandFlamer.gameObject.SetActive(true);
            else if (hand == 1)
                _rightHandFlamer.gameObject.SetActive(true);
            else if (hand == 2)
            {
                _leftHandFlamer.gameObject.SetActive(true);
                _rightHandFlamer.gameObject.SetActive(true);
            }
    }
    // I inlude a method to toggle the flame attack off, because unity doesn't allow to pass boolean parameters with animation events
    public void ToggleFlameAttackOff(int hand)
    {
        if (hand == -1)
            _leftHandFlamer.gameObject.SetActive(false);
        else if (hand == 1)
            _rightHandFlamer.gameObject.SetActive(false);
        else if (hand == 2)
        {
            _leftHandFlamer.gameObject.SetActive(false);
            _rightHandFlamer.gameObject.SetActive(false);
        }
    }

    // --------------------------------------------------------------------------------------------
    // METHOD:      FireGun(), Animation Event
    // DESCRIPTION: Fires a spherecast 
    // --------------------------------------------------------------------------------------------
    void FireGun()
    {
        _enemyGun.FireGun();
    }

    // Make sure a projectile is loaded before firing, always get the last object in the pool:
    /*
    void ChargeGun()
    {
        _enemyGun.ChargeGun();
    }
    */

    // --------------------------------------------------------
    // Method: ToggleTrailRenderer, Animation Event
    // Desc:   Speaks to the MeleeAttack.cs script on an enemies
    //         hand or foot, and toggles the trail renderer
    // --------------------------------------------------------
    private void ToggleTrailRendererOn(int hand)
    {

             if (hand == -1)
             {
                _leftHand.ToggleTrailRendererOn();
             }
             else if (hand == 1)
             {
                _rightHand.ToggleTrailRendererOn();
             }
             else if (hand == 2)
             {
                _rightFoot.ToggleTrailRendererOn();
             }
        
    }

    // I was using one method with a boolean switch to handle enabling/disabling the 
    // TrailRenderer component, but you cannot pass a boolean parameter when using an animation event method
    private void ToggleTrailRendererOff(int hand)
    {
        if (hand == -1)
        {
            _leftHand.ToggleTrailRendererOff();
        }
        else if (hand == 1)
        {
            _rightHand.ToggleTrailRendererOff();
        }
        else if (hand == 2)
        {
            _rightFoot.ToggleTrailRendererOff();
        }
    }
}
