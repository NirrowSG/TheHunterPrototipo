using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;

    private Vector2 moveInput;
    private Vector2 lastMoveDirection = Vector2.down;

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;

        UpdateAnimator();
    }

    private void UpdateAnimator()
    {
        if (animator == null) return;

        float speed = moveInput.magnitude;
        animator.SetFloat("Speed", speed);

        if (speed > 0.01f)
        {
            lastMoveDirection = moveInput.normalized;
        }

        animator.SetFloat("MoveX", lastMoveDirection.x);
        animator.SetFloat("MoveY", lastMoveDirection.y);
    }
}
