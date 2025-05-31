using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;
    public float fireRate = 1f; // Dispara cada 1 segundo

    private float nextFireTime = 0f;

    void Update()
    {
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        // Efecte de retrocés (recoil)
        StartCoroutine(RecoilEffect());

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.linearVelocity = firePoint.forward * bulletSpeed;

        // Añadir el script BulletDestruction si no existe
        if (bullet.GetComponent<BulletDestruction>() == null)
        {
            bullet.AddComponent<BulletDestruction>();
        }

        // Opcional: destruir la bala después de unos segundos
        Destroy(bullet, 1.0f);
    }

    private IEnumerator RecoilEffect()
    {
        Vector3 originalPosition = transform.localPosition;
        Vector3 recoilOffset = -transform.right * 0.1f; // Canvia a l'eix X si la pistola apunta en X
        float recoilTime = 0.05f;
        float returnTime = 0.1f;

        float elapsed = 0f;
        while (elapsed < recoilTime)
        {
            transform.localPosition = Vector3.Lerp(originalPosition, originalPosition + recoilOffset, elapsed / recoilTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originalPosition + recoilOffset;

        elapsed = 0f;
        while (elapsed < returnTime)
        {
            transform.localPosition = Vector3.Lerp(originalPosition + recoilOffset, originalPosition, elapsed / returnTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originalPosition;
    }
}


public class BulletDestruction : MonoBehaviour
{
    void Start()
    {
        transform.Rotate(90f, 0f, 0f);
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Destructible"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
            GameManager.Instance.AddPoints(500);
        }
        else if (collision.gameObject.CompareTag("Walls"))
        {
            Destroy(gameObject);
        }
    }
}
