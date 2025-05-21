using UnityEngine;

public class PowerUpMovement : MonoBehaviour
{
    public float speed = 3f; // Velocidad en unidades por segundo

    private void Start()
    {
        // rotar 90 grados en el eje Y
        transform.Rotate(0, 90, 0);
    }
    void Update()
    {
        //mover objeto hacia delante eje Z
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("PowerUp recogido por el jugador");
            // Aquí puedes añadir la lógica que quieras al recoger el power-up
            Destroy(gameObject); 
        }
    }
}
