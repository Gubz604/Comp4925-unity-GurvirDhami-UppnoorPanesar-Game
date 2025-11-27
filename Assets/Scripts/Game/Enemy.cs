using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 1;
    public int scoreValue = 10;

    private int _currentHealth;

    private void Start()
    {
        _currentHealth = maxHealth;
    }

    public void TakeHit(int damage)
    {
        _currentHealth -= damage;
        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        GameManager.Instance.OnEnemyKilled(scoreValue, transform.position);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerShip player = other.GetComponent<PlayerShip>();
            if (player != null)
            {
                player.TakeHit(1);   
            }

            Destroy(gameObject);
        }
    }
}
