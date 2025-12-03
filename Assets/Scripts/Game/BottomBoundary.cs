using UnityEngine;

public class BottomBoundary : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            GameManager.Instance.OnEnemyReachedBottom();
        }


        Destroy(other.gameObject);
    }
}
