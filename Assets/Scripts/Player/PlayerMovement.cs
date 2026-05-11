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
    public bool IsDashing => isDashing;
    private bool dashPressedThisFrame;
    private float originalGravity;

    private PlayerGravity2D playerGravity;

    private InputSystem_Actions controls;
    private bool controlsLocked;
    public bool ControlsLocked => controlsLocked;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        originalGravity = rb.gravityScale;
        playerGravity = GetComponent<PlayerGravity2D>();

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

        if (!GameStateManager.Instance.IsPlaying() || controlsLocked)
            return;

        HandleDashInput();
    }

    private void FixedUpdate()
    {
        if (!GameStateManager.Instance.IsPlaying() || controlsLocked)
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
        Vector2 gravityDir = GetGravityDirection();

        Vector2 moveDir;

        if (gravityDir == Vector2.down)
        {
            moveDir = Vector2.right * moveVector.x;
        }
        else if (gravityDir == Vector2.up)
        {
            moveDir = Vector2.right * moveVector.x;
        }
        else if (gravityDir == Vector2.right)
        {
            moveDir = Vector2.up * moveVector.y;
        }
        else
        {
            moveDir = Vector2.up * moveVector.y;
        }

        // current velocity along gravity axis
        Vector2 gravityVelocity =
            gravityDir * Vector2.Dot(rb.linearVelocity, gravityDir);

        // current sideways movement
        Vector2 sidewaysVelocity =
            moveDir * Vector2.Dot(rb.linearVelocity, moveDir.normalized);

        // target movement
        Vector2 targetMoveVelocity = moveDir * moveSpeed;

        // smoothly move toward target instead of replacing velocity
        sidewaysVelocity = Vector2.Lerp(
            sidewaysVelocity,
            targetMoveVelocity,
            12f * Time.fixedDeltaTime
        );

        rb.linearVelocity = gravityVelocity + sidewaysVelocity;
    }

    // ----------------------------
    // GROUND CHECK + TIMERS
    // ----------------------------
    private void HandleGrounding()
    {
        wasGrounded = isGrounded;

        Vector2 gravityDir = GetGravityDirection();

        if (groundCheck != null)
        {
            // Move ground check in WORLD direction of gravity
            groundCheck.position = (Vector2)transform.position + gravityDir * 0.55f;

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

    public void LockControls(float duration)
    {
        StartCoroutine(LockControlsRoutine(duration));
    }

    private IEnumerator LockControlsRoutine(float duration)
    {
        controlsLocked = true;

        yield return new WaitForSeconds(duration);

        controlsLocked = false;
    }

    // ----------------------------
    // JUMP
    // ----------------------------
    private void HandleJumpPhysics()
    {
        if (jumpBufferCounter <= 0f)
            return;

        if (!isGrounded && coyoteCounter <= 0f)
            return;


        Vector2 gravityDir = GetGravityDirection();
        Vector2 jumpDir = -gravityDir;

        float speedAlongJump = Vector2.Dot(rb.linearVelocity, jumpDir);

        if (speedAlongJump > 0f)
            rb.linearVelocity -= jumpDir * speedAlongJump;

        rb.linearVelocity += jumpDir * jumpForce;

        jumpBufferCounter = 0f;
        coyoteCounter = 0f;

        if (anim != null)
            anim.Play("Player_JumpSquash", 0, 0f);
    }

    private void HandleJumpTimers()
    {
        if (jumpPressedThisFrame)
            jumpBufferCounter = jumpBufferTime;
        else
            jumpBufferCounter -= Time.deltaTime;

        jumpPressedThisFrame = false;
    }

    private void HandleVariableJumpCut()
    {
        Vector2 gravityDir = GetGravityDirection();
        Vector2 jumpDir = -gravityDir;

        float jumpSpeed = Vector2.Dot(rb.linearVelocity, jumpDir);

        if (!jumpHeld && jumpSpeed > 0f)
        {
            rb.linearVelocity -= jumpDir * jumpSpeed * (1f - jumpCutMultiplier);
        }
    }

    public void CancelHeldJump()
    {
        jumpHeld = false;
        jumpPressedThisFrame = false;
        jumpBufferCounter = 0f;
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

        // Stop jump from boosting dash
        jumpHeld = false;
        jumpPressedThisFrame = false;
        jumpBufferCounter = 0f;
        coyoteCounter = 0f;

        Vector2 gravityDir = GetGravityDirection();
        Vector2 jumpDir = -gravityDir;

        // Remove upward/jump velocity before dash
        float jumpVelocity = Vector2.Dot(rb.linearVelocity, jumpDir);

        if (jumpVelocity > 0f)
        {
            rb.linearVelocity -= jumpDir * jumpVelocity;
        }

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

    private Vector2 GetGravityDirection()
    {
        if (playerGravity != null)
            return playerGravity.GravityDirection;

        return Vector2.down;
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

    public void PushPlayer(Vector2 direction, float force, float duration)
    {
        StartCoroutine(PushRoutine(direction, force, duration));
    }

    private IEnumerator PushRoutine(Vector2 direction, float force, float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            rb.linearVelocity = direction.normalized * force;
            timer += Time.deltaTime;
            yield return null;
        }
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