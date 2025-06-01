using System.Collections;
using UnityEngine;

public class BallBounce : MonoBehaviour
{
    [Header("Configuraci�n de Movimiento")]
    [SerializeField] private float velocidadInicial = 10f;
    [SerializeField] private float fuerzaRebote = 1.2f;
    [SerializeField] private Vector3 direccionInicial = new Vector3(1f, 0f, 0f);

    [Header("L�mites")]
    [SerializeField] private float velocidadMaxima = 8f;

    [Header("Interacción con objetos")]
    [SerializeField] private string tagDestruible = "Destructible"; // Tags de objetos que no se destruyen
    [SerializeField] private string tagPared = "Walls"; // Tag de las paredes
    [SerializeField] private string tagBarra = "Player";


    [Header("Anti-bucle")]
    [SerializeField] private int maxRebotesPared = 4; // Número máximo de rebotes en pared consecutivos
    [SerializeField] private float factorPerturbacion = 0.3f; // Factor de perturbación del ángulo

    [Header("Audio")]
    [SerializeField] private AudioClip sonidoReboteBarra;
    [SerializeField] private float volumenSonido = 1.0f;
    [SerializeField] private AudioClip sonidoDestruir;
    [SerializeField] private float volumenSonido2 = 1.0f;
    [SerializeField] private AudioClip sonidoPared;
    [SerializeField] private float volumenSonido3 = 1.0f;


    private Rigidbody rb;
    private Vector3 ultimaVelocidad;
    private int contadorRebotesPared = 0;
    private Vector3 ultimaNormalPared = Vector3.zero;
    public bool isPowerBallActive = false;
    private bool isMagnetActive = false;
    private bool isEnganchada = false;
    private Transform paddleTransform;
    [SerializeField] private Vector3 offsetDesdePaleta = new Vector3(-0.5f, -0.4f, 0f);
    private float tiempoDesenganche = 0f;
    private float retardoReenganche = 0.2f;
    private AudioSource audioSource;

    private GameObject GameOverImage;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Si no tiene un Rigidbody, a�adir uno
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();

        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Iniciar el movimiento
        rb.linearVelocity = direccionInicial.normalized * velocidadInicial;
        ultimaVelocidad = rb.linearVelocity;

        GameOverImage = GameObject.FindGameObjectWithTag("GameOverText");
        if (GameOverImage != null)
        {
            GameOverImage.SetActive(false);
        }
    }

    private void Awake()
    {
        GameOverImage = GameObject.FindGameObjectWithTag("GameOverText");
        GameOverImage.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (tiempoDesenganche > 0f)
            tiempoDesenganche -= Time.deltaTime;

        if (isEnganchada)
        {
            // La bola sigue la paleta
            transform.position = paddleTransform.position + offsetDesdePaleta;

            // Desactivar física y colisiones mientras está enganchada
            rb.isKinematic = true;
            GetComponent<Collider>().enabled = false;

            // Esperar que el jugador pulse ESPACIO
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isMagnetActive = false;
                isEnganchada = false;
                rb.isKinematic = false;
                GetComponent<Collider>().enabled = true;
                rb.linearVelocity = Vector3.left * velocidadInicial;
                tiempoDesenganche = retardoReenganche; // Inicia el retardo
            }

            return; // no aplicar lógica normal mientras esté enganchada
        }
        else
        {
            rb.isKinematic = false;
            GetComponent<Collider>().enabled = true;
        }


        if (rb.linearVelocity.magnitude < velocidadInicial)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * velocidadInicial;
        }

        // Limitar la velocidad m�xima
        if (rb.linearVelocity.magnitude > velocidadMaxima)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * velocidadMaxima;
        }

        ultimaVelocidad = rb.linearVelocity;

    }

    // Se llama cuando hay una colisi�n
    private void OnCollisionEnter(Collision collision)
    {
        if (isEnganchada) return;

        GameObject objetoColisionado = collision.gameObject;

        if (objetoColisionado.CompareTag(tagBarra))
        {
            contadorRebotesPared = 0;
            Debug.Log("Choca barra");
            Debug.Log("Contador rebotes reiniciado por golpe a barra");

            ReproducirSonidoRebote();

            if (isMagnetActive && !isEnganchada && tiempoDesenganche <= 0f)
            {
                rb.linearVelocity = Vector3.zero;
                transform.position = paddleTransform.position + offsetDesdePaleta;
                isEnganchada = true;
                Debug.Log("¡Bola enganchada a la barra por imán!");
                return;
            }

            // Calcular la nueva dirección de rebote
            // Obtener la posición de contacto y la posición de la barra


            ContactPoint contacto = collision.contacts[0];
            float anchoBarra = objetoColisionado.GetComponent<Collider>().bounds.size.x;
            float posicionRelativa = (contacto.point.x - objetoColisionado.transform.position.x) / (anchoBarra / 2f);

            if (Mathf.Abs(contacto.normal.x) > 0.9f)
            {
                // Forzar la dirección hacia la izquierda 
                float anguloDesvio = Random.Range(-60f, 60f) * Mathf.Deg2Rad; 
                Vector3 direccionRebote = new Vector3(
                    -Mathf.Cos(anguloDesvio), 
                    Mathf.Sin(anguloDesvio),  
                    0f                        
                );

                // Mantener la velocidad actual
                rb.linearVelocity = direccionRebote.normalized * ultimaVelocidad.magnitude;
            }
            else
            {
                float anguloRebote = 0;

                // Limitar el valor entre -1 y 1
                posicionRelativa = Mathf.Clamp(posicionRelativa, -1f, 1f);
                float anguloMaximo = 180f * Mathf.Deg2Rad;

                // Determinar región de impacto
                if (posicionRelativa < -0.33f) // izquierda
                {
                    anguloRebote = -anguloMaximo; 
                }
                else if (posicionRelativa > 0.33f) // derecha
                {
                    anguloRebote = anguloMaximo; 
                }
                else // centro
                {
                    anguloRebote = 0f;
                }

                // Calcular nueva dirección 
                Vector3 nuevaDireccion = new Vector3(
                    -1f,                        
                    Mathf.Sin(anguloRebote),   
                    Mathf.Cos(anguloRebote)    
                );

                // Aplicar velocidad manteniendo la magnitud
                rb.linearVelocity = nuevaDireccion.normalized * ultimaVelocidad.magnitude;
            }
        }

        // Verificar si colisionó con una pared
        if (objetoColisionado.CompareTag(tagPared))
        {
            ReproducirSonidoPared();
            Debug.Log("toca pared");
            ManejarReboteInfinitoPared(collision);
            return;
        }

        if (collision.gameObject.CompareTag("Destructible"))
        {
            ReproducirSonidoDestruir();

            //PowerBall
            if (isPowerBallActive)
            {
                // Evitar rebote artificialmente: forzamos que no cambie la dirección
                Debug.Log("entra en PowerBall de BallBounce");
                return;
            }
        }

        if (objetoColisionado.CompareTag("Limit"))
        {
            Debug.Log("Bola fuera de l�mites, reiniciando posici�n");
            if (gameObject.tag == "Ball") BallFallen();
            else if (gameObject.tag == "ExtraBall")
                Destroy(gameObject);

            if (GameManager.Instance.vidas == 0)
            {
                Destroy(gameObject);
            }
        }

        float velocidad = ultimaVelocidad.magnitude;
        Vector3 direccion = Vector3.Reflect(ultimaVelocidad.normalized, collision.contacts[0].normal);
        rb.linearVelocity = direccion * Mathf.Max(velocidad, 0f);
    }
  
    private void ManejarReboteInfinitoPared(Collision collision)
    {
        // Incrementar contador de rebotes en pared
        contadorRebotesPared++;

        // Obtener la normal de la colisión
        Vector3 normal = collision.contacts[0].normal;

        // Verificar si es un rebote repetitivo (misma normal pero en sentido contrario)
        bool reboteRepetitivo = Vector3.Dot(normal, ultimaNormalPared) < -0.9f;

        // Guardar la normal actual para la próxima comparación
        ultimaNormalPared = normal;

        // Calcular la dirección del rebote
        Vector3 direccionRebote = Vector3.Reflect(ultimaVelocidad.normalized, normal);
        float velocidad = ultimaVelocidad.magnitude;

        // Si se detecta un posible bucle, perturbar el ángulo
        if (contadorRebotesPared >= maxRebotesPared && reboteRepetitivo)
        {
            // Aplicar perturbación para romper el bucle
            direccionRebote = PerturbacionAngulo(direccionRebote);

            // Mostrar mensaje de depuración
            Debug.Log($"¡BUCLE DETECTADO! Contador: {contadorRebotesPared}, Aplicando perturbación");

            // Opcional: reiniciar contador después de aplicar perturbación
            contadorRebotesPared = 0;
        }

        // Aplicar la nueva velocidad
        rb.linearVelocity = direccionRebote * Mathf.Max(velocidad, velocidadInicial);
    }

    private Vector3 PerturbacionAngulo(Vector3 direccion)
    {
        // Crear un vector de perturbación que modifica la dirección original
        // La perturbación es más fuerte en Y para forzar un cambio en el ángulo vertical
        Vector3 perturbacion = new Vector3(
            Random.Range(-factorPerturbacion, factorPerturbacion),
            0,
            Random.Range(0.2f, factorPerturbacion)  // Forzar perturbación hacia abajo/arriba
        );

        // Aplicar perturbación a la dirección original
        Vector3 nuevaDireccion = direccion + perturbacion;

        // Normalizar para mantener la magnitud
        return nuevaDireccion.normalized;
    }

    public void ActivarIman(Transform paddle)
    {
        isMagnetActive = true;
        paddleTransform = paddle;
    }

    private void BallFallen()
    {
        if (GameManager.Instance.vidas != 0)
        {
            transform.position = new Vector3(5f, 1f, 0f);
            rb.linearVelocity = new Vector3(-1f, 0f, 0f).normalized * velocidadInicial;
        }
        GameManager.Instance.LoseLife();
    }

    private void ReproducirSonidoRebote()
    {
        if (sonidoReboteBarra != null && audioSource != null)
        {
            audioSource.volume = volumenSonido;
            audioSource.PlayOneShot(sonidoReboteBarra);
        }
        else
        {
            Debug.LogWarning("Error sonido choque pelota-barra");
        }
    }

    private void ReproducirSonidoDestruir()
    {
        if (sonidoDestruir != null && audioSource != null)
        {
            audioSource.volume = volumenSonido2;
            audioSource.PlayOneShot(sonidoDestruir);
        }
        else
        {
            Debug.LogWarning("Error sonido");
        }
    }

    private void ReproducirSonidoPared()
    {
        if (sonidoPared != null && audioSource != null)
        {
            audioSource.volume = volumenSonido3;
            audioSource.PlayOneShot(sonidoPared);
        }
        else
        {
            Debug.LogWarning("Error sonido");
        }
    }
}
