using UnityEngine;
using System;

public class CameraMovement : MonoBehaviour
{
    public float rotationSpeed = 30f;
    private float accumulatedAngle = 0f;
    private bool hasStartedGame = false;

    // Evento que se dispara al terminar la vuelta
    public event Action OnRotationComplete;

    void Update()
    {
        /**
        if (!hasStartedGame)
        {
            float angle = rotationSpeed * Time.deltaTime;
            transform.RotateAround(Vector3.zero, Vector3.up, angle);
            accumulatedAngle += angle;
            transform.LookAt(Vector3.zero);

            if (accumulatedAngle >= 360f)
            {
                hasStartedGame = true;
                OnRotationComplete?.Invoke(); // Lanza el callback
            }
        }*/
    }
}