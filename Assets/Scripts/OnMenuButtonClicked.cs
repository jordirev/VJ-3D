using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class menuButtonUI : MonoBehaviour
{
    public Button menuButton;

    void Awake()
    {
        if (menuButton != null)
        {
            menuButton.onClick.RemoveAllListeners();
            menuButton.onClick.AddListener(OnmenuButtonRestart);
        }
    }

    public void OnmenuButtonRestart()
    {
        if (SceneManager.GetActiveScene().name == "Credits")
        {
            SceneManager.LoadScene("Menu");
            Debug.Log("Play button (UI) clicked");
            GameManager.Instance.ResetGame();
        }
    }
}