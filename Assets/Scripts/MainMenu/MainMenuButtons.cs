using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    public void PlayGame()
    {
        Debug.Log("Play pressed");
        SceneManager.LoadScene(1); 
    }

    public void OpenOptions()
    {
        Debug.Log("Options pressed");
    }

    public void QuitGame()
    {
        Debug.Log("Quit pressed");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}