using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 5f;

    // Global count so only one enemy bullet can exist at a time
    public static int ActiveCount { get; private set; }

    private void OnEnable()
    {
        ActiveCount++;
    }

    private void OnDestroy()
    {
        ActiveCount--;
    }

    private void Update()
    {
        // Move straight down
        transform.position += Vector3.down * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Hit player
        if (other.CompareTag("Player"))
        {
            // however you currently damage the player:
            GameManager.Instance.OnPlayerHit(1);

            Destroy(gameObject);
        }
        // Hit bottom of the screen or some kill zone
        else if (other.CompareTag("BottomBoundary"))
        {
            Destroy(gameObject);
        }
    }
}
