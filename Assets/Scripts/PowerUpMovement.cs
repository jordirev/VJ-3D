using UnityEngine;

public class PowerUpMovement : MonoBehaviour
{
    public float speed = 3f; // Velocidad en unidades por segundo

    void Update()
    {
        // Mueve el objeto en el eje Z (adelante)
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("PowerUp recogido por el jugador");
            // Aquí puedes añadir la lógica que quieras al recoger el power-up
            //Destroy(gameObject); 
        }
    }
}
