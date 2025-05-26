using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    public int score = 0;
    public int vidas = 3;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddPoints(int amount)
    {
        score += amount;
    }

    public void LoseLife()
    {
        vidas--;
    }

    public void GainLife()
    {
        vidas++;
    }

    public void ResetGame()
    {
        score = 0;
        vidas = 3;
    }
}
