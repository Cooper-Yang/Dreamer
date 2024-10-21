using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class ButterflyController : MonoBehaviour
{
    public float flightSpeed = 10f;
    public float strafeSpeed = 5f;
    public float ascentSpeed = 5f;
    public float turnSpeed = 60f;
    public float tiltAngle = 15f;
    public float fallSpeed = 5f; // Constant fall speed
    public CinemachineFreeLook freeLookCamera;

    private Rigidbody rb;
    private Vector2 movementInput;
    private Vector2 lookInput;

    private InputAction movementAction;
    private InputAction ascendAction;
    private InputAction descendAction;
    private InputAction lookAction;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        var inputActions = new PlayerInputActions();
        inputActions.Enable();

        movementAction = inputActions.Player.Movement;
        ascendAction = inputActions.Player.Ascend;
        descendAction = inputActions.Player.Descend;
        lookAction = inputActions.Player.Look;

        if (freeLookCamera == null)
        {
            Debug.LogError("Cinemachine FreeLook camera is not assigned.");
        }

        // Disable the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMovement();
        HandleRotation();
    }

    void HandleMovement()
    {
        // Check if the player is grounded
        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);

        // Get movement input (forward/backward and strafing)
        movementInput = movementAction.ReadValue<Vector2>();

        // Move forward/backward and strafe left/right only if not grounded
        if (!isGrounded)
        {
            Vector3 forwardMovement = transform.forward * movementInput.y * flightSpeed * Time.deltaTime;
            Vector3 strafeMovement = transform.right * movementInput.x * strafeSpeed * Time.deltaTime;

            // Apply movement
            rb.MovePosition(rb.position + forwardMovement + strafeMovement);
        }

        // Ascend or Descend
        if (ascendAction.IsPressed())
        {
            rb.velocity = new Vector3(rb.velocity.x, ascentSpeed, rb.velocity.z);
        }
        else if (descendAction.IsPressed())
        {
            rb.velocity = new Vector3(rb.velocity.x, -ascentSpeed, rb.velocity.z);
        }
        else if (isGrounded)
        {
            // Apply constant fall speed when grounded and not ascending
            rb.velocity = new Vector3(rb.velocity.x, -fallSpeed, rb.velocity.z);
        }
    }

    void HandleRotation()
    {
        // Check if the player is grounded
        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);

        // Get rotation input from the mouse or right stick
        lookInput = lookAction.ReadValue<Vector2>();

        // Yaw rotation (left/right)
        float yaw = lookInput.x * turnSpeed * Time.deltaTime;
        Quaternion yawRotation = Quaternion.Euler(0, yaw, 0);
        rb.MoveRotation(rb.rotation * yawRotation);

        // Tilt the butterfly when turning only if not grounded
        if (!isGrounded)
        {
            float tilt = movementInput.x * tiltAngle;
            Quaternion tiltRotation = Quaternion.Euler(0, rb.rotation.eulerAngles.y, -tilt);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, tiltRotation, Time.deltaTime * 5f)); // Adjust the smoothing factor as needed
        }

        // Update Cinemachine FreeLook camera
        if (freeLookCamera != null)
        {
            freeLookCamera.m_XAxis.Value += lookInput.x * turnSpeed * Time.deltaTime;
            freeLookCamera.m_YAxis.Value += lookInput.y * turnSpeed * Time.deltaTime * 0.1f; // Adjust pitch sensitivity
        }
    }
}