using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int score = 0;
    public int vidas = 3;

    public bool cupAppeared = false;

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
        if (vidas > 0) vidas--;
    }

    public void GainLife()
    {
        if (vidas < 3) vidas++;
    }

    public void ResetGame()
    {
        score = 0;
        vidas = 3;
        cupAppeared = false;
    }
}
