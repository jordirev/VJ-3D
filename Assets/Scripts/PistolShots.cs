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
        //Quaternion rotation = firePoint.rotation * Quaternion.Euler(0, 0, 0);
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
        }
    }
}
