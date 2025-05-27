using UnityEngine;
using System;

public class CameraMovement : MonoBehaviour
{
    public float rotationSpeed = 30f;
    private float accumulatedAngle = 0f;
    private bool hasStartedGame = false;

    public GameObject titleLvl;
    public float fadeDuration = 1f; // Duración del fade en segundos

    // Evento que se dispara al terminar la vuelta
    public event Action OnRotationComplete;

    private CanvasGroup titleCanvasGroup;
    private bool isFadingIn = false;
    private bool isFadingOut = false;
    private float fadeTimer = 0f;

    void Start()
    {
        if (titleLvl != null)
        {
            titleCanvasGroup = titleLvl.GetComponent<CanvasGroup>();
            if (titleCanvasGroup == null)
                titleCanvasGroup = titleLvl.AddComponent<CanvasGroup>();
            titleCanvasGroup.alpha = 0f;
            titleLvl.SetActive(true);
            isFadingIn = true;
            fadeTimer = 0f;
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

            if (isFadingIn)
            {
                fadeTimer += Time.deltaTime;
                float t = Mathf.Clamp01(fadeTimer / fadeDuration);
                titleCanvasGroup.alpha = t;
                if (t >= 1f)
                {
                    isFadingIn = false;
                }
            }

            if (accumulatedAngle >= 360f && !isFadingOut)
            {
                hasStartedGame = true;
                isFadingOut = true;
                fadeTimer = 0f;
            }
        }

        if (isFadingOut && titleCanvasGroup != null)
        {
            fadeTimer += Time.deltaTime;
            float t = Mathf.Clamp01(fadeTimer / fadeDuration);
            titleCanvasGroup.alpha = 1f - t;
            if (t >= 1f)
            {
                isFadingOut = false;
                titleLvl.SetActive(false);
                OnRotationComplete?.Invoke(); // Lanza el callback
            }
        }
    }
}
