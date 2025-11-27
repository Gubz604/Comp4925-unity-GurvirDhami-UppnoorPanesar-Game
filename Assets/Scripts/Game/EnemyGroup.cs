using UnityEngine;

public class EnemyGroup : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float downStep = 0.5f;
    public float viewportMargin = 0.05f;

    private int _direction = 1;   // 1 = right, -1 = left

    private void Update()
    {
        if (transform.childCount == 0) return;

        // Move sideways
        transform.position += Vector3.right * _direction * moveSpeed * Time.deltaTime;

        // Check if any child is near screen edge
        bool shouldStepDown = false;
        Camera cam = Camera.main;

        foreach (Transform child in transform)
        {
            Vector3 viewportPos = cam.WorldToViewportPoint(child.position);
            if (viewportPos.x < viewportMargin || viewportPos.x > 1f - viewportMargin)
            {
                shouldStepDown = true;
                break;
            }
        }

        if (shouldStepDown)
        {
            _direction *= -1;
            transform.position += Vector3.down * downStep;
        }
    }
}
