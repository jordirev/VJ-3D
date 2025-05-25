using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelText : MonoBehaviour
{
    public TextMeshProUGUI levelText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        string levelNumber = GetNumberFromSceneName(sceneName);

        levelText.text = "LEVEL: " + levelNumber;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    string GetNumberFromSceneName(string sceneName)
    {
        // Busca el primer número en el nombre (por ejemplo, "Nivell2" → 2)
        foreach (char c in sceneName)
        {
            if (char.IsDigit(c))
            {
                return c.ToString();
            }
        }
        return "?"; // Si no encuentra número
    }
}
