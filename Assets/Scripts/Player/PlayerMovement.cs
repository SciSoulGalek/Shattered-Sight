using System.Collections;
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

    [Header("Dash")]
    public float dashSpeed = 18f;
    public float dashDuration = 0.15f;
    public float dashEndSpeedMultiplier = 0.5f;

    [Header("Dash Indicator")]
    public SpriteRenderer dashIndicatorRenderer;
    public Color dashReadyColor = Color.white;
    public Color dashUsedColor = new Color(1f, 1f, 1f, 0.15f);

    [Header("Dash Afterimages")]
    public DashAfterimage afterimagePrefab;
    public SpriteRenderer cubeSpriteRenderer;
    public float afterimageSpacing = 0.04f;
    public Color afterimageColor = new Color(1f, 1f, 1f, 0.45f);

    private float afterimageTimer;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;

    private MovingPlatform2D currentPlatform;

    private Rigidbody2D rb;
    private Animator anim;

    private Vector2 moveVector;
    private float moveInput;

    private bool jumpHeld;
    private bool jumpPressedThisFrame;

    private bool isGrounded;
    private bool wasGrounded;
    private float coyoteCounter;
    private float jumpBufferCounter;

    private bool canDash = true;
    private bool isDashing;
    private bool dashPressedThisFrame;
    private float originalGravity;

    private InputSystem_Actions controls;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        originalGravity = rb.gravityScale;

        controls = new InputSystem_Actions();

        controls.Player.Move.performed += ctx =>
        {
            moveVector = ctx.ReadValue<Vector2>();
            moveInput = moveVector.x;
        };

        controls.Player.Move.canceled += ctx =>
        {
            moveVector = Vector2.zero;
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

        controls.Player.Dash.performed += ctx =>
        {
            dashPressedThisFrame = true;
        };
    }

    private void Start()
    {   
        UpdateDashIndicator();
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
        HandleGrounding();
        HandleJumpTimers();
        UpdateDashIndicator();

        if (!GameStateManager.Instance.IsPlaying())
            return;

        HandleDashInput();
    }

    private void FixedUpdate()
    {
        if (!GameStateManager.Instance.IsPlaying())
            return;

        if (currentPlatform != null && isGrounded)
        {
            rb.position += (Vector2)currentPlatform.DeltaMovement;
        }

        if (isDashing)
            return;

        HandleMovementPhysics();
        HandleJumpPhysics();
        HandleVariableJumpCut();
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
        {
            coyoteCounter = coyoteTime;

            if (!isDashing)
                canDash = true;
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
        }

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
    // DASH
    // ----------------------------
    private void HandleDashInput()
    {
        if (!dashPressedThisFrame)
            return;

        dashPressedThisFrame = false;

        if (!canDash || isDashing)
            return;

        Vector2 dashDirection = GetDashDirection();
        StartCoroutine(Dash(dashDirection));
    }

    private Vector2 GetDashDirection()
    {
        Vector2 direction = moveVector.normalized;

        // If no direction is held, dash toward facing direction
        if (direction == Vector2.zero)
        {
            direction = transform.localScale.x >= 0f
                ? Vector2.right
                : Vector2.left;
        }

        return direction;
    }

    private IEnumerator Dash(Vector2 direction)
    {
        canDash = false;
        isDashing = true;
        UpdateDashIndicator();

        rb.gravityScale = 0f;
        rb.linearVelocity = direction * dashSpeed;

        afterimageTimer = 0f;

        while (afterimageTimer < dashDuration)
        {
            SpawnAfterimage();

            yield return new WaitForSeconds(afterimageSpacing);
            afterimageTimer += afterimageSpacing;
        }

        rb.gravityScale = originalGravity;
        rb.linearVelocity *= dashEndSpeedMultiplier;

        isDashing = false;
        UpdateDashIndicator();
    }

    private void UpdateDashIndicator()
    {
        if (dashIndicatorRenderer == null)
            return;

        dashIndicatorRenderer.color = canDash ? dashReadyColor : dashUsedColor;
    }

    private void SpawnAfterimage()
    {
        if (afterimagePrefab == null || cubeSpriteRenderer == null)
            return;
    
        DashAfterimage img = Instantiate(
            afterimagePrefab,
            cubeSpriteRenderer.transform.position,
            cubeSpriteRenderer.transform.rotation
        );
    
        img.Setup(
            cubeSpriteRenderer.sprite,
            cubeSpriteRenderer.transform.lossyScale,
            afterimageColor,
            cubeSpriteRenderer.sortingOrder - 1
        );
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        MovingPlatform2D platform = collision.collider.GetComponent<MovingPlatform2D>();

        if (platform != null && IsStandingOnTop(collision))
        {
            currentPlatform = platform;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        MovingPlatform2D platform = collision.collider.GetComponent<MovingPlatform2D>();
    
        if (platform == null)
            return;
    
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                currentPlatform = platform;
                return;
            }
        }
    }
    
    private void OnCollisionExit2D(Collision2D collision)
    {
        MovingPlatform2D platform = collision.collider.GetComponent<MovingPlatform2D>();
    
        if (platform != null && platform == currentPlatform)
            currentPlatform = null;
    }
    
    private bool IsStandingOnTop(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
                return true;
        }

        return false;
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