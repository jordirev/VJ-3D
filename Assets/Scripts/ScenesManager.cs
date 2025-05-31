using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Runtime.ExceptionServices;

public class ScenesManager : MonoBehaviour
{
    private int cantidadDeBloques, cantidadDeBloquesMax;
    private float duracionFade = 1.0f;
    bool eventTriggered = false;

    public event Action ActivateCupPowerUp;

    // Referencia al array de Texts
    public GameObject nextLevelImage;

    void Start()
    {
        cantidadDeBloques = GameObject.FindGameObjectsWithTag("Destructible").Length;
        cantidadDeBloquesMax = cantidadDeBloques;
        nextLevelImage.SetActive(false);
    }

    private void Awake()
    {
        nextLevelImage.SetActive(false);
    }

    void Update()
    {
        if (Input.anyKeyDown) changeScene();
        cantidadDeBloques = GameObject.FindGameObjectsWithTag("Destructible").Length;

        if ((cantidadDeBloques * 100) / cantidadDeBloquesMax <= 5f && !eventTriggered)
        {
            eventTriggered = true;
            ActivateCupPowerUp?.Invoke();
        }
        else if (cantidadDeBloques <= 0)
        {
            StartCoroutine(ChangeToNextSceneCoroutine(nextLevelImage));
        }
    }

    private void changeScene()
    {
        switch (true)
        {
            case bool _ when Input.GetKeyDown(KeyCode.Alpha1):
                SceneManager.LoadScene("Nivell1");
                break;
            case bool _ when Input.GetKeyDown(KeyCode.Alpha2):
                SceneManager.LoadScene("Nivell2");
                break;
            case bool _ when Input.GetKeyDown(KeyCode.Alpha3):
                SceneManager.LoadScene("Nivell3");
                break;
            case bool _ when Input.GetKeyDown(KeyCode.Alpha4):
                SceneManager.LoadScene("Nivell4");
                break;
            case bool _ when Input.GetKeyDown(KeyCode.Alpha5):
                SceneManager.LoadScene("Nivell5");
                break;
            case bool _ when Input.GetKeyDown(KeyCode.H):
                GameManager.Instance.vidas = 3;
                break;
        }
    }

    private IEnumerator ChangeToNextSceneCoroutine(GameObject text)
    {
        text.SetActive(true);
        yield return StartCoroutine(FadeIn(text));
        yield return new WaitForSeconds(4f);
        text.SetActive(false);
        int escenaActual = SceneManager.GetActiveScene().buildIndex;
        int totalEscenas = SceneManager.sceneCountInBuildSettings;
        int siguienteEscena = (escenaActual + 1);
        SceneManager.LoadScene(siguienteEscena);
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