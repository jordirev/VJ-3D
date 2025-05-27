using UnityEditorInternal;
using UnityEngine;

public class SpawnPowerUp : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject[] powerUpPrefabs;
    public GameObject cupPrefab;

    bool activateCup = false;
    bool cupAppeared = false;

    private float spwanProbability = 0.2f;
   

    private void Start()
    {
        ScenesManager SceneMngr = Object.FindFirstObjectByType<ScenesManager>();
        if (SceneMngr != null)
        {
            SceneMngr.ActivateCupPowerUp += OnActivateCupPowerUp;
        }
    }

    private void OnDestroy()
    {
        ScenesManager SceneMngr = Object.FindFirstObjectByType<ScenesManager>();
        if (SceneMngr != null)
        {
            SceneMngr.ActivateCupPowerUp -= OnActivateCupPowerUp;
        }
    }

    private void OnActivateCupPowerUp()
    {
        if (!cupAppeared) activateCup = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball") || collision.gameObject.CompareTag("Bullet"))
        {
            if (activateCup && !cupAppeared)
            {
                cupAppeared = true;
                Vector3 spawnPosition = collision.contacts[0].point;
                Instantiate(cupPrefab, spawnPosition, Quaternion.identity);
            }
            else if (Random.value < spwanProbability)
            {
                int indexPrefab = Random.Range(0, powerUpPrefabs.Length);
                GameObject powerUpPrefab = powerUpPrefabs[indexPrefab];
                Vector3 spawnPosition = collision.contacts[0].point;
                Instantiate(powerUpPrefab, spawnPosition, Quaternion.identity);
            }

            //Destruim el bloc amb el que s'ha colisionatm amb la pilota o la bala
            Destroy(gameObject);
            GameManager.Instance.AddPoints(500);
        }
    }
}
