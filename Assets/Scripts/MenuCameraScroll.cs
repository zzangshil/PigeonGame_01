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
    public float edgeThreshold = 50f; // distance from screen edge to start scrolling

    void Update()
    {
        if (Mouse.current == null) return; // safety check

        Vector3 pos = transform.position;
        float scroll = 0f;

        // Get mouse position from new Input System
        Vector2 mousePos = Mouse.current.position.ReadValue();

        // Check if mouse is near top of screen
        if (mousePos.y >= Screen.height - edgeThreshold)
        {
            scroll = 1f; // scroll up
        }
        // Check if mouse is near bottom of screen
        else if (mousePos.y <= edgeThreshold)
        {
            scroll = -1f; // scroll down
        }

        // Apply scroll
        pos.y += scroll * scrollSpeed * Time.deltaTime;

        // Clamp camera movement
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        transform.position = pos;
    }
}