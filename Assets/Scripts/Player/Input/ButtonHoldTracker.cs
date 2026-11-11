using UnityEngine;
using UnityEngine.EventSystems;

namespace Player.Input
{
    public sealed class ButtonHoldTracker : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public bool IsHeld { get; private set; }

        public void OnPointerDown(PointerEventData eventData) => IsHeld = true;

        public void OnPointerUp(PointerEventData eventData) => IsHeld = false;

        private void OnDisable() => IsHeld = false;
    }
}
