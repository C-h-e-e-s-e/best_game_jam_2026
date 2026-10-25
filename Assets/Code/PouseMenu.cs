using UnityEngine;
using UnityEngine.InputSystem; // Required for the new Input System

public class PauseMenu : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject pauseMenuUI; // Drag your PauseMenuUI panel here

    private bool isPaused = false;

    void Update()
    {
        // Check if the Escape key was pressed this frame using the new Input System
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // Unfreeze game time
        isPaused = false;
    }

    void Pause()
    {
        if (pauseMenuUI != null) pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // Freeze game time
        isPaused = true;
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Time.timeScale = 1f; // Reset time before quitting
        Application.Quit(); // Closes built game
    }
}