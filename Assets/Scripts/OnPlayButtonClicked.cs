using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayButtonUI : MonoBehaviour
{
    public Button playButton;

    void Awake()
    {
        if (playButton != null)
        {
            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(OnPlayButtonClicked);
        }
    }

    public void OnPlayButtonClicked()
    {
        if (SceneManager.GetActiveScene().name == "Menu")
        {
            SceneManager.LoadScene("Nivell1");
            Debug.Log("Play button (UI) clicked");
        }
    }

    public void OnPLayButtonRestart()
    {
        if (SceneManager.GetActiveScene().name == "Credits")
        {
            SceneManager.LoadScene("Menu");
            Debug.Log("Play button (UI) clicked");
            GameManager.Instance.ResetGame();
        }
    }
}