using UnityEngine;

public class BarMovement : MonoBehaviour
{
    // Velocidad de movimiento del objeto
    [SerializeField] private float velocidad = 5.0f;

    // Variable para controlar si el objeto puede moverse
    private bool puedeMoverse = true;

    void Update()
    {
        if (!puedeMoverse) return;

        // Obtener la entrada horizontal (flechas izquierda/derecha)
        float movimientoHorizontal = Input.GetAxis("Horizontal");

        // Crear el vector de movimiento (aplicado al eje Z en lugar del X)
        Vector3 movimiento = new Vector3(-movimientoHorizontal, 0, 0);

        // Mover el objeto
        transform.Translate(movimiento * velocidad * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Verificar si el objeto colisionado tiene la etiqueta "walls"
        if (collision.gameObject.CompareTag("Walls"))
        {
            puedeMoverse = false;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // Permitir movimiento nuevamente cuando deje de colisionar con "walls"
        if (collision.gameObject.CompareTag("Walls"))
        {
            puedeMoverse = true;
        }
    }
}