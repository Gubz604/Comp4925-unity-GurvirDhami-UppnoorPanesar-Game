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
    public GameObject deathFlashPrefab;    

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

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        if (_spriteRenderer != null)
            _spriteRenderer.enabled = false;

        if (deathFlashPrefab != null)
        {
            GameObject fx = Instantiate(deathFlashPrefab, transform.position, Quaternion.identity);
            Destroy(fx, 0.35f);
        }

        GameManager.Instance.OnEnemyKilled(scoreValue, transform.position);

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
