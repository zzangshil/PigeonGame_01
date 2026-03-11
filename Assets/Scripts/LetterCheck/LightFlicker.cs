using UnityEngine;
using UnityEngine.UI;

public class LightFlicker : MonoBehaviour
{
    public Image imageToFlicker;  // Drag your UI Image here
    public float minAlpha = 0f;   // Invisible
    public float maxAlpha = 1f;   // Fully visible
    public float flickerSpeed = 0.05f; // How fast it flickers

    void Update()
    {
        Color c = imageToFlicker.color;
        c.a = Random.Range(minAlpha, maxAlpha); // Random alpha
        imageToFlicker.color = c;
    }
}