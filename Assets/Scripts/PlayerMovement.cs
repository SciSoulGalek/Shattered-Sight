using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;

    [Header("Jump")]
    public float jumpForce = 12f;              
    [Range(0f, 1f)]
    public float jumpCutMultiplier = 0.5f;    
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

    private bool isGrounded;
    private bool wasGrounded;
    private float coyoteCounter;
    private float jumpBufferCounter;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        rb.freezeRotation = true;
        wasGrounded = false;
    }

    private void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        bool jumpPressedThisFrame = Input.GetButtonDown("Jump");
        jumpHeld = Input.GetButton("Jump");

        wasGrounded = isGrounded;

        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(
                groundCheck.position,
                groundCheckRadius,
                groundLayer
            );
        }
        else
        {
            isGrounded = false;
        }

        if (isGrounded)
        {
            coyoteCounter = coyoteTime;
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
        }

        if (jumpPressedThisFrame)
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (!wasGrounded && isGrounded)
        {
            if (anim != null)
            {
                anim.Play("Player_JumpSquash", 0, 0f);
            }
        }
    }

    private void FixedUpdate()
    {
        Vector2 v = rb.linearVelocity;
        v.x = moveInput * moveSpeed;
        rb.linearVelocity = v;

        if (jumpBufferCounter > 0f && coyoteCounter > 0f)
        {
            v = rb.linearVelocity;
            v.y = jumpForce;
            rb.linearVelocity = v;

            jumpBufferCounter = 0f;
            coyoteCounter = 0f;

            if (anim != null)
            {
                anim.Play("Player_JumpSquash", 0, 0f);
            }
        }

        if (!jumpHeld && rb.linearVelocity.y > 0f)
        {
            v = rb.linearVelocity;
            v.y *= jumpCutMultiplier;
            rb.linearVelocity = v;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
