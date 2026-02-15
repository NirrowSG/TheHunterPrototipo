using UnityEngine;

public class DynamicSpriteSorter : MonoBehaviour
{
    [Header("Sorting Settings")]
    [Tooltip("Base sorting order offset")]
    [SerializeField] private int baseSortingOrder = 0;

    [Tooltip("Multiplier for position-based sorting (higher = more separation)")]
    [SerializeField] private int sortingOrderMultiplier = 100;

    [Tooltip("Update sorting every frame (disable for static objects)")]
    [SerializeField] private bool updateEveryFrame = true;

    [Tooltip("Use custom pivot offset for sorting calculation")]
    [SerializeField] private bool useCustomPivot = false;

    [Tooltip("Custom pivot Y offset from transform position")]
    [SerializeField] private float customPivotYOffset = 0f;

    private SpriteRenderer spriteRenderer;
    private float lastYPosition;
    private const float UPDATE_THRESHOLD = 0.01f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError($"DynamicSpriteSorter on {gameObject.name} requires a SpriteRenderer component!", this);
            enabled = false;
            return;
        }
    }

    private void Start()
    {
        UpdateSortingOrder();
    }

    private void LateUpdate()
    {
        if (updateEveryFrame)
        {
            float currentY = GetSortingYPosition();

            if (Mathf.Abs(currentY - lastYPosition) > UPDATE_THRESHOLD)
            {
                UpdateSortingOrder();
            }
        }
    }

    private void UpdateSortingOrder()
    {
        float yPosition = GetSortingYPosition();
        lastYPosition = yPosition;

        int newSortingOrder = baseSortingOrder - Mathf.RoundToInt(yPosition * sortingOrderMultiplier);
        spriteRenderer.sortingOrder = newSortingOrder;
    }

    private float GetSortingYPosition()
    {
        if (useCustomPivot)
        {
            return transform.position.y + customPivotYOffset;
        }

        return transform.position.y;
    }

    public void ForceUpdateSorting()
    {
        UpdateSortingOrder();
    }
}
