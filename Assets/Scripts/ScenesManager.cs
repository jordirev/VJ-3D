using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class ScenesManager : MonoBehaviour
{
    private int cantidadDeBloques, cantidadDeBloquesMax;
    bool eventTriggered = false;

    public event Action ActivateCupPowerUp;

    // Referencia al array de Texts
    public GameObject[] textosUI;

    void Start()
    {
        cantidadDeBloques = GameObject.FindGameObjectsWithTag("Destructible").Length;
        cantidadDeBloquesMax = cantidadDeBloques;
    }

    void Update()
    {
        if (Input.anyKeyDown) changeScene();
        cantidadDeBloques = GameObject.FindGameObjectsWithTag("Destructible").Length;

        if ((cantidadDeBloques * 100) / cantidadDeBloquesMax <= 0.05f && !eventTriggered)
        {
            eventTriggered = true;
            ActivateCupPowerUp?.Invoke();
            StartCoroutine(MostrarTextoPorTiempo(0, 4f));
        }
        else if (cantidadDeBloques <= 0)
        {
            StartCoroutine(MostrarTextoPorTiempo(1, 4f));
            int escenaActual = SceneManager.GetActiveScene().buildIndex;
            int totalEscenas = SceneManager.sceneCountInBuildSettings;
            int siguienteEscena = (escenaActual + 1) % totalEscenas;
            SceneManager.LoadScene(siguienteEscena);
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
        }
    }

    private IEnumerator MostrarTextoPorTiempo(int indice, float segundos)
    {
        if (textosUI != null && textosUI.Length > indice && textosUI[indice] != null)
        {
            var textComponent = textosUI[indice].GetComponent<UnityEngine.UI.Text>();
            if (textComponent != null)
            {
                // Aumentar alfa (fade in)
                float fadeInTime = 1f;
                float fadeOutTime = 1f;
                float visibleTime = Mathf.Max(0, segundos - fadeInTime - fadeOutTime);
                Color originalColor = textComponent.color;
                Color color = originalColor;
                color.a = 0f;
                textComponent.color = color;
                textComponent.enabled = true;

                float t = 0f;
                while (t < fadeInTime)
                {
                    t += Time.deltaTime;
                    color.a = Mathf.Clamp01(t / fadeInTime);
                    textComponent.color = color;
                    yield return null;
                }
                color.a = 1f;
                textComponent.color = color;

                // Mantener visible
                yield return new WaitForSeconds(visibleTime);

                // Disminuir alfa (fade out)
                t = 0f;
                while (t < fadeOutTime)
                {
                    t += Time.deltaTime;
                    color.a = Mathf.Clamp01(1f - (t / fadeOutTime));
                    textComponent.color = color;
                    yield return null;
                }
                color.a = 0f;
                textComponent.color = color;
                textComponent.enabled = false;
            }
        }
    }
}
