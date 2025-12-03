using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonParticlePulse : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private ParticleSystem stars;  // assign the child particle system

    // Called by mouse click / touch
    public void OnPointerClick(PointerEventData eventData)
    {
        Pulse();
    }

    public void Pulse()
    {
        if (stars == null) return;

        // Emit an extra burst of particles
        stars.Emit(20);   // tweak this number to taste
    }
}
