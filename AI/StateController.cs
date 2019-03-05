using UnityEngine;
using UnityEngine.AI;

// ----------------------------------------------------------------------------------------------------------------------------------------------
// CLASS:       StateController
// DESCRIPTION: This class is attached to an enemy GameObject in the unity scene. It allows the enemy to 
//              operate in a state machine and switch states, implementing scriptable objects in order to create states, actions, decisions,
//              and transitions. It also uses OnTriggerEnter methods to detect if it's in range for an attack
// ----------------------------------------------------------------------------------------------------------------------------------------------
public class StateController : MonoBehaviour
{
    // public variables
    [Header("State Machine Fields")]
    public State            CurrentState;                   // Our current state
    public State            RemainState;                    // State we will remain in, set to the RemainInState in the inspector

    [Header("Enemy Data")]
    public SphereCollider   AISensor;                       // The sphere trigger collider enemies use to "See", using unity trigger collisions and raycasting
    public float            AttackAngleThreshold = 20.0f;   // The minimum angle allowed between an enemies forward vector and the direction to the player before entering into an attack animation
    public float            ProjectileAttackRange = 12.5f;  // The minimum range from which armoured enemies shoot, randomized in Awake()

    [Header("Publicly Referenced Objects")]
    public TagsHashIDs      TagsHash;                       // Assigned in inspector to avoid using FindObjectOfType(); - especially as enemies are currently instantiated, not pooled
    
    // public component references which other scripts will need access to - hidden from inspector to reduce clutter
    [HideInInspector] public MoveScript     Player;                         
    [HideInInspector] public NavMeshAgent   NavAgent;
    [HideInInspector] public MeleeAttack    MeleeScript;      // Script used for performing melee attacks
    [HideInInspector] public float          stateTimeElapsed; // Float value used for timer
    [HideInInspector] public Animator       AnimatorController;
    [HideInInspector] public EnemySight     EnemySightScript; // script used to detect line of sight to player

    // Booleans for detecting if we are in an attack animation, melee or ranged, or flame
    // These are used by state decision scripts and also stateMachineBehaviour scripts in the animator controller
    [HideInInspector] public bool IsMeleeAttacking = false, IsRangedAttacking = false, IsFlameAttacking = false;

    // Bools used for detecting combat range:
    [HideInInspector] public bool InMeleeRange          = false;
    [HideInInspector] public bool InProjectileRange     = false;
    [HideInInspector] public bool InFlamethrowerRange   = false;

    
    [HideInInspector] public bool _aiActive = true; // We will not update a state if the ai is not active

    // -------------------------------------------------------------------
    // METHOD:      Awake()
    // DESCRIPTION: Caches some global variables, and sets the armoured enemies'
    //              default attack range - I vary this with each instance of the 
    //              armoured enemy to give them some varience
    // -------------------------------------------------------------------
    protected void Awake()
    {
        AnimatorController = GetComponent<Animator>();
        NavAgent = GetComponent<NavMeshAgent>();
        EnemySightScript = GetComponentInChildren<EnemySight>();
        Player = Camera.main.GetComponentInParent<MoveScript>(); // Instead of a manual assign, or using FindObjectOfType, go through the main camera to get the player ref
        ProjectileAttackRange = Random.Range(5.0f, 15.0f);
    }

    // -------------------------------------------------------------------
    // METHOD:      Update()
    // DESCRIPTION: Calls UpdateState() and passes itself in as the StateController
    //              parameter. Each state will have a list of actions and decisions which 
    //              get updated every frame
    // -------------------------------------------------------------------
    protected void Update()
    {
        if (!_aiActive)
            return;
        // Here is where the chain of command begins, passing in our own StateControllerInstance for the next classes
        CurrentState.UpdateState(this);
    }

    
    // ------------------------------------------------------------------------------------------------
    // METHOD:      TransitionToState()
    // DESCRIPTION: switches to a new state (nextState). Only does this if the next state
    //              is not marked as the remain state, because then we want to ramin in the same state
    // ------------------------------------------------------------------------------------------------
    public void TransitionToState(State nextState)
    {
        if (nextState != RemainState)
        {
          
            CurrentState = nextState;
        }
    }

    // ------------------------------------------------------------------------------------------------
    // METHOD:      CheckIfCountdownElapsed(float duration)
    // DESCRIPTION: checks if the duration has counted down, returning a boolean
    // ------------------------------------------------------------------------------------------------
    public bool CheckIfCountdownElapsed(float duration)
    {
        stateTimeElapsed += Time.deltaTime;
        if (stateTimeElapsed >= duration)
        {
            stateTimeElapsed = 0.0f;
            return true;
        }
        else
        { 
            return false;
        }
    }

    // ------------------------------------------------------------------------------------------------
    // METHOD:      OnExitState()
    // DESCRIPTION: resets the stateTimeElapsed variable just in case
    // ------------------------------------------------------------------------------------------------
    protected void OnExitState()
    {
        stateTimeElapsed = 0.0f;

    }

    // ------------------------------------------------------------------------------------------------
    // METHOD:      OnTriggerEnter(), virtual
    // DESCRIPTION: If we enter a trigger with the melee range trigger, we are then in melee range and set
    //              the bol accordingly, same goes for flamethrower range and projectile range
    //              Virtual because the StateControllerRnged.cs file callls this method a little dfferently
    // ------------------------------------------------------------------------------------------------
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(TagsHashIDs.MeleeRange))
        {
            
            InMeleeRange = true;
            InProjectileRange = false;
            InFlamethrowerRange = false; // switch to false so we can default to a melee attack if the player gets too close
        }
    }

    // ------------------------------------------------------------------------------------------------
    // METHOD:      OnTriggerExit(Collider other),virtual
    // DESCRIPTION: enables projectile and flame based attacks - also handled slightly differently in 
    //              StateControllerRanged.cs
    // ------------------------------------------------------------------------------------------------
    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(TagsHashIDs.MeleeRange))
        {
            InMeleeRange = false;
            InProjectileRange = true;
            InFlamethrowerRange = true;
        }

    }
}
