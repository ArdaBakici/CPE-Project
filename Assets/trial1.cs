using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public Button startButton;
    public Button exitButton;

    private void Start()
    {
        // Add listeners to the buttons
        startButton.onClick.AddListener(StartGame);
        exitButton.onClick.AddListener(ExitGame);
    }

    private void StartGame()
    {
        // Load the game scene (replace "GameScene" with your actual game scene name)
        SceneManager.LoadScene("Game");
    }

    private void ExitGame()
    {
        // Quit the application (works in standalone builds)
        Application.Quit();
    }
}