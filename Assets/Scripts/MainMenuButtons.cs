using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    // Called when Play button is clicked
    public void PlayGame()
    {
        Debug.Log("Play pressed");
        // Replace "GameScene" with your actual gameplay scene name
        SceneManager.LoadScene("GameScene");
    }

    // Called when Options button is clicked
    public void OpenOptions()
    {
        Debug.Log("Options pressed");
        // Here you can open an options panel
        // For example: optionsPanel.SetActive(true);
    }

    // Called when Quit button is clicked
    public void QuitGame()
    {
        Debug.Log("Quit pressed");
        Application.Quit();

        // If in Editor, also stop play mode
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}