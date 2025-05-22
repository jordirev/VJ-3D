using UnityEngine;
using TMPro;

public class GodModeWall : MonoBehaviour
{
    public GameObject GodMode;
    public TextMeshProUGUI godModeText;

    private bool isGodModeActive = false;


    void Start()
    {
        if (GodMode != null)
        {
            GodMode.SetActive(false);
        }

        if (godModeText != null)
        {
            godModeText.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            isGodModeActive = !isGodModeActive;

            if (GodMode != null)
            {
                GodMode.SetActive(isGodModeActive);
            }

            if (godModeText != null)
            {
                godModeText.gameObject.SetActive(isGodModeActive);
            }
        }
    }
}
