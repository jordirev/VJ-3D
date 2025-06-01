using UnityEngine;
using TMPro;

public class HighScore : MonoBehaviour
{
    public TextMeshProUGUI highScoreText;
    public float delay = 4.75f;

    public void ShowHighScore()
    {
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        highScoreText.text = "High Score: " + highScore;

        Color c = highScoreText.color;
        c.a = 0f;
        highScoreText.color = c;

        CancelInvoke("MostrarTexto");
        Invoke("MostrarTexto", delay);
    }

    void Start()
    {
        ShowHighScore();
    }

    void MostrarTexto()
    {
        Color c = highScoreText.color;
        c.a = 1f;
        highScoreText.color = c;
    }
}