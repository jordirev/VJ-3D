using UnityEngine;
using UnityEngine.UI; // O TMPro si usas TextMeshPro

public class CameraMovement : MonoBehaviour
{
    public float rotationSpeed = 30f;
    private float accumulatedAngle = 0f;
    private bool hasStartedGame = false;

    public GameObject mensajeTexto;
    public float duracionFade = 1f;
    public float tiempoVisible = 2f;

    // Evento que se dispara al terminar la vuelta
    public event System.Action OnRotationComplete;

    private bool textoMostrado = false;
    private Coroutine fadeCoroutine;

    private void Start()
    {
        // Asegúrate de que el texto esté oculto al inicio
        if (mensajeTexto != null)
        {
            mensajeTexto.SetActive(false);
        }
    }

    void Update()
    {
        if (!hasStartedGame)
        {
            float angle = rotationSpeed * Time.deltaTime;
            transform.RotateAround(Vector3.zero, Vector3.up, angle);
            accumulatedAngle += angle;
            transform.LookAt(Vector3.zero);

            // Inicia el fade in solo una vez al principio
            if (!textoMostrado)
            {
                textoMostrado = true;
                fadeCoroutine = StartCoroutine(FadeInTexto());
            }

            // Calcula cuándo empezar el fade out (antes de terminar la vuelta)
            if (textoMostrado && fadeCoroutine != null && accumulatedAngle >= 360f - (duracionFade * rotationSpeed / 2f))
            {
                // Inicia el fade out si aún no ha empezado
                StopCoroutine(fadeCoroutine);
                fadeCoroutine = StartCoroutine(FadeOutTexto());
                textoMostrado = false; // Para que no vuelva a intentar el fade out
            }

            if (accumulatedAngle >= 360f)
            {
                hasStartedGame = true;
                OnRotationComplete?.Invoke(); // Lanza el callback
            }
        }
    }

    private System.Collections.IEnumerator FadeInTexto()
    {
        if (mensajeTexto == null) yield break;

        mensajeTexto.SetActive(true);
        var canvasGroup = mensajeTexto.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = mensajeTexto.AddComponent<CanvasGroup>();

        float elapsed = 0f;
        while (elapsed < duracionFade)
        {
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / duracionFade);
            elapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    private System.Collections.IEnumerator FadeOutTexto()
    {
        if (mensajeTexto == null) yield break;

        var canvasGroup = mensajeTexto.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            yield break;

        float elapsed = 0f;
        while (elapsed < duracionFade)
        {
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / duracionFade);
            elapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0f;
        mensajeTexto.SetActive(false);
    }
}
