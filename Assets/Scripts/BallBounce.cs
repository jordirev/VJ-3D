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


    private Rigidbody rb;
    private Vector3 ultimaVelocidad;
    private int contadorRebotesPared = 0;
    private Vector3 ultimaNormalPared = Vector3.zero;
    public bool isPowerBallActive = false;
    private bool isMagnetActive = false;
    private bool isEnganchada = false;  
    private Transform paddleTransform;
    [SerializeField] private Vector3 offsetDesdePaleta = new Vector3(-0.5f, -0.4f, 0f);

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

        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Iniciar el movimiento
        rb.linearVelocity = direccionInicial.normalized * velocidadInicial;
        ultimaVelocidad = rb.linearVelocity;

        GameOverImage = GameObject.FindGameObjectWithTag("GameOverText");
        GameOverImage.SetActive(false);
    }

    private void Awake()
    {
        GameOverImage = GameObject.FindGameObjectWithTag("GameOverText");
        GameOverImage.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isEnganchada && paddleTransform != null)
        {
            // La bola sigue la paleta
            transform.position = paddleTransform.position + offsetDesdePaleta;

            // Esperar que el jugador pulse ESPACIO
            if (Input.GetKeyDown(KeyCode.Space))
            {
            //    isMagnetActive = false;
                isEnganchada = false;
                transform.SetParent(null);
                rb.linearVelocity = new Vector3(-1f, 0f, 0f).normalized * velocidadInicial;
            }

            return; // no aplicar lógica normal mientras esté enganchada
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

        GameObject objetoColisionado = collision.gameObject;

        if (objetoColisionado.CompareTag(tagBarra))
        {
            contadorRebotesPared = 0;
            Debug.Log("Choca barra");
            Debug.Log("Contador rebotes reiniciado por golpe a barra");

            if (isMagnetActive && !isEnganchada)
            {
                // Engancha la bola a la barra, pero no la lanza
                rb.linearVelocity = Vector3.zero;
                transform.position = paddleTransform.position + offsetDesdePaleta;
                transform.SetParent(paddleTransform);
                isEnganchada = true;
                Debug.Log("¡Bola enganchada a la barra por imán!");
            }
        }

        // Verificar si colisionó con una pared
        if (objetoColisionado.CompareTag(tagPared))
        {
            ManejarRebotePared(collision);
            return;
        }

        //PowerBall
        if (isPowerBallActive && collision.gameObject.CompareTag("Destructible"))
        {
            // Evitar rebote artificialmente: forzamos que no cambie la dirección
            Debug.Log("entra en PowerBall de BallBounce");
            return;
        }

        if (objetoColisionado.CompareTag("Limit"))
        {
            Debug.Log("Bola fuera de l�mites, reiniciando posici�n");
            if(gameObject.tag == "Ball")
                StartCoroutine(BallFallen());
            else if (gameObject.tag == "ExtraBall")
                Destroy(gameObject); 
            
            if(GameManager.Instance.vidas == 0)
            {
                Destroy(gameObject);
            }
        }

        float velocidad = ultimaVelocidad.magnitude;
        Vector3 direccion = Vector3.Reflect(ultimaVelocidad.normalized, collision.contacts[0].normal);
        rb.linearVelocity = direccion * Mathf.Max(velocidad, 0f);
    }
  
    private void ManejarRebotePared(Collision collision)
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

        Debug.Log($"Rebote en pared #{contadorRebotesPared}, Dirección: {direccionRebote}");
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

    private IEnumerator BallFallen()
    {
        if (GameManager.Instance.vidas != 0) transform.position = new Vector3(5f, 1f, 0f);
        yield return StartCoroutine(GameManager.Instance.UpdateLives(GameOverImage));
    }
}
