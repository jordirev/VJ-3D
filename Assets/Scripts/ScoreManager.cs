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


  /*  public void AddPoints(int amount)
    {
        score += amount;
        UpdateScoreText();

   
        SaveHighScore();
    }*/

    void UpdateScoreText()
    {
        if (GameManager.Instance != null)
        {
            scoreText.text = "Score: " + GameManager.Instance.score;
        }
    }

  /*  public void SaveHighScore()
    {
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (score > highScore)
        {
            PlayerPrefs.SetInt("HighScore", score);
            PlayerPrefs.Save();
        }
    }*/

   /* public int GetScore()
    {
        return score;
    }*/
}