using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerShip : MonoBehaviour
{
    public float moveSpeed = 8f;
    public float padding = 0.5f;

    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireCooldown = 0.25f;

    private float _nextFireTime;
    private float _xMin;
    private float _xMax;

    private void Start()
    {
        // Compute horizontal bounds based on camera
        var cam = Camera.main;
        Vector3 left = cam.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 right = cam.ViewportToWorldPoint(new Vector3(1, 0, 0));
        _xMin = left.x + padding;
        _xMax = right.x - padding;
    }

    private void Update()
    {
        HandleMovement();
        HandleShooting();
    }

    private void HandleMovement()
    {
        float x = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right
        Vector3 pos = transform.position;
        pos.x += x * moveSpeed * Time.deltaTime;
        pos.x = Mathf.Clamp(pos.x, _xMin, _xMax);
        transform.position = pos;
    }

    private void HandleShooting()
    {
        if (Input.GetKey(KeyCode.Space) || Input.GetButton("Fire1"))
        {
            if (Time.time >= _nextFireTime)
            {
                _nextFireTime = Time.time + fireCooldown;
                Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            }
        }
    }

    public void TakeHit(int damage)
    {
        GameManager.Instance.OnPlayerHit(damage);
    }
}
