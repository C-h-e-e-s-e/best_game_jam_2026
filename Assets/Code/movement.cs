using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class PlayerPlatformerController : MonoBehaviour
{
    public bool IsDashing => isDashing;

    [Header("Movement")]
    public float moveSpeed = 8f;
    private float horizontalInput;
    private float lastFacingDirection = 1f;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    [Header("Jumping")]
    public float jumpForce = 12f;
    public int maxExtraJumps = 1;
    private int extraJumpsRemaining;

    [Header("Dash")]
    public float dashPower = 24f;
    public float dashTime = 0.2f;
    public float dashCooldown = 1f;
    private bool canDash = true;
    private bool isDashing;

    [Header("Jump Feel Improvements")]
    [SerializeField] private float coyoteTime = 0.15f;
    private float coyoteTimeCounter;
    [SerializeField] private float jumpBufferTime = 0.15f;
    private float jumpBufferCounter;

    [Header("Ground Check (Automatic)")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckExtraDepth = 0.05f;
    private bool isGrounded;

    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private float originalGravity;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        rb.freezeRotation = true;
        originalGravity = rb.gravityScale;

        // Find SpriteRenderer and Animator in children or on the object itself
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // Don't process normal input or jump checks while actively dashing
        if (isDashing) return;

        // 1. Direct Keyboard Polling
        horizontalInput = 0f;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                horizontalInput = -1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                horizontalInput = 1f;

            // Track last non-zero direction for dashing while idle
            if (horizontalInput != 0f)
            {
                lastFacingDirection = Mathf.Sign(horizontalInput);
            }

            // Jump press
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                jumpBufferCounter = jumpBufferTime;

                if (animator != null)
                {
                    animator.SetTrigger("JumpTrigger");
                }
            }

            // Variable jump height
            if (Keyboard.current.spaceKey.wasReleasedThisFrame)
            {
                if (rb.linearVelocity.y > 0f)
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
                }
            }

            // Dash Trigger (Left Shift, K, or C)
            if ((Keyboard.current.leftShiftKey.wasPressedThisFrame ||
                 Keyboard.current.kKey.wasPressedThisFrame ||
                 Keyboard.current.cKey.wasPressedThisFrame) && canDash)
            {
                StartCoroutine(PerformDash());
            }
        }

        // 2. Sprite Flipping & Animations
        if (horizontalInput > 0.01f)
        {
            spriteRenderer.flipX = false; // Facing right
        }
        else if (horizontalInput < -0.01f)
        {
            spriteRenderer.flipX = true;  // Facing left
        }

        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(horizontalInput));
        }

        // 3. Automatic Ground Check
        Bounds bounds = boxCollider.bounds;
        RaycastHit2D hit = Physics2D.BoxCast(bounds.center, new Vector2(bounds.size.x * 0.9f, 0.1f), 0f, Vector2.down, bounds.extents.y + groundCheckExtraDepth, groundLayer);

        isGrounded = hit.collider != null;

        if (isGrounded)
        {
            extraJumpsRemaining = maxExtraJumps;
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (jumpBufferCounter > 0)
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // 4. Execute Jump
        if (jumpBufferCounter > 0)
        {
            if (coyoteTimeCounter > 0f)
            {
                ExecuteJump();
                jumpBufferCounter = 0f;
            }
            else if (extraJumpsRemaining > 0)
            {
                ExecuteJump();
                extraJumpsRemaining--;
                jumpBufferCounter = 0f;
            }
        }
    }

    void FixedUpdate()
    {
        if (isDashing) return;

        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }

    private void ExecuteJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        coyoteTimeCounter = 0f;
    }

    private IEnumerator PerformDash()
    {
        canDash = false;
        isDashing = true;

        // Temporarily disable gravity during dash
        rb.gravityScale = 0f;

        // Determine dash direction (use current input or last facing direction)
        float dashDirection = horizontalInput != 0f ? Mathf.Sign(horizontalInput) : lastFacingDirection;

        // Apply high horizontal velocity (Fixed the missing multiplication operator here)
        rb.linearVelocity = new Vector2(dashDirection * dashPower, 0f);

        // Wait for dash duration
        yield return new WaitForSeconds(dashTime);

        // Restore normal physics
        rb.gravityScale = originalGravity;
        isDashing = false;

        // Wait for cooldown
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}