using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 1;
    public int scoreValue = 10;

    public int row;
    public int col;

    [Header("Sprites")]
    public Sprite frameA;   // first animation frame
    public Sprite frameB;   // second animation frame

    private int _currentHealth;
    private SpriteRenderer _spriteRenderer;
    private bool _useFrameA = true;
    private bool _isDead = false;

    private void Awake()
    {
        // Try to find SpriteRenderer on this object or its children
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null)
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (_spriteRenderer == null)
        {
            Debug.LogError("Enemy: No SpriteRenderer found!", this);
            return;
        }

        // Default to frameA at start if set
        if (frameA != null)
        {
            _spriteRenderer.sprite = frameA;
        }
    }

    private void Start()
    {
        _currentHealth = maxHealth;
    }

    public void TakeHit(int damage)
    {
        if (_isDead) return;              // already dead, ignore further hits

        _currentHealth -= damage;
        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (_isDead) return;
        _isDead = true;

        // ✅ Only here do we tell GameManager about the kill
        GameManager.Instance.OnEnemyKilled(scoreValue, transform.position);

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Enemy collides with player ship
        if (other.CompareTag("Player"))
        {
            PlayerShip player = other.GetComponent<PlayerShip>();
            if (player != null)
            {
                player.TakeHit(1);
            }

            // Usually the enemy dies when hitting the player
            Die();
        }
        // Enemy gets hit by player bullet
        else if (other.CompareTag("PlayerBullet"))
        {
            // Bullet is consumed
            Destroy(other.gameObject);

            // Apply damage through the normal path
            TakeHit(1);
        }
    }

    // Called by EnemyGroup every time the formation “steps”
    public void ToggleFrame()
    {
        if (frameA == null || frameB == null || _spriteRenderer == null)
        {
            // Helpful while debugging:
            Debug.LogWarning("Enemy: Missing frameA/frameB/spriteRenderer", this);
            return;
        }

        _useFrameA = !_useFrameA;
        _spriteRenderer.sprite = _useFrameA ? frameA : frameB;
    }
}
