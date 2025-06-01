using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraMenuMovement : MonoBehaviour
{
    public GameObject[] fotos; // Asigna las fotos desde el inspector
    public float tiempoEntreFotos = 0.5f;
    public float duracionFade = 1f; // Duración del fade-in en segundos
    public float tiempoEntreFoto1y2 = 1.5f; // Tiempo especial entre foto 1 y 2 en Credits

    private bool fotosMostradas = false;

   

    void Awake()
    {
        Vector3 pos = transform.position;
        pos.y = 17.8f;
        transform.position = pos;

        // Oculta todas las fotos al inicio y pone su alpha en 0
        if (fotos != null)
        {
            foreach (var foto in fotos)
            {
                if (foto != null)
                {
                    foto.SetActive(false);
                    var canvasGroup = foto.GetComponent<CanvasGroup>();
                    if (canvasGroup == null)
                        canvasGroup = foto.AddComponent<CanvasGroup>();
                    canvasGroup.alpha = 0f;
                }
            }
        }
    }

    void Update()
    {
        Vector3 pos = transform.position;
        if (pos.y > 11.62f)
        {
            float distancia = pos.y - 11.62f;
            // Velocidad base más rápida, pero disminuye cerca del destino
            float velocidad = Mathf.Lerp(0.5f, 3f, Mathf.Clamp01(distancia / 3f));
            pos.y = Mathf.MoveTowards(pos.y, 11.62f, Time.deltaTime * velocidad);
            transform.position = pos;
        }
        else if (!fotosMostradas)
        {
            fotosMostradas = true;
            StartCoroutine(MostrarFotosPorEscena());
        }
    }

    private System.Collections.IEnumerator MostrarFotosPorEscena()
    {
        if (fotos == null) yield break;
        string escena = SceneManager.GetActiveScene().name;

        if (escena == "Menu")
        {
            // Mostrar la foto en el índice 0 primero (si existe), luego la 1 (botón de play)
            if (fotos.Length > 0 && fotos[0] != null)
            {
                fotos[0].SetActive(true);
                yield return StartCoroutine(FadeIn(fotos[0]));
            }
            if (fotos.Length > 1 && fotos[1] != null)
            {
                fotos[1].SetActive(true);
                yield return StartCoroutine(FadeIn(fotos[1]));
            }
        }
        else if (escena == "Credits")
        {
            // Mostrar la foto 0
            if (fotos.Length > 0 && fotos[0] != null)
            {
                fotos[0].SetActive(true);
                yield return StartCoroutine(FadeIn(fotos[0]));
            }
            yield return new WaitForSeconds(0.5f);

            // Mostrar la foto 1
            if (fotos.Length > 1 && fotos[1] != null)
            {
                fotos[1].SetActive(true);
                yield return StartCoroutine(FadeIn(fotos[1]));
            }
            yield return new WaitForSeconds(5f); // Cambiado a 5 segundos

            // Desaparecer la foto 1 y mostrar la foto 2
            if (fotos.Length > 1 && fotos[1] != null)
            {
                fotos[1].SetActive(false);
            }
            if (fotos.Length > 2 && fotos[2] != null)
            {
                fotos[2].SetActive(true);
                float duracionFadeOriginal = duracionFade;
                duracionFade = duracionFade * 1.5f; // Hace que la foto 2 tarde más en aparecer
                yield return StartCoroutine(FadeIn(fotos[2]));
                duracionFade = duracionFadeOriginal; // Restaura la duración original
            }
        }
        else
        {
            // Comportamiento por defecto: mostrar todas progresivamente
            foreach (var foto in fotos)
            {
                if (foto != null)
                {
                    //foto.SetActive(true);
                    yield return StartCoroutine(FadeIn(foto));
                    yield return new WaitForSeconds(tiempoEntreFotos);
                }
            }
        }
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
