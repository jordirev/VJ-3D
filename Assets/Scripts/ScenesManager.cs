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

    [Header("Música de Fondo")]
    [SerializeField] private AudioClip musicaFondo; 
    [SerializeField] private float volumenMusica = 0.5f;
    private AudioSource audioSource;
    private bool estaEnNivel = false;


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
