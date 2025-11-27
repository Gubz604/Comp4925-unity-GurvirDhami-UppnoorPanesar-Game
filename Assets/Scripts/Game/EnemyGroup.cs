using UnityEngine;

public class EnemyGroup : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float downStep = 0.5f;
    public float viewportMargin = 0.05f;
    public float edgeCooldown = 0.2f;   // time between valid edge hits

    private int _direction = 1;         // 1 = right, -1 = left
    private float _edgeTimer = 0f;

    private void Update()
    {
        if (transform.childCount == 0) return;

        _edgeTimer -= Time.deltaTime;

        // Move sideways
        transform.position += Vector3.right * _direction * moveSpeed * Time.deltaTime;

        // Check if any child is near screen edge
        Camera cam = Camera.main;
        if (cam == null) return;

        bool hitEdge = false;

        foreach (Transform child in transform)
        {
            Vector3 viewportPos = cam.WorldToViewportPoint(child.position);
            if (viewportPos.x < viewportMargin || viewportPos.x > 1f - viewportMargin)
            {
                hitEdge = true;
                break;
            }
        }

        // Only react if we *just* hit an edge (cooldown over)
        if (hitEdge && _edgeTimer <= 0f)
        {
            _direction *= -1;
            transform.position += Vector3.down * downStep;
            _edgeTimer = edgeCooldown;  // prevent immediate repeated steps
        }
    }

    // Called from GameManager when starting a new wave
    public void ResetGroup(Vector3 worldPosition)
    {
        transform.position = worldPosition;
        _direction = 1;
        _edgeTimer = 0f;
    }
}
