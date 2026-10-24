using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 30;
    private int currentHealth;
    public float moveSpeed = 3.5f;
    public float jumpForce = 12f;

    [Header("Platform AI Settings")]
    public LayerMask groundLayer;
    public float checkDistance = 1.5f;
    public float jumpCooldown = 0.6f;
    private float lastJumpTime = -999f;

    [Header("References")]
    private Transform player;
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;
    private bool isGrounded;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        // Ground Check
        Bounds bounds = boxCollider.bounds;
        RaycastHit2D groundHit = Physics2D.BoxCast(bounds.center, new Vector2(bounds.size.x * 0.9f, 0.1f), 0f, Vector2.down, bounds.extents.y + 0.1f, groundLayer);
        isGrounded = groundHit.collider != null;

        if (player == null || !isGrounded) return;

        // Direction toward player
        float direction = Mathf.Sign(player.position.x - transform.position.x);

        // 1. Horizontal wall check
        Vector2 origin = new Vector2(transform.position.x, bounds.center.y);
        RaycastHit2D wallHit = Physics2D.Raycast(origin, new Vector2(direction, 0), checkDistance, groundLayer);

        // 2. Upward/Diagonal ledge check (looks up towards platforms above you)
        Vector2 diagonalDir = new Vector2(direction, 1f).normalized;
        RaycastHit2D platformAboveHit = Physics2D.Raycast(origin, diagonalDir, checkDistance * 1.5f, groundLayer);

        // 3. Check if player is higher up
        bool playerIsAbove = player.position.y > transform.position.y + 0.5f;

        // Jump if there's a wall, an overhead platform path, or the player is higher up
        if ((wallHit.collider != null || platformAboveHit.collider != null || playerIsAbove) && Time.time >= lastJumpTime + jumpCooldown)
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        if (player == null) return;

        float direction = Mathf.Sign(player.position.x - transform.position.x);
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);

        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = (direction < 0f);
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        lastJumpTime = Time.time;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"Enemy took {damage} damage! HP left: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, Vector3.right * checkDistance);
        Gizmos.DrawRay(transform.position, Vector3.left * checkDistance);
    }
}