using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController2D : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;

    [Header("Jump")]
    public float jumpForce = 12f;
    [Range(0f, 1f)] public float jumpCutMultiplier = 0.5f;
    public float coyoteTime = 0.1f;
    public float jumpBufferTime = 0.1f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator anim;

    private float moveInput;
    private bool jumpHeld;
    private bool jumpPressedThisFrame;

    private bool isGrounded;
    private bool wasGrounded;
    private float coyoteCounter;
    private float jumpBufferCounter;

    private InputSystem_Actions controls;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        controls = new InputSystem_Actions();

        // INPUT BINDINGS
        controls.Player.Move.performed += ctx =>
        {
            moveInput = ctx.ReadValue<Vector2>().x;
        };

        controls.Player.Move.canceled += ctx =>
        {
            moveInput = 0f;
        };

        controls.Player.Jump.performed += ctx =>
        {
            jumpPressedThisFrame = true;
            jumpHeld = true;
        };

        controls.Player.Jump.canceled += ctx =>
        {
            jumpHeld = false;
        };
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Update()
    {
        // Always update ground + timers (important for feel consistency)
        HandleGrounding();
        HandleJumpTimers();

        // Block INPUT only, not physics logic
        if (!GameStateManager.Instance.IsPlaying())
            return;

        HandleMovementInput();
    }

    private void FixedUpdate()
    {
        if (!GameStateManager.Instance.IsPlaying())
            return;

        HandleMovementPhysics();
        HandleJumpPhysics();
        HandleVariableJumpCut();
    }

    // ----------------------------
    // INPUT
    // ----------------------------
    private void HandleMovementInput()
    {
        // nothing else needed here for now
    }

    // ----------------------------
    // MOVEMENT
    // ----------------------------
    private void HandleMovementPhysics()
    {
        Vector2 v = rb.linearVelocity;
        v.x = moveInput * moveSpeed;
        rb.linearVelocity = v;
    }

    // ----------------------------
    // GROUND CHECK + TIMERS
    // ----------------------------
    private void HandleGrounding()
    {
        wasGrounded = isGrounded;

        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(
                groundCheck.position,
                groundCheckRadius,
                groundLayer
            );
        }

        if (isGrounded)
            coyoteCounter = coyoteTime;
        else
            coyoteCounter -= Time.deltaTime;

        // landing animation
        if (!wasGrounded && isGrounded)
        {
            if (anim != null)
                anim.Play("Player_JumpSquash", 0, 0f);
        }
    }

    private void HandleJumpTimers()
    {
        if (jumpPressedThisFrame)
            jumpBufferCounter = jumpBufferTime;
        else
            jumpBufferCounter -= Time.deltaTime;

        jumpPressedThisFrame = false;
    }

    // ----------------------------
    // JUMP
    // ----------------------------
    private void HandleJumpPhysics()
    {
        if (jumpBufferCounter > 0f && coyoteCounter > 0f)
        {
            Vector2 v = rb.linearVelocity;
            v.y = jumpForce;
            rb.linearVelocity = v;

            jumpBufferCounter = 0f;
            coyoteCounter = 0f;

            if (anim != null)
                anim.Play("Player_JumpSquash", 0, 0f);
        }
    }

    private void HandleVariableJumpCut()
    {
        if (!jumpHeld && rb.linearVelocity.y > 0f)
        {
            Vector2 v = rb.linearVelocity;
            v.y *= jumpCutMultiplier;
            rb.linearVelocity = v;
        }
    }

    // ----------------------------
    // DEBUG
    // ----------------------------
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}