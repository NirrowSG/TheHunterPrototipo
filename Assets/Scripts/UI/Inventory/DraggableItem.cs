using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [HideInInspector] public Transform parentAfterDrag;
    [HideInInspector] public ItemDataSO itemData;
    [HideInInspector] public int slotIndex;
    [HideInInspector] public bool isQuickSlot = false;
    [HideInInspector] public bool isFromBaseStash = false;

    [SerializeField] private TextMeshProUGUI quantityText;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Transform rootCanvas;
    private bool isDragging = false;
    private bool wasDropped = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        rootCanvas = GetComponentInParent<Canvas>().transform;

        if (rootCanvas == null)
        {
            Debug.LogError("DraggableItem: No se encontró Canvas padre!");
        }

        if (quantityText == null)
        {
            quantityText = GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isDragging && itemData != null)
        {
            Debug.Log($"DraggableItem: Click detectado en {itemData.Name} (slot {slotIndex})");

            if (ItemInfoDisplay.Instance != null)
            {
                if (isFromBaseStash)
                {
                    ItemInfoDisplay.Instance.MostrarInfoItemDeBaseStash(itemData, transform.position, slotIndex);
                }
                else
                {
                    ItemInfoDisplay.Instance.MostrarInfoItem(itemData, transform.position, slotIndex);
                }
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        wasDropped = false;
        Debug.Log($"DraggableItem: OnBeginDrag - Iniciando arrastre desde slot {slotIndex}");

        if (ItemInfoDisplay.Instance != null)
        {
            ItemInfoDisplay.Instance.OcultarPanel();
        }

        parentAfterDrag = transform.parent;

        transform.SetParent(rootCanvas);
        transform.SetAsLastSibling();

        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (rootCanvas == null) return;

        Canvas canvas = rootCanvas.GetComponent<Canvas>();
        if (canvas != null)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        Debug.Log($"DraggableItem: OnEndDrag - Finalizando arrastre (wasDropped: {wasDropped})");

        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        transform.SetParent(parentAfterDrag);
        transform.localPosition = Vector3.zero;

        if (!wasDropped)
        {
            Debug.Log("DraggableItem: No se soltó en ningún slot, actualizando UI");
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.ActualizarUI();
            }
        }
    }

    public void SetItemData(ItemDataSO data, int index)
    {
        itemData = data;
        slotIndex = index;
    }

    public void SetQuantity(int quantity)
    {
        if (quantityText != null)
        {
            if (quantity > 1)
            {
                quantityText.text = quantity.ToString();
                quantityText.gameObject.SetActive(true);
            }
            else
            {
                quantityText.gameObject.SetActive(false);
            }
        }
    }

    public void MarkAsDropped()
    {
        wasDropped = true;
    }
}
