using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public TextMeshProUGUI scoreText;
    private int score = 0;

    void Awake()
    {
        // Singleton pattern para que sea accesible desde otros scripts
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateScoreText();
    }


    public void AddPoints(int amount)
    {
        score += amount;
        UpdateScoreText();

        //CUANDO TERMINE LA PARTIDA LLAMAR A:
        // ScoreManager.instance.SaveHighScore();
        //Y QUITAR LINEA DE ABAJO
        SaveHighScore();
    }

    void UpdateScoreText()
    {
        scoreText.text = "Score: " + score;
    }

    public void SaveHighScore()
    {
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (score > highScore)
        {
            PlayerPrefs.SetInt("HighScore", score);
            PlayerPrefs.Save();
        }
    }

    public int GetScore()
    {
        return score;
    }
}