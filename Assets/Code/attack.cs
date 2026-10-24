using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Sword Reference")]
    public OrbitingSword orbitingSword;

    [Header("Melee Attack Settings")]
    public float meleeDuration = 0.4f;
    public float meleeCooldown = 0.6f;
    private float lastMeleeTime = -999f;

    [Header("Ranged Shot Settings")]
    public float rangedCooldown = 0.8f;
    private float lastRangedTime = -999f;

    void Update()
    {
        if (Keyboard.current == null && Mouse.current == null) return;

        // 1. Melee Attack: Left Mouse Button or J key
        bool meleePressed = (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) ||
                            (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame);

        if (meleePressed && Time.time >= lastMeleeTime + meleeCooldown)
        {
            if (orbitingSword != null)
            {
                orbitingSword.PerformSpinAttack(meleeDuration);
                lastMeleeTime = Time.time;
            }
        }

        // 2. Ranged Shot Attack: Right Mouse Button or E key
        bool rangedPressed = (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame) ||
                             (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame);

        if (rangedPressed && Time.time >= lastRangedTime + rangedCooldown)
        {
            if (orbitingSword != null)
            {
                orbitingSword.PerformRangedShot();
                lastRangedTime = Time.time;
            }
        }
    }
}