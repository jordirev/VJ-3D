using Unity.VisualScripting;
using UnityEngine;

public class TreeCutMovement : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody rb;
    private float moveInput;

    private bool canMoveLeft = true;  //canviar els dos a false
    private bool canMoveRight = true;

    public GameObject pistolPrefab;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Busca la cámara y se suscribe al evento
        /**CameraMovement cam = Object.FindFirstObjectByType<CameraMovement>();
        if (cam != null)
        {
            cam.OnRotationComplete += EnableMovement;
        }*/

        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionX;
        rb.useGravity = false;
        rb.isKinematic = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    void Update()
    {
        moveInput = Input.GetAxis("Horizontal");
    }

    void FixedUpdate()
    {
        if ((moveInput < 0 && canMoveLeft) || (moveInput > 0 && canMoveRight))
        {
            Vector3 movement = new Vector3(0, 0, moveInput) * speed * Time.fixedDeltaTime;
            Vector3 newPosition = rb.position + movement;

            // Limita el movimiento según valores observados
            newPosition.z = Mathf.Clamp(newPosition.z, -6.248f, 5.744f);

            rb.MovePosition(newPosition);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Walls"))
        {
            Vector3 contactPoint = collision.GetContact(0).point;
            Vector3 direction = (contactPoint - transform.position).normalized;

            if (direction.z > 0) // Wall is in front (moving forward)
            {
                canMoveRight = false;
            }
            else if (direction.z < 0) // Wall is behind (moving backward)
            {
                canMoveLeft = false;
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Walls"))
        {
            // Al salir del muro, permitir movimiento en ambas direcciones
            canMoveLeft = true;
            canMoveRight = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        /**
         * Principalment els codis dels PowerUps
         */

        if (other.CompareTag("BigArrow"))
        {
            Vector3 newScale = transform.localScale;
            newScale.x *= 1.25f;
            transform.localScale = newScale;
            Debug.Log("PowerUp utilizado por el jugador");
        }

        if (other.CompareTag("SmallArrow"))
        {
            Vector3 newScale = transform.localScale;
            newScale.x *= 0.75f;
            transform.localScale = newScale;
            Debug.Log("PowerUp utilizado por el jugador");
        }

        if (other.CompareTag("Pistols"))
        {
            // Obtener el BoxCollider del tree_cut
            MeshCollider mesh = GetComponent<MeshCollider>();
            if (mesh != null)
            {
                // Calcular los extremos en espacio local usando bounds del MeshCollider
                Bounds bounds = mesh.sharedMesh.bounds;
                Vector3 leftLocal = new Vector3(bounds.min.x +1, 0.1f, 0);
                Vector3 rightLocal = new Vector3(bounds.max.x -1, 0.1f, 0);

                // Convertir a espacio global
                Vector3 leftWorld = transform.TransformPoint(leftLocal);
                Vector3 rightWorld = transform.TransformPoint(rightLocal);

                // Instanciar la primera pistola en el extremo izquierdo
                GameObject pistol1 = Instantiate(
                    pistolPrefab,
                    leftWorld,
                    transform.rotation,
                    transform
                );
                Destroy(pistol1, 10000f); // CAAAAAAAAAAAAAANVIAR A 20 

                // Instanciar la segunda pistola en el extremo derecho
                GameObject pistol2 = Instantiate(
                    pistolPrefab,
                    rightWorld,
                    transform.rotation,
                    transform
                );
                Destroy(pistol2, 10000); // CAAAAAAAAAAAAAAAAAAAANVIAR A 20 
                Debug.Log("PowerUp utilizado por el jugador");
            }
        }
    }

    public void EnableMovement()
    {
        canMoveLeft = true;
        canMoveRight = true;
    }
}
