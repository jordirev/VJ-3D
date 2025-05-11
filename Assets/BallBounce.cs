using UnityEngine;

public class BallBounce : MonoBehaviour
{
    [Header("Configuraci�n de Movimiento")]
    [SerializeField] private float velocidadInicial = 10f;
    [SerializeField] private float fuerzaRebote = 1.2f;
    [SerializeField] private Vector3 direccionInicial = new Vector3(1f, 0f, 1f);

    [Header("L�mites")]
    [SerializeField] private float velocidadMaxima = 15f;

    [Header("Interacción con objetos")]
    [SerializeField] private string tagDestruible = "Destructible"; // Tags de objetos que no se destruyen


    private Rigidbody rb;


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


        // Iniciar el movimiento
        rb.linearVelocity = direccionInicial.normalized * velocidadInicial;

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

    }

    // Se llama cuando hay una colisi�n
    private void OnCollisionEnter(Collision collision)
    {

        GameObject objetoColisionado = collision.gameObject;

        // Verificar si el objeto debe ser destruido (no está en la lista de tags no destruibles)

        if (objetoColisionado.CompareTag(tagDestruible))
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

        }


        // Calcular la direcci�n del rebote
        Vector3 direccionRebote = Vector3.Reflect(rb.linearVelocity.normalized, collision.contacts[0].normal);

        // Aplicar el rebote con la fuerza configurada
        rb.linearVelocity = direccionRebote * (rb.linearVelocity.magnitude * fuerzaRebote);

    }
}
