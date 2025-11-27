using UnityEngine;

public class BottomBoundary : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            GameManager.Instance.OnEnemyReachedBottom();
        }

        // Optional: destroy anything else that falls out of bounds
        // (bullets / powerups / etc.)
        Destroy(other.gameObject);
    }
}
