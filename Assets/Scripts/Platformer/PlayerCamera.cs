using UnityEngine;

public class PlatformerCameraPro : MonoBehaviour
{
    [Header("Target")]
    public Transform player;
    private Rigidbody2D playerRb;

    [Header("Follow")]
    public Vector3 offset = new Vector3(0f, 1f, -10f);
    public float smoothTime = 0.15f;

    [Header("Dead Zone")]
    public float deadZoneX = 1.5f;
    public float deadZoneY = 1f;

    [Header("Zoom")]
    public Camera cam;
    public float normalZoom = 5f;
    public float jumpZoom = 5.5f;
    public float fallZoom = 4.5f;
    public float zoomSpeed = 3f;

    [Header("Shake")]
    public float landShakeIntensity = 0.15f;
    public float landShakeDuration = 0.15f;

    private Vector3 velocity = Vector3.zero;
    private float currentZoom;
    private Vector3 shakeOffset;

    private bool wasGrounded;

    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        if (player != null)
            playerRb = player.GetComponent<Rigidbody2D>();

        if (cam == null)
            cam = Camera.main;

        currentZoom = normalZoom;
    }

    void LateUpdate()
    {
        if (player == null) return;

        HandleCameraFollow();
        HandleZoom();
        HandleLandingShake();

        cam.orthographicSize = Mathf.Lerp(
            cam.orthographicSize,
            currentZoom,
            Time.deltaTime * zoomSpeed
        );
    }

    void HandleCameraFollow()
    {
        Vector3 target = player.position + offset;

        Vector3 delta = target - transform.position;

        // ---- DEAD ZONE (this fixes fast camera movement) ----
        if (Mathf.Abs(delta.x) < deadZoneX)
            target.x = transform.position.x;

        if (Mathf.Abs(delta.y) < deadZoneY)
            target.y = transform.position.y;

        target += shakeOffset;
        target.z = -10f;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            target,
            ref velocity,
            smoothTime
        );
    }

    void HandleZoom()
    {
        if (playerRb == null) return;

        if (playerRb.linearVelocity.y > 0.1f)
        {
            currentZoom = jumpZoom;
        }
        else if (playerRb.linearVelocity.y < -0.1f)
        {
            currentZoom = fallZoom;
        }
        else
        {
            currentZoom = normalZoom;
        }
    }

    void HandleLandingShake()
    {
        if (playerRb == null) return;

        bool isGrounded = Mathf.Abs(playerRb.linearVelocity.y) < 0.01f;

        // detect landing
        if (!wasGrounded && isGrounded)
        {
            StartCoroutine(Shake());
        }

        wasGrounded = isGrounded;
    }

    System.Collections.IEnumerator Shake()
    {
        float t = 0f;

        while (t < landShakeDuration)
        {
            shakeOffset = (Vector3)Random.insideUnitCircle * landShakeIntensity;

            t += Time.deltaTime;
            yield return null;
        }

        shakeOffset = Vector3.zero;
    }
}