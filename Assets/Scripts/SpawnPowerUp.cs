using UnityEditorInternal;
using UnityEngine;

public class SpawnPowerUp : MonoBehaviour
{
    public GameObject[] powerUpPrefabs;
    public GameObject cupPrefab;

    bool activateCup = false;
    [Header("Efectos de Destrucción")]
    public GameObject efectoDesintegracionPrefab; // Prefab con el efecto de desintegración
    public float duracionEfectoDesintegracion = 1.5f;

    private float spwanProbability = 0.2f;

    private bool cupAppeared = false;

    private void Start()
    {

        ScenesManager SceneMngr = Object.FindFirstObjectByType<ScenesManager>();
        if (SceneMngr != null)
        {
            SceneMngr.ActivateCupPowerUp += OnActivateCupPowerUp;
        }
    }

    private void Awake()
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

            Vector3 posicion = transform.position;
            Quaternion rotacion = transform.rotation;
            Vector3 escala = transform.localScale;

            // efecto desintegracion
            if (efectoDesintegracionPrefab != null)
            {
                GameObject efectoInstancia = Instantiate(
                    efectoDesintegracionPrefab,
                    posicion,
                    rotacion
                );

                // Ajustar la escala del efecto para que coincida con el objeto destruido
                efectoInstancia.transform.localScale = escala;

                // Destruir el efecto después de un tiempo
                Destroy(efectoInstancia, (duracionEfectoDesintegracion - 1f));
            }

            if (activateCup)
            {
                Vector3 spawnPosition = collision.contacts[0].point;
                spawnPosition.y += 0.5f;
                Instantiate(cupPrefab, spawnPosition, Quaternion.identity);
            }
            else if (Random.value < spwanProbability)
            {
                int indexPrefab = Random.Range(0, powerUpPrefabs.Length);
                GameObject powerUpPrefab = powerUpPrefabs[indexPrefab];
                Vector3 spawnPosition = collision.contacts[0].point;
                spawnPosition.y += 0.5f;
                Instantiate(powerUpPrefab, spawnPosition, Quaternion.identity);
            }

            //Destruim el bloc amb el que s'ha colisionatm amb la pilota o la bala
            Destroy(gameObject);
            GameManager.Instance.AddPoints(500);
        }
    }
}
