using UnityEngine;
using UnityEngine.UI;

public class HealthUIController : MonoBehaviour
{
    [Header("Sprite References")]
    public Sprite fullHeartSprite;
    public Sprite emptyHeartSprite;

    [Header("UI Image Elements")]
    public Image[] heartImages; // Drag your 10 heart images here in order

    // Call this method whenever the player's health/lives change
    public void UpdateHearts(int currentHealth)
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (i < currentHealth)
            {
                heartImages[i].sprite = fullHeartSprite;
            }
            else
            {
                heartImages[i].sprite = emptyHeartSprite;
            }
        }
    }
}