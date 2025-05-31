using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TreeCutMovement : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody rb;
    private float moveInput;

    private bool canMoveLeft = true;  //canviar els dos a false
    private bool canMoveRight = true;

    public GameObject pistolPrefab;

    public GameObject prefabBall;

    public GameObject nextLevelText;

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
            newScale.x = Mathf.Clamp(newScale.x, 1.3f, 2.9f);
            transform.localScale = newScale;
            Debug.Log("PowerUp utilizado por el jugador");

            GameManager.Instance.AddPoints(800);
        }

        if (other.CompareTag("SmallArrow"))
        {
            Vector3 newScale = transform.localScale;
            newScale.x *= 0.85f;
            newScale.x = Mathf.Clamp(newScale.x, 1.3f, 2.9f);
            transform.localScale = newScale;
            Debug.Log("PowerUp utilizado por el jugador");

            GameManager.Instance.AddPoints(800);
        }

        if (other.CompareTag("NextLevel"))
        {
            // Mostrar el texto de "Next Level" durante 2 segundos
            if (nextLevelText != null)
            {
                nextLevelText.SetActive(true);
                Invoke("HideNextLevelText", 2f);
            }
            int escenaActual = SceneManager.GetActiveScene().buildIndex;
            int totalEscenas = SceneManager.sceneCountInBuildSettings;
            int siguienteEscena = (escenaActual + 1) % totalEscenas;
            SceneManager.LoadScene(siguienteEscena);
            Debug.Log("PowerUp utilizado por el jugador");

            GameManager.Instance.AddPoints(1000);
        }

        if (other.CompareTag("Pistols"))
        {
            // Obtener el BoxCollider del tree_cut
            MeshCollider mesh = GetComponent<MeshCollider>();
            if (mesh != null)
            {
                // Calcular los extremos en espacio local usando bounds del MeshCollider
                Bounds bounds = mesh.sharedMesh.bounds;
                Vector3 leftLocal = new Vector3(bounds.min.x + 1, 0.1f, 0);
                Vector3 rightLocal = new Vector3(bounds.max.x - 1, 0.1f, 0);

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

            GameManager.Instance.AddPoints(800);
        }

        if (other.CompareTag("PowerBallOn"))
        {
            GameObject ball = GameObject.FindGameObjectWithTag("Ball");
            if (ball != null)
            {
                BallBounce ballScript = ball.GetComponent<BallBounce>();
                if (ballScript != null)
                {
                    ballScript.isPowerBallActive = true;
                }
            }

            Destroy(other.gameObject);

            GameManager.Instance.AddPoints(1000);
        }

        if (other.CompareTag("PowerBallOff"))
        {
            GameObject ball = GameObject.FindGameObjectWithTag("Ball");
            if (ball != null)
            {
                BallBounce ballScript = ball.GetComponent<BallBounce>();
                if (ballScript != null)
                {
                    ballScript.isPowerBallActive = false;
                }
            }

            Destroy(other.gameObject);

            GameManager.Instance.AddPoints(800);
        }

        if (other.CompareTag("ExtraBalls"))
        {
            GameObject mainBall = GameObject.FindGameObjectWithTag("Ball");
            if (mainBall != null)
            {
                Vector3 spawnPosition = mainBall.transform.position;

                GameObject ball1 = Instantiate(prefabBall, spawnPosition, Quaternion.identity);
                GameObject ball2 = Instantiate(prefabBall, spawnPosition, Quaternion.identity);
                Debug.Log("Aparecen extra balls");

                Rigidbody mainRb = mainBall.GetComponent<Rigidbody>();
                Vector3 direccionPrincipal = mainRb.linearVelocity.normalized;

                // Rotamos la dirección un poco a la izquierda y a la derecha (15 grados)
                Vector3 direccion1 = Quaternion.Euler(0, -15f, 0) * direccionPrincipal;
                Vector3 direccion2 = Quaternion.Euler(0, 15f, 0) * direccionPrincipal;

                float velocidad = 10f;

                ball1.GetComponent<Rigidbody>().linearVelocity = direccion1 * velocidad;
                ball2.GetComponent<Rigidbody>().linearVelocity = direccion2 * velocidad;

                Destroy(other.gameObject);
            }

            GameManager.Instance.AddPoints(1000);
        }

        if (other.CompareTag("Magnet"))
        {
            GameObject ball = GameObject.FindGameObjectWithTag("Ball");

            if (ball != null)
            {
                BallBounce ballScript = ball.GetComponent<BallBounce>();
                if (ballScript != null)
                {
                    ballScript.ActivarIman(transform); // le pasas tu Transform de la paleta
                }
            }

            Destroy(other.gameObject);

            GameManager.Instance.AddPoints(1000);
        }

        if (other.CompareTag("PUgodMode"))
        {
            GameObject godModeWall = GameObject.Find("GameManager"); 
            if (godModeWall != null)
            {
                GodModeWall gmScript = godModeWall.GetComponent<GodModeWall>();
                if (gmScript != null)
                {
                    if (!gmScript.IsGodModeActive) gmScript.ActivarGodModeTemporal(15f);
                }
            }

            Destroy(other.gameObject);

            GameManager.Instance.AddPoints(1000);
        }
    }

    public void EnableMovement()
    {
        canMoveLeft = true;
        canMoveRight = true;
    }
}
