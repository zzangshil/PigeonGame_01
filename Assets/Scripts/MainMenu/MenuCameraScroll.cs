using UnityEngine;
using UnityEngine.InputSystem;

public class MenuCameraScroll : MonoBehaviour
{
    [Header("Scroll Settings")]
    public float scrollSpeed = 5f;

    [Header("Vertical Limits")]
    public float minY = -5f;
    public float maxY = 5f;

    [Header("Edge Scrolling")]
    public float edgeThreshold = 50f;

    void Update()
    {
        if (Mouse.current == null) return; 

        Vector3 pos = transform.position;
        float scroll = 0f;

        Vector2 mousePos = Mouse.current.position.ReadValue();

        if (mousePos.y >= Screen.height - edgeThreshold)
        {
            scroll = 1f; 
        }
        
        else if (mousePos.y <= edgeThreshold)
        {
            scroll = -1f; 
        }

        pos.y += scroll * scrollSpeed * Time.deltaTime;

        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        transform.position = pos;
    }
}