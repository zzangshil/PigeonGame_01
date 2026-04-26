using UnityEngine;

public class SeamlessParallaxLoop : MonoBehaviour
{
    public Transform cameraTransform;
    public Transform[] backgrounds;

    private float spriteWidth;

    void Start()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        // Automatically detect width from first background
        SpriteRenderer sr = backgrounds[0].GetComponent<SpriteRenderer>();
        spriteWidth = sr.bounds.size.x;
    }

    void LateUpdate()
    {
        for (int i = 0; i < backgrounds.Length; i++)
        {
            Transform bg = backgrounds[i];

            float camX = cameraTransform.position.x;
            float bgX = bg.position.x;

            // If background is too far left → move it right
            if (camX - bgX > spriteWidth)
            {
                MoveToRight(bg);
            }

            // If background is too far right → move it left
            if (bgX - camX > spriteWidth)
            {
                MoveToLeft(bg);
            }
        }
    }

    void MoveToRight(Transform bg)
    {
        float rightMost = GetRightMostX();
        bg.position = new Vector3(rightMost + spriteWidth, bg.position.y, bg.position.z);
    }

    void MoveToLeft(Transform bg)
    {
        float leftMost = GetLeftMostX();
        bg.position = new Vector3(leftMost - spriteWidth, bg.position.y, bg.position.z);
    }

    float GetRightMostX()
    {
        float max = backgrounds[0].position.x;
        foreach (var b in backgrounds)
            if (b.position.x > max) max = b.position.x;
        return max;
    }

    float GetLeftMostX()
    {
        float min = backgrounds[0].position.x;
        foreach (var b in backgrounds)
            if (b.position.x < min) min = b.position.x;
        return min;
    }
}