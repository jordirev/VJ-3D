using UnityEngine;
using TMPro;

public class LivesManager : MonoBehaviour
{
    public TextMeshProUGUI livesText;

    void Start()
    {
        UpdateLivesText();
    }

    void Update()
    {
        UpdateLivesText();
    }

    void UpdateLivesText()
    {
        if (GameManager.Instance != null)
        {
            livesText.text = "Lives: " + GameManager.Instance.vidas;
        }
    }
}