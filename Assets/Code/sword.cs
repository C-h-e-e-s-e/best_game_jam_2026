using System.Collections;
using UnityEngine;

public class OrbitingSword : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    private Rigidbody2D playerRb;

    [Header("Idle Settings")]
    public Vector3 idleOffsetRight = new Vector3(1.2f, 0.5f, 0f);
    public Vector3 idleOffsetLeft = new Vector3(-1.2f, 0.5f, 0f);
    public float followSpeed = 10f;

    [Header("Orbit Attack Settings")]
    public float orbitRadius = 1.5f;
    public float orbitSpeed = 720f;
    public int meleeDamage = 10;

    [Header("Ranged Shot Settings")]
    public float shootSpeed = 20f;
    public float shootRange = 10f;
    public int rangedDamage = 15;

    private bool isAttacking;
    private bool isShooting;
    private float currentAngle;
    private float currentAttackTime;
    private float totalAttackDuration;
    private float facingDirection = 1f;

    void Start()
    {
        if (player != null)
        {
            playerRb = player.GetComponent<Rigidbody2D>();
        }
    }

    private float lockedAttackDirection = 1f; // Lock direction during attack

    void Update()
    {
        if (player == null || isShooting) return;

        // Track player facing direction (only when not attacking)
        if (!isAttacking && playerRb != null && Mathf.Abs(playerRb.linearVelocity.x) > 0.1f)
        {
            facingDirection = Mathf.Sign(playerRb.linearVelocity.x);
        }

        if (isAttacking)
        {
            // Use the locked direction so it never changes mid-attack
            currentAngle -= (orbitSpeed * lockedAttackDirection) * Time.deltaTime;
            float radian = currentAngle * Mathf.Deg2Rad;

            float x = Mathf.Cos(radian) * orbitRadius;
            float y = Mathf.Sin(radian) * orbitRadius;

            transform.position = player.position + new Vector3(x, y, 0f);

            // Adjust sword rotation to match the locked spin direction
            transform.rotation = Quaternion.Euler(0, 0, currentAngle - (90f * lockedAttackDirection));

            currentAttackTime += Time.deltaTime;
            if (currentAttackTime >= totalAttackDuration)
            {
                StopAttack();
            }
        }
        else
        {
            Vector3 targetOffset = (facingDirection > 0f) ? idleOffsetRight : idleOffsetLeft;
            Vector3 targetPosition = player.position + targetOffset;

            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.identity, Time.deltaTime * 5f);
        }
    }

    public void PerformSpinAttack(float duration)
    {
        if (isAttacking || isShooting) return;

        isAttacking = true;
        currentAttackTime = 0f;
        totalAttackDuration = duration;

        // Lock in whichever direction you are currently facing for the duration of the attack
        lockedAttackDirection = facingDirection;

        Vector3 offsetFromPlayer = transform.position - player.position;
        currentAngle = Mathf.Atan2(offsetFromPlayer.y, offsetFromPlayer.x) * Mathf.Rad2Deg;
    }
    public void PerformRangedShot()
    {
        if (isAttacking || isShooting) return;
        StartCoroutine(ShootRoutine());
    }

    private IEnumerator ShootRoutine()
    {
        isShooting = true;
        Vector3 startPos = transform.position;
        Vector3 shootDir = new Vector3(facingDirection, 0f, 0f);

        // Point sword in shooting direction
        float angle = facingDirection > 0f ? 0f : 180f;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        float distanceTraveled = 0f;

        // Fly forward
        while (distanceTraveled < shootRange)
        {
            float step = shootSpeed * Time.deltaTime;
            transform.position += shootDir * step;
            distanceTraveled += step;
            yield return null;
        }

        // Return smoothly back to player idle position
        float returnTimer = 0f;
        float returnDuration = 0.3f;
        Vector3 returnStartPos = transform.position;

        while (returnTimer < returnDuration)
        {
            returnTimer += Time.deltaTime;
            Vector3 targetOffset = (facingDirection > 0f) ? idleOffsetRight : idleOffsetLeft;
            Vector3 targetPos = player.position + targetOffset;
            transform.position = Vector3.Lerp(returnStartPos, targetPos, returnTimer / returnDuration);
            yield return null;
        }

        isShooting = false;
    }

    private void StopAttack()
    {
        isAttacking = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((isAttacking || isShooting) && collision.CompareTag("Enemy"))
        {
            int damageToDeal = isShooting ? rangedDamage : meleeDamage;
            collision.SendMessage("TakeDamage", damageToDeal, SendMessageOptions.DontRequireReceiver);
        }
    }
}