using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Cinemachine;
using System.Collections;

public class ButterflyController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float flightSpeed = 10f;
    public float strafeSpeed = 5f;
    public float ascentSpeed = 5f;
    public float turnSpeed = 60f;
    public float tiltAngle = 15f;
    [Header("Camera Settings")]
    public CinemachineFreeLook freeLookCamera;
    public float cameraLerpSpeed = 2f; // Speed at which the camera lerps to the forward direction
    public float flapCooldown = 1f; // Cooldown duration for the flap action
    [Header("Stamina Settings")]
    public Slider staminaSlider;
    public float maxStamina = 100f; // Maximum stamina
    public float airStaminaRegenRate = 10f; // Stamina regeneration rate per second
    public float groundStaminaRegenRate = 20f; // Stamina regeneration rate per second when on the ground
    public float staminaCostPerFlap = 20f; // Stamina cost per flap
    [Header("Rumble Settings")]
    public float rumbleIntensity = 0.5f; // Intensity of the gamepad rumble
    public float rumbleDuration = 0.2f; // Duration of the gamepad rumble

    private Rigidbody rb;
    private Vector2 movementInput;
    private Vector2 lookInput;
    private float lastFlapTime;
    private float currentStamina;
    private bool bgmStarted = false; // Flag to track if BGM has started
    private bool wasGrounded = true; // Track if the player was previously grounded

    private InputAction movementAction;
    private InputAction ascendAction;
    private InputAction descendAction;
    private InputAction lookAction;

    void Start()
    {
        if (staminaSlider != null)
        {
            staminaSlider.maxValue = maxStamina;
            staminaSlider.value = currentStamina;
        }

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

        // Initialize the last flap time and current stamina
        lastFlapTime = -flapCooldown;
        currentStamina = maxStamina;
    }

    void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleCamera();
        RegenerateStamina();
        UpdateStaminaUI();
        CheckLanding();
    }

    bool checkIfGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 0.4f);
    }

    void HandleMovement()
    {
        // Check if the player is grounded
        bool isGrounded = checkIfGrounded();

        if (!isGrounded)
        {
            // Get movement input (forward/backward and strafing)
            movementInput = movementAction.ReadValue<Vector2>();

            // Start playing BGM on first movement input
            if (!bgmStarted && movementInput != Vector2.zero)
            {
                StartBGM();
                bgmStarted = true;
            }

            // Move forward/backward and strafe left/right
            Vector3 forwardMovement = transform.forward * movementInput.y * flightSpeed * Time.deltaTime;
            Vector3 strafeMovement = transform.right * movementInput.x * strafeSpeed * Time.deltaTime;

            // Apply movement force
            rb.AddForce(forwardMovement + strafeMovement);
        }

        // Ascend or Descend
        if (ascendAction.WasPressedThisFrame() && Time.time >= lastFlapTime + flapCooldown && currentStamina >= staminaCostPerFlap)
        {
            // Apply an upward impulse force to imitate a "flap"
            rb.AddForce(Vector3.up * ascentSpeed, ForceMode.Impulse);
            lastFlapTime = Time.time; // Reset the cooldown timer
            currentStamina -= staminaCostPerFlap; // Decrease stamina
            AudioManager.Instance.PlayRandomSFX(0, 1, 0.9f, 1.1f);

            // Rumble the gamepad
            StartCoroutine(RumbleGamepad(rumbleIntensity, rumbleDuration));
        }
        else if (descendAction.IsPressed())
        {
            rb.velocity = new Vector3(rb.velocity.x, -ascentSpeed, rb.velocity.z);
        }
    }

    void HandleRotation()
    {
        // Check if the player is grounded
        bool isGrounded = checkIfGrounded();

        // Tilt the butterfly when turning only if not grounded
        if (!isGrounded)
        {
            // Yaw rotation (left/right)
            float yaw = lookInput.x * turnSpeed * Time.deltaTime;

            // Add strafing input to yaw rotation
            yaw += movementInput.x * turnSpeed * Time.deltaTime;

            Quaternion yawRotation = Quaternion.Euler(0, yaw, 0);
            rb.MoveRotation(rb.rotation * yawRotation);

            float tilt = movementInput.x * tiltAngle;
            Quaternion tiltRotation = Quaternion.Euler(0, rb.rotation.eulerAngles.y, -tilt);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, tiltRotation, Time.deltaTime * 5f)); // Adjust the smoothing factor as needed
        }
        else
        {
            // Reset the rotation to normal (no tilt) when grounded
            Quaternion normalRotation = Quaternion.Euler(0, rb.rotation.eulerAngles.y, 0);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, normalRotation, Time.deltaTime * 5f)); // Adjust the smoothing factor as needed
        }
    }

    void HandleCamera()
    {
        // Get mouse input
        lookInput = lookAction.ReadValue<Vector2>();

        // If there is mouse input, use it to control the camera
        if (lookInput != Vector2.zero || checkIfGrounded())
        {
            // Do nothing
        }
        // If the player is moving and there is no mouse input, lerp the camera to align with the direction of the player's rotation
        else if (movementInput != Vector2.zero)
        {
            // Calculate the target rotation based on the player's current rotation
            Vector3 forward = transform.forward;
            forward.y = 0; // Ignore the vertical component

            if (forward != Vector3.zero)
            {
                // Calculate the target rotation in world space
                Quaternion targetRotation = Quaternion.LookRotation(forward);

                // Extract the yaw angle from the target rotation
                float targetYaw = targetRotation.eulerAngles.y;

                // Lerp the camera's yaw to the target value
                freeLookCamera.m_XAxis.Value = Mathf.LerpAngle(freeLookCamera.m_XAxis.Value, targetYaw, Time.deltaTime * cameraLerpSpeed);
            }
        }
    }

    void RegenerateStamina()
    {
        bool isGrounded = checkIfGrounded();
        float regenRate = isGrounded ? groundStaminaRegenRate : airStaminaRegenRate;

        if (currentStamina < maxStamina)
        {
            currentStamina += regenRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        }
    }

    void UpdateStaminaUI()
    {
        if (staminaSlider != null)
        {
            staminaSlider.value = currentStamina;
            if (currentStamina < staminaCostPerFlap)
            {
                staminaSlider.fillRect.GetComponent<Image>().color = Color.red;
            }
            else
            {
                staminaSlider.fillRect.GetComponent<Image>().color = new Color(0.5137255f, 0.9453539f, 1);
            }
        }
    }

    void StartBGM()
    {
        // Play the first BGM clip in a loop
        AudioManager.Instance.PlayAudio(AudioType.BGM, 0, true);
    }

    void CheckLanding()
    {
        bool isGrounded = checkIfGrounded();

        // Check if the player has just landed
        if (isGrounded && !wasGrounded)
        {
            // Rumble the gamepad on landing
            StartCoroutine(RumbleGamepad(rumbleIntensity, rumbleDuration));
        }

        // Update the wasGrounded flag
        wasGrounded = isGrounded;
    }

    IEnumerator RumbleGamepad(float intensity, float duration)
    {
        if (Gamepad.current != null)
        {
            Gamepad.current.SetMotorSpeeds(intensity, intensity);
            yield return new WaitForSeconds(duration);
            Gamepad.current.SetMotorSpeeds(0, 0);
        }
    }
}