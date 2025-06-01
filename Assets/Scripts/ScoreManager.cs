using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    void Start()
    {
        UpdateScoreText();
    }

    void Update()
    {
        UpdateScoreText();
    }

    public void AddPoints(int amount)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.score += amount;
            UpdateScoreText();
            SaveHighScore();
        }
    }

    void UpdateScoreText()
    {
        if (GameManager.Instance != null)
        {
            scoreText.text = "Score: " + GameManager.Instance.score;
        }
    }

    public void SaveHighScore()
    {
        if (GameManager.Instance != null)
        {
            int highScore = PlayerPrefs.GetInt("HighScore", 0);
            if (GameManager.Instance.score > highScore)
            {
                PlayerPrefs.SetInt("HighScore", GameManager.Instance.score);
                PlayerPrefs.Save();
            }
        }
    }

    public int GetScore()
    {
        if (GameManager.Instance != null)
            return GameManager.Instance.score;
        return 0;
    }
}