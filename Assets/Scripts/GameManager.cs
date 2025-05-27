using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private GameObject[] Hearts;
    private float duracionFade = 1.0f;

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

    void Start()
    {
        Hearts = GameObject.FindGameObjectsWithTag("Heart");
    }

    public IEnumerator UpdateLives(GameObject GameOverImage)
    {
        if (vidas != 0)
        {
            LoseLife();
            if (vidas == 0)
            {
                GameOverImage.SetActive(true);
                yield return StartCoroutine(ChangeToMenuCoroutine(GameOverImage));
            }
            yield break;
        }
    }

    public void AddPoints(int amount)
    {
        score += amount;
    }

    public void LoseLife()
    {
        if (vidas != 0) Hearts[vidas-1].SetActive(false);
        if (vidas > 0) vidas--;
    }

    public void GainLife()
    {
        if (vidas != 3) Hearts[vidas].SetActive(true);
        if (vidas < 3) vidas++;
    }

    public void ResetGame()
    {
        score = 0;
        vidas = 3;
    }

    private IEnumerator ChangeToMenuCoroutine(GameObject text)
    {
        if (text != null)
        {
            text.SetActive(true);
            yield return StartCoroutine(FadeIn(text));
            yield return new WaitForSeconds(3f);
            text.SetActive(false);
        }
        SceneManager.LoadScene(0);
    }

    private System.Collections.IEnumerator FadeIn(GameObject foto)
    {
        var canvasGroup = foto.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            yield break;

        float elapsed = 0f;
        while (elapsed < duracionFade)
        {
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / duracionFade);
            elapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }
}
