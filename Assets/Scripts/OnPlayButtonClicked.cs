using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButtonUI : MonoBehaviour
{
    public void OnPlayButtonClicked()
    {
        SceneManager.LoadScene("Nivell1");
        Debug.Log("Play button (UI) clicked");
    }

    public void OnPLayButtonRestart()
    {
        SceneManager.LoadScene("Menu");
        Debug.Log("Play button (UI) clicked");
    }
}