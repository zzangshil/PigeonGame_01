using UnityEngine;

public class PressureCamera : MonoBehaviour
{
    public Transform player;

    [Header("Movement")]
    public float forwardSpeed = 3f;
    public float followYSpeed = 3f;
    public float yOffset = 1.5f;

    [Header("Speed Increase")]
    public float speedIncreaseRate = 0.2f;
    public float maxSpeed = 8f;

    [Header("Background Bounds (Y only)")]
    public SpriteRenderer background;

    private float minY, maxY;
    private float camHalfHeight;

    void Start()
    {
        camHalfHeight = Camera.main.orthographicSize;

        Bounds bgBounds = background.bounds;

        minY = bgBounds.min.y + camHalfHeight;
        maxY = bgBounds.max.y - camHalfHeight;
    }

    void Update()
    {
        if (player == null) return;

        Vector3 pos = transform.position;

        // speed increase
        forwardSpeed += speedIncreaseRate * Time.deltaTime;
        forwardSpeed = Mathf.Clamp(forwardSpeed, 0, maxSpeed);

        // ALWAYS move forward (IMPORTANT)
        pos.x += forwardSpeed * Time.deltaTime;

        // follow player Y
        float targetY = player.position.y + yOffset;
        pos.y = Mathf.Lerp(pos.y, targetY, Time.deltaTime * followYSpeed);

        // ONLY clamp Y (safe)
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        transform.position = pos;

        CheckIfPlayerLost();
    }

    void CheckIfPlayerLost()
    {
        Vector3 screenPos = Camera.main.WorldToViewportPoint(player.position);

        if (screenPos.x < 0 || screenPos.y < -0.2f)
        {
            GameManager.instance.PlayerDied();
        }
    }
}