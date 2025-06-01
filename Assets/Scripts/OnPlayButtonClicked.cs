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
        SceneManager.LoadScene("Nivell1");
        Debug.Log("Play button (UI) clicked");
    }

    public void OnPLayButtonRestart()
    {
        SceneManager.LoadScene("Menu");
        Debug.Log("Play button (UI) clicked");
        GameManager.Instance.ResetGame(); 
    }
}