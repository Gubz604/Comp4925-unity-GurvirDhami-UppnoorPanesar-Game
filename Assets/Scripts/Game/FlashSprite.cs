using UnityEngine;
using System.Collections;

public class FlashSprite : MonoBehaviour
{
    public float liveTime = 0.12f;

    private SpriteRenderer sr;

    private void OnEnable()
    {
        sr = GetComponent<SpriteRenderer>();
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        // Fade in fast
        sr.color = new Color(1, 1, 1, 1);
        yield return new WaitForSeconds(liveTime);

        // Fade out
        sr.color = new Color(1, 1, 1, 0);
    }
}
