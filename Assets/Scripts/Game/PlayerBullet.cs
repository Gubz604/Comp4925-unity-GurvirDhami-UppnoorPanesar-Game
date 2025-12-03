using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public static bool ActiveBulletExists { get; private set; }

    public float speed = 12f;
    public float lifetime = 3f;

    private void OnEnable()
    {
        ActiveBulletExists = true;
    }

    private void OnDisable()
    {
        ActiveBulletExists = false;
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Hit enemy
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeHit(1);
            }

            Destroy(gameObject); 
        }

        // Hit barrier
        if (other.CompareTag("Barrier"))
        {
            Barrier barrier = other.GetComponent<Barrier>();
            if (barrier != null)
            {
                barrier.TakeDamage(1);
            }

            Destroy(gameObject);
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject); 
    }
}
