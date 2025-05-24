using UnityEditorInternal;
using UnityEngine;

public class SpawnPowerUp : MonoBehaviour
{
    public GameObject[] powerUpPrefabs;

    private float spwanProbability = 0.2f;
    private int cantidadDeBloques, cantidadDeBloquesMax;
    bool cupAppeared = false;

    private void Start()
    {
        cantidadDeBloques = GameObject.FindGameObjectsWithTag("Destructible").Length;
        cantidadDeBloquesMax = cantidadDeBloques;
    }
    void Update()
    {
        cantidadDeBloques = GameObject.FindGameObjectsWithTag("Destructible").Length;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball") || collision.gameObject.CompareTag("Bullet"))
        {
            int indexPrefab = -1;

            if ((float)cantidadDeBloques / cantidadDeBloquesMax <= 0.05f && !cupAppeared)
            {
                cupAppeared = true;
                indexPrefab = 0; // Cup prefab
            }
            else if (Random.value < spwanProbability)
            {
                indexPrefab = Random.Range(0, powerUpPrefabs.Length);
            }

            if (indexPrefab >= 0 && indexPrefab < powerUpPrefabs.Length)
            {
                GameObject powerUpPrefab = powerUpPrefabs[indexPrefab];
                Vector3 spawnPosition = collision.contacts[0].point;
                Instantiate(powerUpPrefab, spawnPosition, Quaternion.identity);
            }

            //Destruim el bloc amb el que s'ha colisionatm amb la pilota o la bala
            Destroy(gameObject);
            ScoreManager.instance.AddPoints(500);
        }
    }
}
