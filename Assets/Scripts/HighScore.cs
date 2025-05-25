using UnityEngine;
using TMPro;

public class HighScore : MonoBehaviour
{

    public TextMeshProUGUI highScoreText;
    public float delay = 4.75f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        highScoreText.text = "High Score: " + highScore;

        Color c = highScoreText.color;
        c.a = 0f;
        highScoreText.color = c;

        // Mostrar texto tras delay
        Invoke("MostrarTexto", delay);
    }

    void MostrarTexto()
    {
        Color c = highScoreText.color;
        c.a = 1f;
        highScoreText.color = c;
    }
}
