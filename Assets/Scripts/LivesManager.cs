using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using System.Collections;
using UnityEngine.Video;

public class LivesManager : MonoBehaviour
{
    public static LivesManager Instance;
    public GameObject GameOverImage;

    private GameObject[] Hearts;
    private float duracionFade = 1.0f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Hearts = GameObject.FindGameObjectsWithTag("Heart");
        System.Array.Sort(Hearts, (a, b) => string.Compare(a.name, b.name, System.StringComparison.Ordinal));
    }

    private void Update()
    {
        UpdateHearts();
        if (GameManager.Instance.vidas == 0) StartCoroutine(ChangeToMenuCoroutine());
    }

    public void UpdateHearts()
    {
        int vidas = GameManager.Instance.vidas;
        for (int i = 0; i < Hearts.Length; i++)
        {
            if (i < vidas)
            {
                Hearts[i].SetActive(true);
            }
            else
            {
                Hearts[i].SetActive(false);
            }
        }
    }

    private IEnumerator ChangeToMenuCoroutine()
    {
        GameOverImage.SetActive(true);
        yield return StartCoroutine(FadeIn(GameOverImage));
        yield return new WaitForSeconds(3f);
        GameOverImage.SetActive(false);
        GameManager.Instance.ResetGame();
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