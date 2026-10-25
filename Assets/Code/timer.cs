using UnityEngine;
using TMPro; // Remove this if using standard Unity UI Text

public class GameTimer : MonoBehaviour
{
    public TMP_Text timerText; // Drag your TimerText here
    private float elapsedTime = 0f;

    void Update()
    {
        // Tracks time (automatically pauses when Time.timeScale = 0 during upgrades or death)
        elapsedTime += Time.deltaTime;

        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);

        if (timerText != null)
        {
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}