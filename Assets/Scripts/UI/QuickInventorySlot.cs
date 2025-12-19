using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class QuickInventorySlot : MonoBehaviour, IDropHandler
{
    public GameObject itemPrefab;

    private InventoryItem slotItemActual;
    private GameObject currentItemObject;
    private int slotIndex;

    private void Awake()
    {
        slotIndex = transform.GetSiblingIndex();
        Debug.Log($"QuickInventorySlot: Slot inicializado en índice {slotIndex}");
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
            Debug.Log($"QuickInventorySlot [{slotIndex}]: Actualizando slot con {itemSlot.itemData.Name} x{itemSlot.cantidad}");

            if (itemPrefab == null)
            {
                Debug.LogError("QuickInventorySlot: itemPrefab es NULL!");
                return;
            }

            currentItemObject = Instantiate(itemPrefab, transform);
            currentItemObject.transform.localPosition = Vector3.zero;

            Image itemIcon = currentItemObject.GetComponent<Image>();
            if (itemIcon != null)
            {
                itemIcon.sprite = itemSlot.itemData.Icon;
            }

            DraggableItem draggable = currentItemObject.GetComponent<DraggableItem>();
            if (draggable != null)
            {
                draggable.SetItemData(itemSlot.itemData, slotIndex);
                draggable.SetQuantity(itemSlot.cantidad);
                draggable.isQuickSlot = true;
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
    }

    public void OnDrop(PointerEventData eventData)
    {
        int targetIndex = transform.GetSiblingIndex();
        Debug.Log($"QuickInventorySlot: OnDrop detectado en índice {targetIndex}!");

        GameObject droppedObject = eventData.pointerDrag;
        if (droppedObject == null)
        {
            Debug.LogWarning("QuickInventorySlot: droppedObject es null");
            return;
        }

        DraggableItem draggableItem = droppedObject.GetComponent<DraggableItem>();
        if (draggableItem == null)
        {
            Debug.LogWarning("QuickInventorySlot: No tiene DraggableItem");
            return;
        }

        Transform originalParent = draggableItem.parentAfterDrag;
        if (originalParent == null)
        {
            Debug.LogWarning("QuickInventorySlot: originalParent es null");
            return;
        }

        bool wasFromQuickSlot = draggableItem.isQuickSlot;

        if (wasFromQuickSlot)
        {
            QuickInventorySlot originalSlot = originalParent.GetComponent<QuickInventorySlot>();
            if (originalSlot != null)
            {
                int originalIndex = originalParent.GetSiblingIndex();
                Debug.Log($"QuickInventorySlot: Moviendo dentro de quick slots {originalIndex} -> {targetIndex}");

                draggableItem.parentAfterDrag = transform;
                draggableItem.MarkAsDropped();

                if (QuickInventoryManager.Instance != null)
                {
                    QuickInventoryManager.Instance.IntercambiarItems(originalIndex, targetIndex);
                }
                else
                {
                    Debug.LogError("QuickInventorySlot: QuickInventoryManager.Instance es null");
                }
            }
        }
        else
        {
            InventorySlot originalSlot = originalParent.GetComponent<InventorySlot>();
            if (originalSlot != null)
            {
                Debug.Log($"QuickInventorySlot: Moviendo desde inventario principal a quick slot {targetIndex}");

                int originalIndex = draggableItem.slotIndex;

                if (InventoryManager.Instance == null)
                {
                    Debug.LogError("QuickInventorySlot: InventoryManager.Instance es null");
                    return;
                }

                if (originalIndex < 0 || originalIndex >= InventoryManager.Instance.inventarioItems.Count)
                {
                    Debug.LogError($"QuickInventorySlot: originalIndex {originalIndex} fuera de rango");
                    return;
                }

                InventoryItem item = InventoryManager.Instance.inventarioItems[originalIndex];

                if (item != null)
                {
                    InventoryItem copiedItem = new InventoryItem(item.itemData, item.cantidad);

                    Debug.Log($"QuickInventorySlot: Agregando {copiedItem.itemData.Name} x{copiedItem.cantidad} en slot {targetIndex}");

                    if (QuickInventoryManager.Instance == null)
                    {
                        Debug.LogError("QuickInventorySlot: QuickInventoryManager.Instance es null");
                        return;
                    }

                    QuickInventoryManager.Instance.AgregarItemEnSlot(copiedItem, targetIndex);
                    InventoryManager.Instance.inventarioItems[originalIndex] = null;
                    InventoryManager.Instance.ActualizarUI();
                }
                else
                {
                    Debug.LogWarning($"QuickInventorySlot: Item en índice {originalIndex} es null");
                }

                draggableItem.MarkAsDropped();
            }
            else
            {
                Debug.LogWarning("QuickInventorySlot: originalSlot no es InventorySlot");
            }
        }
    }
}
