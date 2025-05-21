using UnityEngine;

public class TextureScroller : MonoBehaviour
{
    float scrollX = 0f;
    float scrollY = 0.3f;

    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        float offsetX = Time.time * scrollX;
        float offsetY = Time.time * scrollY;
 
        rend.material.mainTextureOffset = new Vector2(offsetX, offsetY);
    }
}