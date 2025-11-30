using UnityEngine;

public class DeathSpriteEffect : MonoBehaviour
{
    public float lifetime = 0.25f; // how long the sprite stays

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
