using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 1;
    public int scoreValue = 10;

    public int row;
    public int col;

    [Header("Sprites")]
    public Sprite frameA;
    public Sprite frameB;

    [Header("Death Effect")]
    public GameObject deathFlashPrefab;    // full destruction sprite OR screen flash

    private int _currentHealth;
    private SpriteRenderer _spriteRenderer;
    private bool _useFrameA = true;
    private bool _isDead = false;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null)
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (_spriteRenderer == null)
        {
            Debug.LogError("Enemy: No SpriteRenderer found!", this);
            return;
        }

        if (frameA != null)
            _spriteRenderer.sprite = frameA;
    }

    private void Start()
    {
        _currentHealth = maxHealth;
    }

    // -------------------------
    //       TAKE DAMAGE
    // -------------------------
    public void TakeHit(int damage)
    {
        if (_isDead) return;

        _currentHealth -= damage;

        if (_currentHealth <= 0)
            Die();
    }

    // -------------------------
    //          DEATH
    // -------------------------
    private void Die()
    {
        if (_isDead) return;
        _isDead = true;

        // Disable collisions immediately
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // Hide original enemy sprite
        if (_spriteRenderer != null)
            _spriteRenderer.enabled = false;

        // Spawn flash effect
        if (deathFlashPrefab != null)
        {
            GameObject fx = Instantiate(deathFlashPrefab, transform.position, Quaternion.identity);
            Destroy(fx, 0.35f);
        }

        // Notify manager (ONLY ONE AT A TIME BECAUSE OF QUEUE)
        GameManager.Instance.OnEnemyKilled(scoreValue, transform.position);

        // Remove original enemy object
        Destroy(gameObject);
    }

    // -------------------------
    // COLLISIONS
    // -------------------------
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerShip p = other.GetComponent<PlayerShip>();
            if (p != null)
                p.TakeHit(1);

            Die();
        }
        else if (other.CompareTag("PlayerBullet"))
        {
            Destroy(other.gameObject);
            TakeHit(1);
        }
    }

    // -------------------------
    // WALK CYCLE ANIMATION
    // -------------------------
    public void ToggleFrame()
    {
        if (frameA == null || frameB == null || _spriteRenderer == null)
            return;

        _useFrameA = !_useFrameA;
        _spriteRenderer.sprite = _useFrameA ? frameA : frameB;
    }
}
