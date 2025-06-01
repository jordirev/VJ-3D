using UnityEngine;
using TMPro;
using System.Collections;

public class GodModeWall : MonoBehaviour
{
    public GameObject GodMode;
    public TextMeshProUGUI godModeText;
    public TextMeshProUGUI TempoGodMode;

    private bool isGodModeActive = false;
    private bool temporizador = false;
    private Coroutine godModeCoroutine;


    void Start()
    {
        if (GodMode != null) GodMode.SetActive(false);
       
        if (godModeText != null) godModeText.gameObject.SetActive(false);
      
        if (TempoGodMode != null) TempoGodMode.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (temporizador) temporizador = false;
            else isGodModeActive = !isGodModeActive;

            if (GodMode != null) GodMode.SetActive(isGodModeActive);          

            if (godModeText != null) godModeText.gameObject.SetActive(isGodModeActive);           

            if (TempoGodMode != null) TempoGodMode.gameObject.SetActive(false); // No mostrar temporizador si se activa manualmente

            if (isGodModeActive)
            {
                MoverBolasEnPared();
            }
        }
    }

    public void ActivarGodModeTemporal(float duracion)
    {
        if(godModeCoroutine != null)
        {
            StopCoroutine(godModeCoroutine);
        }

        MoverBolasEnPared();

        godModeCoroutine = StartCoroutine(GodModeConTemporizador(duracion));
    }

    private IEnumerator GodModeConTemporizador(float segundos)
    {
        isGodModeActive = true;
        temporizador = true;

        if (GodMode != null) GodMode.SetActive(true);
        if (godModeText != null) godModeText.gameObject.SetActive(true);
        if (TempoGodMode != null) TempoGodMode.gameObject.SetActive(true);

        int tiempoRestante = Mathf.CeilToInt(segundos);
        while (tiempoRestante > 0)
        {
            if (TempoGodMode != null)
            {
                TempoGodMode.text = "Timer: " + tiempoRestante + "s";
            }

            yield return new WaitForSeconds(1f);
            tiempoRestante--;
        }

        isGodModeActive = false;
        temporizador = false;

        if (GodMode != null) GodMode.SetActive(false);
        if (godModeText != null) godModeText.gameObject.SetActive(false);
        if (TempoGodMode != null) TempoGodMode.gameObject.SetActive(false);

        godModeCoroutine = null;
    }

    public bool IsGodModeActive
    {
        get { return isGodModeActive; }
    }

    private void MoverBolasEnPared()
    {
        if (GodMode == null) return;

        // Obtener la posici�n y dimensiones de la pared del GodMode
        Collider wallCollider = GodMode.GetComponent<Collider>();
        if (wallCollider == null) return;

        Bounds wallBounds = wallCollider.bounds;

        // Buscar todas las bolas en la escena
        GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");

        foreach (GameObject ball in balls)
        {
            // Comprobar si la bola est� dentro o muy cerca de la pared
            if (wallBounds.Contains(ball.transform.position) ||
                Vector3.Distance(ball.transform.position, wallBounds.ClosestPoint(ball.transform.position)) < 0.5f)
            {
                // Mover la bola hacia (-1,0,0) para sacarla de la pared
                ball.transform.position += new Vector3(-3f, 0f, 0f);

                // Si la bola tiene un Rigidbody, asegurarse de que siga movi�ndose
                Rigidbody rb = ball.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    if (rb.linearVelocity.magnitude < 2f || rb.linearVelocity.x > 0)
                    {
                        BallBounce ballScript = ball.GetComponent<BallBounce>();
                        float velocidad = (ballScript != null) ? 10f : 10f;
                        rb.linearVelocity = new Vector3(-1f, 0f, 0f).normalized * velocidad;
                    }
                }

                Debug.Log("Bola desplazada para evitar quedar atrapada en la pared del GodMode");
            }
        }
    }
}
