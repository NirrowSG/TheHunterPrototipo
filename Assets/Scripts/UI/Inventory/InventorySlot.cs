using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public GameObject itemPrefab;

    private InventoryItem slotItemActual;
    private GameObject currentItemObject;
    public int slotIndex { get; private set; }


    private void Awake()
    {
        slotIndex = transform.GetSiblingIndex();
        Debug.Log($"InventorySlot: Slot inicializado en índice {slotIndex}");
    }

    private void OnTransformParentChanged()
    {
        slotIndex = transform.GetSiblingIndex();
        Debug.Log($"InventorySlot: Índice actualizado a {slotIndex}");
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
                draggable.SetQuantity(itemSlot.cantidad);
                draggable.isQuickSlot = false;
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
        Debug.Log($"InventorySlot [{targetIndex}]: OnDrop detectado!");

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
            draggableItem.MarkAsDropped();
            return;
        }

        bool wasFromQuickSlot = draggableItem.isQuickSlot;

        if (wasFromQuickSlot)
        {
            QuickInventorySlot originalSlot = originalParent.GetComponent<QuickInventorySlot>();
            if (originalSlot == null)
            {
                Debug.LogWarning("InventorySlot: originalSlot no es QuickInventorySlot");
                draggableItem.MarkAsDropped();
                return;
            }

            int originalIndex = originalSlot.slotIndex;
            Debug.Log($"InventorySlot: Moviendo desde quick slot {originalIndex} a inventario principal slot {targetIndex}");

            if (QuickInventoryManager.Instance == null)
            {
                Debug.LogError("InventorySlot: QuickInventoryManager.Instance es null");
                draggableItem.MarkAsDropped();
                return;
            }

            if (InventoryManager.Instance == null)
            {
                Debug.LogError("InventorySlot: InventoryManager.Instance es null");
                draggableItem.MarkAsDropped();
                return;
            }

            if (originalIndex < 0 || originalIndex >= QuickInventoryManager.Instance.quickItems.Count)
            {
                Debug.LogError($"InventorySlot: originalIndex {originalIndex} fuera de rango en QuickInventory (Count: {QuickInventoryManager.Instance.quickItems.Count})");
                draggableItem.MarkAsDropped();
                return;
            }

            if (targetIndex < 0 || targetIndex >= InventoryManager.Instance.inventarioItems.Count)
            {
                Debug.LogError($"InventorySlot: targetIndex {targetIndex} fuera de rango en InventoryManager (Count: {InventoryManager.Instance.inventarioItems.Count})");
                draggableItem.MarkAsDropped();
                return;
            }

            InventoryItem quickItem = QuickInventoryManager.Instance.quickItems[originalIndex];
            InventoryItem inventoryItem = InventoryManager.Instance.inventarioItems[targetIndex];

            if (quickItem == null)
            {
                Debug.LogWarning($"InventorySlot: Item en quick slot {originalIndex} es null");
                draggableItem.MarkAsDropped();
                return;
            }

            if (quickItem.itemData == null)
            {
                Debug.LogError($"InventorySlot: quickItem.itemData es null en slot {originalIndex}");
                draggableItem.MarkAsDropped();
                return;
            }

            if (inventoryItem == null)
            {
                Debug.Log($"InventorySlot: Slot de destino vacío, moviendo {quickItem.itemData.Name}");
                QuickInventoryManager.Instance.quickItems[originalIndex] = null;
                InventoryManager.Instance.inventarioItems[targetIndex] = quickItem;
            }
            else if (inventoryItem.itemData == null)
            {
                Debug.LogWarning($"InventorySlot: inventoryItem.itemData es null en slot {targetIndex}, tratando como slot vacío");
                QuickInventoryManager.Instance.quickItems[originalIndex] = null;
                InventoryManager.Instance.inventarioItems[targetIndex] = quickItem;
            }
            else if (quickItem.itemData == inventoryItem.itemData && quickItem.itemData.IsStackable)
            {
                Debug.Log($"InventorySlot: Items iguales y apilables, intentando apilar {quickItem.itemData.Name}");
                int maxStack = quickItem.itemData.MaxStackSize;
                int total = quickItem.cantidad + inventoryItem.cantidad;

                if (total <= maxStack)
                {
                    inventoryItem.cantidad = total;
                    QuickInventoryManager.Instance.quickItems[originalIndex] = null;
                    Debug.Log($"InventorySlot: Apilado exitoso, total: {total}");
                }
                else
                {
                    inventoryItem.cantidad = maxStack;
                    quickItem.cantidad = total - maxStack;
                    Debug.Log($"InventorySlot: Stack lleno, inventario: {maxStack}, quick: {total - maxStack}");
                }
            }
            else
            {
                Debug.Log($"InventorySlot: Intercambiando {quickItem.itemData.Name} <-> {inventoryItem.itemData.Name}");
                QuickInventoryManager.Instance.quickItems[originalIndex] = inventoryItem;
                InventoryManager.Instance.inventarioItems[targetIndex] = quickItem;
            }

            QuickInventoryManager.Instance.ActualizarUI();
            InventoryManager.Instance.ActualizarUI();
            draggableItem.MarkAsDropped();
            QuickInventoryManager.Instance.GuardarQuickInventory();
        }
        else
        {
            InventorySlot originalSlot = originalParent.GetComponent<InventorySlot>();
            if (originalSlot == null)
            {
                Debug.LogWarning("InventorySlot: El slot original no tiene componente InventorySlot");
                draggableItem.MarkAsDropped();
                return;
            }

            int originalIndex = originalSlot.slotIndex;

            Debug.Log($"InventorySlot: Intercambiando ítems - Desde slot {originalIndex} hacia slot {targetIndex}");

            draggableItem.parentAfterDrag = transform;
            draggableItem.MarkAsDropped();

            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.IntercambiarItems(originalIndex, targetIndex);
            }
        }
    }

}
