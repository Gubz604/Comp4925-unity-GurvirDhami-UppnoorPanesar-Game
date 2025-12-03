using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerShip : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float padding = 0.5f;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireCooldown = 0.25f;

    [Header("Death / Respawn")]
    public GameObject deathEffectPrefab;   
    public float respawnDelay = 0.5f;

    [Header("Audio")]
    [SerializeField] private AudioClip shootClip;

    private float _nextFireTime;
    private float _xMin;
    private float _xMax;

    private Vector3 _startPosition;
    private bool _isDead = false;

    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider2D;
    private Rigidbody2D _rb;

    private AudioSource audioSource;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null)
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        _collider2D = GetComponent<Collider2D>();
        _rb = GetComponent<Rigidbody2D>();

        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        _startPosition = transform.position;

        var cam = Camera.main;
        Vector3 left = cam.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 right = cam.ViewportToWorldPoint(new Vector3(1, 0, 0));
        _xMin = left.x + padding;
        _xMax = right.x - padding;
    }

    private void Update()
    {
        if (_isDead) return;

        HandleMovement();
        HandleShooting();
    }

    private void HandleMovement()
    {
        float x = Input.GetAxisRaw("Horizontal");
        Vector3 pos = transform.position;
        pos.x += x * moveSpeed * Time.deltaTime;
        pos.x = Mathf.Clamp(pos.x, _xMin, _xMax);
        transform.position = pos;
    }

    private void HandleShooting()
    {
        // Space or mouse button
        if (Input.GetKey(KeyCode.Space) || Input.GetButton("Fire1"))
        {
            if (Time.time >= _nextFireTime && !PlayerBullet.ActiveBulletExists)
            {
                _nextFireTime = Time.time + fireCooldown;
                Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

                if (shootClip != null && audioSource != null)
                    audioSource.PlayOneShot(shootClip);
            }
        }
    }

    public void TakeHit(int damage)
    {
        if (_isDead) return;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlayerHit(damage);
        }

        StartCoroutine(DeathAndRespawnRoutine());
    }

    private IEnumerator DeathAndRespawnRoutine()
    {
        _isDead = true;

        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        if (_rb != null)
        {
            _rb.linearVelocity = Vector2.zero;
        }

        if (_spriteRenderer != null) _spriteRenderer.enabled = false;
        if (_collider2D != null) _collider2D.enabled = false;

        yield return new WaitForSecondsRealtime(respawnDelay);

        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
        {
            yield break;
        }

        transform.position = _startPosition;

        if (_spriteRenderer != null) _spriteRenderer.enabled = true;
        if (_collider2D != null) _collider2D.enabled = true;

        _isDead = false;
    }
}
