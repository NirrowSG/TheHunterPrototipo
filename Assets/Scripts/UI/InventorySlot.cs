using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public Image iconoDisplay;
    public TextMeshProUGUI textoCantidad;
    public GameObject itemPrefab;

    private InventoryItem slotItemActual;
    private GameObject currentItemObject;
    private int slotIndex;

    private void Start()
    {
        slotIndex = transform.GetSiblingIndex();
        Debug.Log($"InventorySlot: Slot inicializado en índice {slotIndex}");
    }

    public void ActualizarSlot(InventoryItem itemSlot)
    {
        slotItemActual = itemSlot;

        if (currentItemObject != null)
        {
            Destroy(currentItemObject);
        }

        if (itemSlot != null && itemSlot.itemData != null)
        {
            Debug.Log($"InventorySlot [{slotIndex}]: Actualizando slot con {itemSlot.itemData.Name} x{itemSlot.cantidad}");

            if (itemPrefab == null)
            {
                Debug.LogError("InventorySlot: itemPrefab es NULL! Asigna el DraggableItemPrefab en el Inspector");
                return;
            }

            currentItemObject = Instantiate(itemPrefab, transform);
            currentItemObject.transform.localPosition = Vector3.zero;

            Image itemIcon = currentItemObject.GetComponent<Image>();
            if (itemIcon != null)
            {
                itemIcon.sprite = itemSlot.itemData.Icon;
            }
            else
            {
                Debug.LogError("InventorySlot: No se encontró componente Image en el itemPrefab");
            }

            DraggableItem draggable = currentItemObject.GetComponent<DraggableItem>();
            if (draggable != null)
            {
                draggable.SetItemData(itemSlot.itemData, slotIndex);
            }

            if (itemSlot.cantidad > 1 && textoCantidad != null)
            {
                textoCantidad.text = itemSlot.cantidad.ToString();
                textoCantidad.gameObject.SetActive(true);
            }
            else if (textoCantidad != null)
            {
                textoCantidad.gameObject.SetActive(false);
            }
        }
        else
        {
            LimpiarSlot();
        }
    }

    public void LimpiarSlot()
    {
        slotItemActual = null;

        if (currentItemObject != null)
        {
            Destroy(currentItemObject);
        }

        if (textoCantidad != null)
        {
            textoCantidad.gameObject.SetActive(false);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log($"InventorySlot [{slotIndex}]: OnDrop detectado!");

        GameObject droppedObject = eventData.pointerDrag;
        if (droppedObject == null)
        {
            Debug.LogWarning("InventorySlot: El objeto soltado es null");
            return;
        }

        DraggableItem draggableItem = droppedObject.GetComponent<DraggableItem>();
        if (draggableItem == null)
        {
            Debug.LogWarning("InventorySlot: El objeto no tiene componente DraggableItem");
            return;
        }

        Transform originalParent = draggableItem.parentAfterDrag;
        if (originalParent == null)
        {
            Debug.LogWarning("InventorySlot: El padre original es null");
            return;
        }

        InventorySlot originalSlot = originalParent.GetComponent<InventorySlot>();
        if (originalSlot == null)
        {
            Debug.LogWarning("InventorySlot: El slot original no tiene componente InventorySlot");
            return;
        }

        int originalIndex = originalSlot.slotIndex;
        int targetIndex = slotIndex;

        Debug.Log($"InventorySlot: Intercambiando ítems - Desde slot {originalIndex} hacia slot {targetIndex}");

        draggableItem.parentAfterDrag = transform;
        draggableItem.MarkAsDropped();

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.IntercambiarItems(originalIndex, targetIndex);
        }
    }
}
