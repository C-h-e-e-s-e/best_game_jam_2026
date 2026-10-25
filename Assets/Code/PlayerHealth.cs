using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 10;
    private int currentHealth;

    [Header("Damage & Proximity Settings")]
    public float damageRadius = 1.2f;
    public float invulnerabilityDuration = 0.5f;
    private float lastDamageTime = -999f;

    [Header("UI & Visual References")]
    public HealthUIController healthUI;
    public GameObject deathScreenUI;
    public SpriteRenderer playerSpriteRenderer; // Drag your player's SpriteRenderer here

    [Header("References")]
    private PlayerPlatformerController movementController;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        movementController = GetComponent<PlayerPlatformerController>();

        if (playerSpriteRenderer == null)
            playerSpriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (healthUI != null) healthUI.UpdateHearts(currentHealth);
        if (deathScreenUI != null) deathScreenUI.SetActive(false);
    }

    void Update()
    {
        if (isDead)
        {
            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                RestartGame();
            }
            return;
        }

        CheckEnemyProximity();
    }

    void CheckEnemyProximity()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            if (enemy == null) continue;

            float distance = Vector2.Distance(transform.position, enemy.transform.position);

            if (distance <= damageRadius)
            {
                TryTakeDamage(1);
                break;
            }
        }
    }

    public void TryTakeDamage(int damage)
    {
        if (isDead) return;
        if (movementController != null && movementController.IsDashing) return;
        if (Time.time < lastDamageTime + invulnerabilityDuration) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        lastDamageTime = Time.time;

        // Trigger red flash effect
        StartCoroutine(FlashRedRoutine());

        if (healthUI != null) healthUI.UpdateHearts(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator FlashRedRoutine()
    {
        if (playerSpriteRenderer != null)
        {
            Color originalColor = playerSpriteRenderer.color;
            playerSpriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.5f);
            playerSpriteRenderer.color = originalColor;
        }
    }

    private void Die()
    {
        isDead = true;
        if (deathScreenUI != null) deathScreenUI.SetActive(true);
        Time.timeScale = 0f;
    }

    private void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}