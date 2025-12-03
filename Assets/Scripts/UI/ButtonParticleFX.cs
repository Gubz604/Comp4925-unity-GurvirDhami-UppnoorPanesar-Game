using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonParticleFX : MonoBehaviour,
    ISelectHandler, IDeselectHandler,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerClickHandler
{
    [SerializeField] private ParticleSystem stars;
    [SerializeField] private float idleRateOverTime = 8f; // particles per second when highlighted
    [SerializeField] private int clickBurstCount = 20;    // extra on click / enter

    private ParticleSystem.EmissionModule _emission;

    private void Awake()
    {
        if (stars == null)
            stars = GetComponentInChildren<ParticleSystem>();

        if (stars != null)
        {
            _emission = stars.emission;
            _emission.rateOverTime = 0f; // start disabled
        }
    }

    // ---- Highlight on/off helpers ----

    private void SetHighlighted(bool highlighted)
    {
        if (stars == null) return;
        _emission.rateOverTime = highlighted ? idleRateOverTime : 0f;
    }

    // Keyboard selection
    public void OnSelect(BaseEventData eventData)
    {
        SetHighlighted(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        SetHighlighted(false);
    }

    // Mouse hover
    public void OnPointerEnter(PointerEventData eventData)
    {
        SetHighlighted(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetHighlighted(false);
    }

    // Mouse click
    public void OnPointerClick(PointerEventData eventData)
    {
        Pulse();
    }

    // Called by keyboard (Enter) via LoginMenuController
    public void Pulse()
    {
        if (stars == null) return;
        stars.Emit(clickBurstCount);
    }
}
