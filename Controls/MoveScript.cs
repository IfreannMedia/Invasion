using System.Collections;
using UnityEngine;
//---------------------------------------------------------------------------------------------------
// CLASS:       MoveScript.cs
// DESCRIPTION: Responsible for moving the player's capsule around the level geometry using WASD keys
//              Uses the unity CharacterController component, which is useful for going up steps and
//              Eliminating wall-sliding
//---------------------------------------------------------------------------------------------------

public class MoveScript : MonoBehaviour {

    // Public fields:
    [HideInInspector] public bool grounded;

    private CharacterController playerController;
    private Animator animator;
    // Visible in Inspector
    [Header("Controller Settings")]
    [SerializeField] private float jumpVelocity;
    [SerializeField] private float speed;
    [Range(0.0f,5.0f)] private float sprintMeter = 5.0f; // Max amount of possible sprint, equivelint to time ie 5.0 seconds
    [HideInInspector] public bool isSprinting = false;
    [SerializeField] private float gravity = -9.7f;
    
    private Animator playerAnim;
    private PlayerLifecycle _lifeCycleScript;


    //-------------------------------
    // Method:      Start()
    // Description: Cache components
    //-------------------------------
    void Start()
    {
        playerController = GetComponent<CharacterController>();
        playerAnim = GetComponent<Animator>();
    }

    //----------------------------------------------------------------------------------------------------------------------------------
    // Method:      Update()
    // Description: listens for user input. Allows user to move the character by implementing the CharacterController.Move()
    //              method. This avoid slippery sliding on the floor and avoids the character getting snagged or stuck on env objects
    //              which was a common issue with using a rigidbody controller
    //----------------------------------------------------------------------------------------------------------------------------------
    private void Update()
    {

        if (Input.GetButtonDown("Sprint") && sprintMeter > 0.0f) // Left Shift is sprint
        {
            speed = 9;
            isSprinting = true;
            StartCoroutine(Sprint());
        }
        else if (Input.GetButtonUp("Sprint") || sprintMeter <= 0.0f) // if we release sprint or run out of sprint meter disable it
        {
            speed = 6.35f;       // 6.45f is standard speed
            isSprinting = false; // the MouseLook.cs script likes to read this boolean value
        }

        Vector3 playerMovement = new Vector3(Input.GetAxis("Horizontal") * speed, 0.0f, Input.GetAxis("Vertical") * speed);
        playerMovement = Vector3.ClampMagnitude(playerMovement, speed); // Use clamp magnitute to make sure the speed has a hard maximum limit
        playerMovement.y = gravity;
        playerMovement *= Time.deltaTime;
        playerMovement = transform.TransformDirection(playerMovement);
        playerController.Move(playerMovement);

        if (playerController.isGrounded)
        {
            grounded = true;
            playerAnim.SetFloat("VelocityX", playerController.velocity.magnitude); // this param is used for the 1D locomotion blend tree
        }
        else
        {
            grounded = false;
        }

    }
    // ---------------------------------------------------------------------------------
    // Method: Sprint()
    // Desc:   Depletes the sprintmeter by 1 unit every second. Increases sprint meter
    //         by a half unit every second the player is not sprinting
    //         I chose not to display a sprint timer as few modern FPS games do this
    // ---------------------------------------------------------------------------------
    IEnumerator Sprint()
    {
        while (isSprinting)
        {
            sprintMeter -= 1.0f;
            yield return new WaitForSeconds(1.0f);
        }
        while (!isSprinting && sprintMeter < 5.0f)
        {
            sprintMeter += 0.25f;
            yield return new WaitForSeconds(1.0f);
        }
        yield return null;
    }
}

