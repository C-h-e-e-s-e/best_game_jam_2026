using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Damage Settings")]
    public float damageRadius = 0.8f; // How close an enemy needs to be to hurt you
    public int touchDamage = 10;
    public float invulnerabilityDuration = 0.5f;
    private float lastDamageTime = -999f;

    [Header("References")]
    private PlayerPlatformerController movementController;

    void Start()
    {
        currentHealth = maxHealth;
        movementController = GetComponent<PlayerPlatformerController>();
    }

    void Update()
    {
        CheckEnemyProximity();
    }

    void CheckEnemyProximity()
    {
        // Find all objects tagged as Enemy
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            if (enemy == null) continue;

            float distance = Vector2.Distance(transform.position, enemy.transform.position);

            // If an enemy is within touching distance, try to take damage
            if (distance <= damageRadius)
            {
                TryTakeDamage(touchDamage);
                break; // Only take damage from one enemy per cooldown tick
            }
        }
    }

    public void TryTakeDamage(int damage)
    {
        if (movementController != null && movementController.IsDashing) return;
        if (Time.time < lastDamageTime + invulnerabilityDuration) return;

        currentHealth -= damage;
        lastDamageTime = Time.time;
        Debug.Log($"Player took damage! HP left: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player Died!");
         UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}