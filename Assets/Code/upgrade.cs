using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    [Header("UI References")]
    public GameObject upgradeMenuUI;

    [Header("Player & Weapon References")]
    public OrbitingSword orbitingSword;
    public PlayerHealth playerHealth;

    [Header("Settings")]
    public int killsNeededForUpgrade = 15;
    private int currentKills = 0;
    private bool isUpgradeActive = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (upgradeMenuUI != null)
        {
            upgradeMenuUI.SetActive(false);
        }
    }

    public void AddKill()
    {
        if (isUpgradeActive) return;

        currentKills++;
        Debug.Log($"Enemy Killed! Count: {currentKills}/{killsNeededForUpgrade}");

        if (currentKills >= killsNeededForUpgrade)
        {
            TriggerUpgradeMenu();
        }
    }

    void TriggerUpgradeMenu()
    {
        isUpgradeActive = true;
        currentKills = 0; // Reset counter for next tier

        if (upgradeMenuUI != null)
        {
            upgradeMenuUI.SetActive(true);
        }

        // Freeze game time so enemies and player stop moving
        Time.timeScale = 0f;
        Debug.Log("Upgrade Menu Triggered!");
    }

    // --- BUTTON CLICK METHODS ---

    public void OnAttackButtonClicked()
    {
        if (orbitingSword != null)
        {
            orbitingSword.meleeDamage += 2;
            orbitingSword.rangedDamage += 2;
            Debug.Log("Attack upgraded by +2!");
        }
        else
        {
            Debug.LogWarning("OrbitingSword reference missing on UpgradeManager!");
        }
        CloseUpgradeMenu();
    }

    public void OnDashButtonClicked()
    {
        // Add your dash stat increase here if stored in a movement script
        Debug.Log("Dash upgraded by +2!");
        CloseUpgradeMenu();
    }

    public void OnHealthButtonClicked()
    {
        if (playerHealth != null)
        {
            playerHealth.maxHealth += 2;
            Debug.Log("Max Health upgraded by +2!");
        }
        else
        {
            Debug.LogWarning("PlayerHealth reference missing on UpgradeManager!");
        }
        CloseUpgradeMenu();
    }

    void CloseUpgradeMenu()
    {
        isUpgradeActive = false;

        if (upgradeMenuUI != null)
        {
            upgradeMenuUI.SetActive(false);
        }

        // Unfreeze game time
        Time.timeScale = 1f;
    }
}