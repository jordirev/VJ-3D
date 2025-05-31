using UnityEngine;
using TMPro;
using System.Collections;

public class GodModeWall : MonoBehaviour
{
    public GameObject GodMode;
    public TextMeshProUGUI godModeText;
    public TextMeshProUGUI TempoGodMode;

    private bool isGodModeActive = false;
    private Coroutine godModeCoroutine;


    private void Awake()
    {
        isGodModeActive = false;
        if (GodMode != null) GodMode.SetActive(isGodModeActive);
        if (godModeText != null) godModeText.gameObject.SetActive(false);
        if (TempoGodMode != null) TempoGodMode.gameObject.SetActive(false);
    }

    void Start()
    {
        if (GodMode != null) GodMode.SetActive(false);
       
        if (godModeText != null) godModeText.gameObject.SetActive(false);
      
        if (TempoGodMode != null) TempoGodMode.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            isGodModeActive = !isGodModeActive;

            if (GodMode != null) GodMode.SetActive(isGodModeActive);          

            if (godModeText != null) godModeText.gameObject.SetActive(isGodModeActive);           

            if (TempoGodMode != null) TempoGodMode.gameObject.SetActive(false); // No mostrar temporizador si se activa manualmente

        }
    }

    public void ActivarGodModeTemporal(float duracion)
    {
        if(godModeCoroutine != null)
        {
            StopCoroutine(godModeCoroutine);
        }

        godModeCoroutine = StartCoroutine(GodModeConTemporizador(duracion));
    }

    private IEnumerator GodModeConTemporizador(float segundos)
    {
        isGodModeActive = true;

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

        if (GodMode != null) GodMode.SetActive(false);
        if (godModeText != null) godModeText.gameObject.SetActive(false);
        if (TempoGodMode != null) TempoGodMode.gameObject.SetActive(false);

        godModeCoroutine = null;
    }
}
