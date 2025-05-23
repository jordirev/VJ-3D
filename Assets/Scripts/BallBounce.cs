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

    }

    // Update is called once per frame
    void Update()
    {
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
            Debug.Log("Contador rebotes reiniciado por golpe a barra");
        }

        // Verificar si colisionó con una pared
        if (objetoColisionado.CompareTag(tagPared))
        {
            ManejarRebotePared(collision);
            return;
        }

        // Verificar si el objeto debe ser destruido (no está en la lista de tags no destruibles)

     /*   if (objetoColisionado.CompareTag(tagDestruible))
        {

            DestruirObjetoYManejarObjetosSuperiores(objetoColisionado);
            ScoreManager.instance.AddPoints(500);

        }*/

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

    
  /*  private void DestruirObjetoYManejarObjetosSuperiores(GameObject objetoColisionado)
    {
        Debug.Log("Destruyendo: " + objetoColisionado.name);

        Destroy(objetoColisionado, 0.1f);


        // Buscar objetos que están encima
        Collider[] objetosSuperior = Physics.OverlapBox(
            objetoColisionado.transform.position + Vector3.up * 1.1f,
            objetoColisionado.GetComponent<Collider>().bounds.extents,
            objetoColisionado.transform.rotation);


        // Para cada objeto encontrado encima, habilitar gravedad
        foreach (Collider col in objetosSuperior)
        {
            if (col.gameObject != objetoColisionado && col.gameObject != gameObject)
            {
                Rigidbody objetoRb = col.GetComponent<Rigidbody>();

                // Si no tiene Rigidbody, añadirlo para que caiga
                if (objetoRb == null)
                {
                    objetoRb = col.gameObject.AddComponent<Rigidbody>();
                }

                // Guardar la posición X y Z original
                Vector3 posicionOriginal = col.gameObject.transform.position;
                float posX = posicionOriginal.x;
                float posZ = posicionOriginal.z;

                // Activar gravedad para que caiga
                objetoRb.useGravity = true;
                objetoRb.isKinematic = false;

                // Congelar posición en X y Z para que solo se mueva en Y
                objetoRb.constraints = RigidbodyConstraints.FreezePositionX |
                                      RigidbodyConstraints.FreezePositionZ |
                                      RigidbodyConstraints.FreezeRotation;

                // Aumentar la masa para que caiga rápido y sin efectos secundarios
                objetoRb.mass = 10f;

                objetoRb.linearDamping = 0.5f;

                // Detección de colisiones continua para evitar atravesar el suelo
                objetoRb.collisionDetectionMode = CollisionDetectionMode.Continuous;


                PhysicsMaterial material = new PhysicsMaterial("LowBounce");
                material.bounciness = 0.0f; // Muy poco rebote
                material.dynamicFriction = 1.0f; // Bastante fricción
                material.staticFriction = 1.0f;
                col.sharedMaterial = material;


                Debug.Log("Objeto que cae: " + col.gameObject.name);
            }
        }
    }*/
}
