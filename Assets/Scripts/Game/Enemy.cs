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

        if (other.CompareTag("PlayerBullet"))
        {
            GameManager.Instance.OnEnemyKilled(scoreValue, transform.position);
            Destroy(other.gameObject);
            Destroy(gameObject);
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
