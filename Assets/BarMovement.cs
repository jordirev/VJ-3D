using UnityEngine;
using UnityEngine.InputSystem;

public class BarMovement : MonoBehaviour
{
    public float speed = 5f; // Velocidad de movimiento
    private Vector2 movementInput;

    void Update()
    {
        // Calcular el movimiento lateral
        Vector3 movement = new Vector3(movementInput.x, 0, 0) * speed * Time.deltaTime;

        // Aplicar el movimiento al objeto
        transform.Translate(movement);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        // Capturar la entrada del nuevo sistema de entrada
        movementInput = context.ReadValue<Vector2>();
    }
}
