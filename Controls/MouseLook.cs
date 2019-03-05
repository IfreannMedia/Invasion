using UnityEngine;
using UnityEngine.UI;

// ----------------------------------------------------------------------------------------------------------------
// CLASS:       MouseLook..cs
// DESCRIPTION: This cass gets user input from mouse and joystick controller (only tested with PS4 dualshock models)
//              And tranlsates that input to the camera rotation on it's X-axis and the Player's character controller 
//              rotation on it's Y-axis
//------------------------------------------------------------------------------------------------------------------
public class MouseLook : MonoBehaviour {

    // Public Fields
    [Header("Look Settings")]
    public float lookSensitivity = 3f;
    public float clampValue = 80.0f;
    public bool inverted = false;
    public Slider AimSlider;

    // Private Fields
    private CharacterController playerCharController;
    private float _rotationX = 0.0f; // the rotation of the camera on the X-axis
    private MoveScript _moveScript; // Used to detect if we are currently sprinting
    private string _mouseX = "Mouse X", _joyX = "Joy X"; // Cached names for X axis input, depending on whether using mouse or controller
    private string _mouseY = "Mouse Y", _joyY = "Joy Y"; // cahced names for Y axis input

    // ----------------------------------------------------
    // Method:      Awake()
    // Description: Lock the cursor, cache global variables
    //-----------------------------------------------------
    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerCharController = GetComponentInParent<CharacterController>();
        _moveScript = GetComponentInParent<MoveScript>();
    }
	
    // ----------------------------------------------------------------------------------------------------------------
    // Method:      Update()
    // Description: This method gets input from both the mouse and the anolog sticks of a joypad
    //              Joypad input is pre-configured to work with a dualshock PS4 controller - it has not been tested with 
    //              xBox controller or other varients
    //------------------------------------------------------------------------------------------------------------------
	void Update () {
        // We cannot control the player's cam rotation if the game is paused:
        if (Time.timeScale < 1.0f)
        {
            return;
        }

        // rotate the player along it's Y access to turn left and right
        // If we are sprinting, we cannot rotate as fast:
        if (_moveScript.isSprinting)
        {
            playerCharController.transform.Rotate(0.0f, Input.GetAxis(_mouseX) * (lookSensitivity * 0.50f), 0.0f);

            if (Input.GetAxis(_joyX) != 0)
            {
                playerCharController.transform.Rotate(0.0f, Input.GetAxis(_joyX) * (lookSensitivity * 0.50f), 0.0f);
            }
        }
        else
        {
            playerCharController.transform.Rotate(0.0f, Input.GetAxis(_mouseX) * lookSensitivity, 0.0f);
            if (Input.GetAxis(_joyX) != 0)
            {
                playerCharController.transform.Rotate(0.0f, Input.GetAxis(_joyX) * lookSensitivity, 0.0f);
            }

        }

        // Rotate the camera object along it's x access, depending on inversion:
        if (inverted)
        {
            _rotationX += Input.GetAxis(_mouseY) * lookSensitivity;
            _rotationX += Input.GetAxis(_joyY) * lookSensitivity; 
        }
        else
        {
            _rotationX -= Input.GetAxis(_mouseY) * lookSensitivity;
            _rotationX -= Input.GetAxis(_joyY) * lookSensitivity;
        }
         


        _rotationX = Mathf.Clamp(_rotationX, -clampValue, clampValue); // clamp the vertical look rotation

        float rotationY = transform.localEulerAngles.y; // grab the current y rotation which we have already set

        transform.localEulerAngles = new Vector3(_rotationX, rotationY, 0.0f); // assign the camera's rotation

    }

    // ----------------------------------------------------------------------------------------------------------------
    // Method:      UpdateLookSensitivity()
    // Description: This method is called from the Pause Game canvas Aim Slider when it's value is changed
    //              The slider value is clamped to be between 1 and 6, it is how users adjust aim sensitivity for both
    //              horizontal and vertical look axis
    //------------------------------------------------------------------------------------------------------------------
    public void UpdateLookSensitivity()
    {
        lookSensitivity = AimSlider.value;

    }

    //---------------------------------------------------------------------------
    // Method:      InvertCamera()
    // Description: Inverts the camera's vertical input using a ternery operator
    //              Called from pause game canvas button
    //---------------------------------------------------------------------------
    public void InvertCamera()
    {
        inverted = inverted == true ? false : true;
    }
}

