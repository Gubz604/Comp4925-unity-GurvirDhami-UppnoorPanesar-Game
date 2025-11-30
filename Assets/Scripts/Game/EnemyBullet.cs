using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public float lifetime = 3f;

    [Header("Animation Frames")]
    public Sprite frameA;
    public Sprite frameB;
    public float frameInterval = 0.5f; // change every half-second

    private SpriteRenderer sr;
    private float nextFrameTime;
    private bool useFrameA = true;

    // --- Only allow 1 bullet at a time ---
    public static int ActiveCount { get; private set; }

    // ------------------------------------------------------------
    private void OnEnable()
    {
        ActiveCount++;
        sr = GetComponent<SpriteRenderer>();

        // Make sure animation starts immediately
        nextFrameTime = Time.time + frameInterval;
    }

    private void OnDestroy()
    {
        ActiveCount = Mathf.Max(ActiveCount - 1, 0); // prevents negative errors
    }
    // ------------------------------------------------------------

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // MOVE BULLET DOWN
        transform.position += Vector3.down * speed * Time.deltaTime;

        // ANIMATE SPRITE
        if (sr != null && Time.time >= nextFrameTime)
        {
            useFrameA = !useFrameA;
            sr.sprite = useFrameA ? frameA : frameB;
            nextFrameTime = Time.time + frameInterval;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Hit Player
        if (other.CompareTag("Player"))
        {
            PlayerShip p = other.GetComponent<PlayerShip>();
            if (p != null)
                p.TakeHit(1);

            Destroy(gameObject);
        }

        // Hit Barrier
        else if (other.CompareTag("Barrier"))
        {
            Barrier b = other.GetComponent<Barrier>();
            if (b != null)
                b.TakeDamage(1);

            Destroy(gameObject);
        }

        // Hit bottom boundary (clean up)
        else if (other.CompareTag("BottomBoundary"))
        {
            Destroy(gameObject);
        }
    }
}
