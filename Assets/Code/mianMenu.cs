using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // Call this when pressing the Start button
    public void StartGame()
    {
        // Replace "GameScene" with the exact name of your gameplay scene
        SceneManager.LoadScene("SampleScene");
    }

    // Call this when pressing the Exit button
    public void ExitGame()
    {
        Debug.Log("Game is exiting...");
        Application.Quit();
    }
}