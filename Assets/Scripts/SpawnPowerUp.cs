using UnityEngine;

public class SpawnPowerUp : MonoBehaviour
{
    public GameObject[] powerUpPrefabs;

    private float Spwanprobability = 0.2f;
    private int cantidadDeBloques;

    private void Start()
    {
        cantidadDeBloques = GameObject.FindGameObjectsWithTag("Bloque").Length;
    }
    void Update()
    {
        cantidadDeBloques = GameObject.FindGameObjectsWithTag("Bloque").Length;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball") || collision.gameObject.CompareTag("Bullet"))
        {
            // Check if the object should spawn a power-up
            if (Random.value < Spwanprobability)
            {
                // Randomly select a power-up prefab
                int randomIndex = Random.Range(0, powerUpPrefabs.Length);
                GameObject powerUpPrefab = powerUpPrefabs[randomIndex];
                Vector3 spawnPosition = collision.contacts[0].point;
                Instantiate(powerUpPrefab, spawnPosition, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }
}
