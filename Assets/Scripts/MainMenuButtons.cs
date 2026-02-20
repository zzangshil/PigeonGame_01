using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    // Called when Play button is clicked
    public void PlayGame()
    {
        Debug.Log("Play pressed");
        SceneManager.LoadScene(1); // Load scene with build index 1
    }

    // Called when Options button is clicked
    public void OpenOptions()
    {
        Debug.Log("Options pressed");
        // Open options panel here if you have one
    }

    // Called when Quit button is clicked
    public void QuitGame()
    {
        Debug.Log("Quit pressed");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}