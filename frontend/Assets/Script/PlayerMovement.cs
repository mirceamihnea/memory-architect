using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private PlayerControls controls;
    private CharacterController controller;
    private Vector2 moveInput;
    private Vector3 velocity;

    public float speed = 3f;
public float runMultiplier = 1.3f;
public float gravity = -20f;
public float jumpHeight = 1.2f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private bool isGrounded;
    private bool jumpPressed;
    private bool isRunning;

    private void Awake()
    {
        InitializeControls();
    }

    private void InitializeControls()
    {
        if (controls != null) return;

        controls = new PlayerControls();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += _ => moveInput = Vector2.zero;

        controls.Player.Jump.performed += _ => jumpPressed = true;

        controls.Player.Run.started += _ => isRunning = true;
        controls.Player.Run.canceled += _ => isRunning = false;
    }

    private void OnEnable()
    {
        InitializeControls();
        controls.Enable();
    }

    private void OnDisable()
    {
        if (controls != null) controls.Disable();
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        GroundCheck();
        HandleMovement();
        ApplyGravity();
        HandleJump();
    }

    private void GroundCheck()
    {
        if (groundCheck != null)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        }
        else
        {
            Debug.LogWarning("GroundCheck Transform is not assigned.");
        }
    }

    private void HandleMovement()
    {
        float currentSpeed = isRunning ? speed * runMultiplier : speed;
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * currentSpeed * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        if (isGrounded && velocity.y < 0)
{
    velocity.y = -8f;
}

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleJump()
    {
        if (isGrounded && jumpPressed)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        jumpPressed = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}
