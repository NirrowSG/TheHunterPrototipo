using UnityEngine;
using UnityEngine.InputSystem;

public class CombatInputHandler : MonoBehaviour
{
    [Header("Combat Reference")]
    public CombatManager combatManager;

    [Header("Swipe Settings")]
    public float swipeThreshold = 50f;
    public float maxSwipeTime = 0.5f;

    private Vector2 touchStartPosition;
    private float touchStartTime;
    private bool isSwiping = false;

    private void Update()
    {
        if (combatManager == null || !combatManager.IsCombatActive)
            return;

        HandleTouchInput();
    }

    private void HandleTouchInput()
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            var touch = Touchscreen.current.primaryTouch;

            if (touch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began)
            {
                touchStartPosition = touch.position.ReadValue();
                touchStartTime = Time.time;
                isSwiping = true;
            }
            else if (touch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Ended && isSwiping)
            {
                Vector2 touchEndPosition = touch.position.ReadValue();
                float swipeTime = Time.time - touchStartTime;
                float swipeDistance = Vector2.Distance(touchStartPosition, touchEndPosition);

                if (swipeTime <= maxSwipeTime && swipeDistance >= swipeThreshold)
                {
                    OnSwipe();
                }
                else
                {
                    OnTap();
                }

                isSwiping = false;
            }
        }
        else if (Mouse.current != null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                touchStartPosition = Mouse.current.position.ReadValue();
                touchStartTime = Time.time;
                isSwiping = true;
            }
            else if (Mouse.current.leftButton.wasReleasedThisFrame && isSwiping)
            {
                Vector2 mouseEndPosition = Mouse.current.position.ReadValue();
                float swipeTime = Time.time - touchStartTime;
                float swipeDistance = Vector2.Distance(touchStartPosition, mouseEndPosition);

                if (swipeTime <= maxSwipeTime && swipeDistance >= swipeThreshold)
                {
                    OnSwipe();
                }
                else
                {
                    OnTap();
                }

                isSwiping = false;
            }
        }
    }

    private void OnSwipe()
    {
        Debug.Log("CombatInputHandler: Swipe detectado - Ataque Melee");
        combatManager.PlayerMeleeAttack();
    }

    private void OnTap()
    {
        Debug.Log("CombatInputHandler: Tap detectado - Ataque Fire");
        combatManager.PlayerFireAttack();
    }
}
