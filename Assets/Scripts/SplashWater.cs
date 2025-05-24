using UnityEngine;

public class SplashWater : MonoBehaviour
{
    void Start()
    {
        var ps = GetComponent<ParticleSystem>();
        var main = ps.main;
        main.startDelay = Random.Range(0f, 1f); // Cambia el rango según necesites
    }
}
