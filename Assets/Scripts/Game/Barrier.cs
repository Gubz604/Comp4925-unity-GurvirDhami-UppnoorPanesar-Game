using UnityEngine;

public class Barrier : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] int maxHealth = 20;
    int currentHealth;

    [Header("Sprites by Damage Stage")]
    [SerializeField] Sprite undamagedSprite;     // 100%–50%
    [SerializeField] Sprite damagedSprite;       // 50%–25%
    [SerializeField] Sprite heavilyDamagedSprite;// 25%–1%

    SpriteRenderer spriteRenderer;

    void Awake()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateVisual();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Destroy(gameObject);  
            return;
        }

        UpdateVisual();
    }

    void UpdateVisual()
    {
        if (spriteRenderer == null) return;

        float healthPercent = (float)currentHealth / maxHealth;

        if (healthPercent > 0.5f)
        {
            // 100% - 50% health
            spriteRenderer.sprite = undamagedSprite;
        }
        else if (healthPercent > 0.25f)
        {
            // 50% - 25% health
            spriteRenderer.sprite = damagedSprite;
        }
        else
        {
            // 25% - 0% health
            spriteRenderer.sprite = heavilyDamagedSprite;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EnemyBullet") || other.CompareTag("PlayerBullet"))
        {
            TakeDamage(1);              
            Destroy(other.gameObject);  // remove bullet on impact
        }
    }
}
