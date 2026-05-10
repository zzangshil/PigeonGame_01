using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Lives")]
    public int lives = 3;

    [Header("References")]
    public GameObject player;

    private bool isGameOver = false;

    void Awake()
    {
        instance = this;
    }

    public void PlayerDied()
    {
        if (isGameOver) return;

        lives--;
        Debug.Log("Lives left: " + lives);

        if (lives > 0)
        {
            Invoke(nameof(RevivePlayer), 1f);
        }
        else
        {
            GameOver();
        }
    }

    void RevivePlayer()
    {
        PlayerMovement pm = player.GetComponent<PlayerMovement>();

        // Respawn at last safe platform
        player.transform.position = pm.lastSafePosition + new Vector3(0, 0.5f, 0);

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
    }

    void GameOver()
    {
        isGameOver = true;
        Debug.Log("GAME OVER");
        Time.timeScale = 0f;
    }
}