using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFlash : MonoBehaviour
{
    public static ScreenFlash Instance { get; private set; }

    [Header("Flash Settings")]
    public Image flashImage;
    public float flashOnTime = 0.06f;   // how long it stays fully bright
    public float flashOffTime = 0.06f;  // pause between flashes

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void FlashTwice()
    {
        if (flashImage == null) return;
        StopAllCoroutines();
        StartCoroutine(FlashRoutine(2));
    }

    private IEnumerator FlashRoutine(int flashes)
    {
        Color c = flashImage.color;

        for (int i = 0; i < flashes; i++)
        {
            // ON
            c.a = 1f;
            flashImage.color = c;
            yield return new WaitForSeconds(flashOnTime);

            // OFF
            c.a = 0f;
            flashImage.color = c;
            yield return new WaitForSeconds(flashOffTime);
        }
    }
}
