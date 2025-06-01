using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Runtime.ExceptionServices;

public class ScenesManager : MonoBehaviour
{
    private int cantidadDeBloques, cantidadDeBloquesMax;
    private float duracionFade = 1.0f;

    public event Action ActivateCupPowerUp;

    // Referencia al array de Texts
    public GameObject nextLevelImage;

    [Header("Música de Fondo")]
    [SerializeField] private AudioClip musicaFondo; 
    [SerializeField] private float volumenMusica = 0.5f;
    [SerializeField] private AudioClip sonidoWin;
    [SerializeField] private float volumenWin = 0.5f;
    private AudioSource audioSource;
    private bool estaEnNivel = false;
    private bool soundplayed = false;

    private bool triggered = false;



    void Awake()
    {
        // Verificar si existe un AudioSource, si no, crearlo
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Configurar el AudioSource
        audioSource.clip = musicaFondo;
        audioSource.loop = true;
        audioSource.volume = volumenMusica;
        audioSource.playOnAwake = false; // No reproducir automáticamente

        // Suscribirse al evento de cambio de escena
        SceneManager.sceneLoaded += OnSceneLoaded;

        nextLevelImage.SetActive(false);

        GameManager.Instance.cupAppeared = false;
        soundplayed = false;
        triggered = false;
    }

    /*void OnDestroy()
    {
        // Desuscribirse del evento al destruir el objeto
        SceneManager.sceneLoaded -= OnSceneLoaded;

        ScenesManager SceneMngr = Object.FindFirstObjectByType<ScenesManager>();
        if (SceneMngr != null)
        {
            SceneMngr.ActivateCupPowerUp -= OnActivateCupPowerUp;
        }
    }*/

    // Este método se llama cada vez que se carga una escena
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Verificar si es un nivel (1-5) o el menú
        string nombreEscena = scene.name;
        bool esNivel = nombreEscena.StartsWith("Nivell");

        if (esNivel && !audioSource.isPlaying)
        {
            // Si es un nivel y la música no está sonando, iniciarla
            audioSource.Play();
            estaEnNivel = true;
        }
        else if (!esNivel && estaEnNivel)
        {
            // Si no es un nivel y estábamos en un nivel, detener la música
            audioSource.Stop();
            estaEnNivel = false;
        }
    }

    void Start()
    {
        cantidadDeBloques = GameObject.FindGameObjectsWithTag("Destructible").Length;
        cantidadDeBloquesMax = cantidadDeBloques;

        // Verificar si ya estamos en un nivel al iniciar
        string nombreEscenaActual = SceneManager.GetActiveScene().name;
        if (nombreEscenaActual.StartsWith("Nivell") && !audioSource.isPlaying && musicaFondo != null)
        {
            audioSource.Play();
            estaEnNivel = true;
        }

        nextLevelImage.SetActive(false);

        GameManager.Instance.cupAppeared = false;
        soundplayed = false;
        triggered = false;
    }

    void Update()
    {
        if (Input.anyKeyDown) changeScene();
        cantidadDeBloques = GameObject.FindGameObjectsWithTag("Destructible").Length;
        int percentatgeBlocs = (cantidadDeBloques) * 100 / cantidadDeBloquesMax;

        if (percentatgeBlocs <= 5 && cantidadDeBloques > 0)
        {
            ActivateCupPowerUp?.Invoke();
        }
        if (cantidadDeBloques == 0)
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
        GameObject[] ball = GameObject.FindGameObjectsWithTag("Ball");
        if (ball != null)
        {
            foreach (GameObject b in ball)
            {
                Destroy(b);
            }
        }

        if (text != null)
        {
            text.SetActive(true);
            yield return StartCoroutine(FadeIn(text));

            if (sonidoWin != null && audioSource != null && !soundplayed)
            {
                soundplayed = true;
                audioSource.PlayOneShot(sonidoWin);
                yield return new WaitForSeconds(sonidoWin.length);
            }

            yield return new WaitForSeconds(3f);
            text.SetActive(false);
        }

        int escenaActual = SceneManager.GetActiveScene().buildIndex;
        int totalEscenas = SceneManager.sceneCountInBuildSettings;
        int siguienteEscena = (escenaActual + 1) % totalEscenas;
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