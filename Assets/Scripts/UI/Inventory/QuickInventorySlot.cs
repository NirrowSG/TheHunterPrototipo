using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class QuickInventorySlot : MonoBehaviour, IDropHandler
{
    public GameObject itemPrefab;

    private InventoryItem slotItemActual;
    private GameObject currentItemObject;
    public int slotIndex { get; private set; }


    private void Awake()
    {
        slotIndex = transform.GetSiblingIndex();
        Debug.Log($"QuickInventorySlot: Slot inicializado en índice {slotIndex}");
    }

    private void OnTransformParentChanged()
    {
        slotIndex = transform.GetSiblingIndex();
        Debug.Log($"QuickInventorySlot: Índice actualizado a {slotIndex}");
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
            draggableItem.MarkAsDropped();
            return;
        }

        bool wasFromQuickSlot = draggableItem.isQuickSlot;

        if (wasFromQuickSlot)
        {
            QuickInventorySlot originalSlot = originalParent.GetComponent<QuickInventorySlot>();
            if (originalSlot != null)
            {
                int originalIndex = originalSlot.slotIndex;
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
            else
            {
                Debug.LogWarning("QuickInventorySlot: originalSlot no es QuickInventorySlot");
                draggableItem.MarkAsDropped();
            }
        }
        else
        {
            InventorySlot originalSlot = originalParent.GetComponent<InventorySlot>();
            if (originalSlot == null)
            {
                Debug.LogWarning("QuickInventorySlot: originalSlot no es InventorySlot");
                draggableItem.MarkAsDropped();
                return;
            }

            Debug.Log($"QuickInventorySlot: Moviendo desde inventario principal a quick slot {targetIndex}");

            int originalIndex = originalSlot.slotIndex;

            if (InventoryManager.Instance == null)
            {
                Debug.LogError("QuickInventorySlot: InventoryManager.Instance es null");
                draggableItem.MarkAsDropped();
                return;
            }

            if (QuickInventoryManager.Instance == null)
            {
                Debug.LogError("QuickInventorySlot: QuickInventoryManager.Instance es null");
                draggableItem.MarkAsDropped();
                return;
            }

            if (originalIndex < 0 || originalIndex >= InventoryManager.Instance.inventarioItems.Count)
            {
                Debug.LogError($"QuickInventorySlot: originalIndex {originalIndex} fuera de rango (Count: {InventoryManager.Instance.inventarioItems.Count})");
                draggableItem.MarkAsDropped();
                return;
            }

            if (targetIndex < 0 || targetIndex >= QuickInventoryManager.Instance.quickItems.Count)
            {
                Debug.LogError($"QuickInventorySlot: targetIndex {targetIndex} fuera de rango en QuickInventory (Count: {QuickInventoryManager.Instance.quickItems.Count})");
                draggableItem.MarkAsDropped();
                return;
            }

            InventoryItem inventoryItem = InventoryManager.Instance.inventarioItems[originalIndex];
            InventoryItem quickItem = QuickInventoryManager.Instance.quickItems[targetIndex];

            if (inventoryItem == null)
            {
                Debug.LogWarning($"QuickInventorySlot: Item en inventario principal {originalIndex} es null");
                draggableItem.MarkAsDropped();
                return;
            }

            if (inventoryItem.itemData == null)
            {
                Debug.LogError($"QuickInventorySlot: inventoryItem.itemData es null en slot {originalIndex}");
                draggableItem.MarkAsDropped();
                return;
            }

            if (quickItem == null)
            {
                Debug.Log($"QuickInventorySlot: Slot de destino vacío, moviendo {inventoryItem.itemData.Name}");
                InventoryManager.Instance.inventarioItems[originalIndex] = null;
                QuickInventoryManager.Instance.quickItems[targetIndex] = inventoryItem;
            }
            else if (quickItem.itemData == null)
            {
                Debug.LogWarning($"QuickInventorySlot: quickItem.itemData es null en slot {targetIndex}, reemplazando con item válido");
                InventoryManager.Instance.inventarioItems[originalIndex] = null;
                QuickInventoryManager.Instance.quickItems[targetIndex] = inventoryItem;
            }
            else if (inventoryItem.itemData == quickItem.itemData && inventoryItem.itemData.IsStackable)
            {
                Debug.Log($"QuickInventorySlot: Items iguales y apilables, intentando apilar {inventoryItem.itemData.Name}");
                int maxStack = inventoryItem.itemData.MaxStackSize;
                int total = inventoryItem.cantidad + quickItem.cantidad;

                if (total <= maxStack)
                {
                    quickItem.cantidad = total;
                    InventoryManager.Instance.inventarioItems[originalIndex] = null;
                    Debug.Log($"QuickInventorySlot: Apilado exitoso, total: {total}");
                }
                else
                {
                    quickItem.cantidad = maxStack;
                    inventoryItem.cantidad = total - maxStack;
                    Debug.Log($"QuickInventorySlot: Stack lleno, quick: {maxStack}, inventario: {total - maxStack}");
                }
            }
            else
            {
                Debug.Log($"QuickInventorySlot: Intercambiando {inventoryItem.itemData.Name} <-> {quickItem.itemData.Name}");
                InventoryManager.Instance.inventarioItems[originalIndex] = quickItem;
                QuickInventoryManager.Instance.quickItems[targetIndex] = inventoryItem;
            }

            InventoryManager.Instance.ActualizarUI();
            InventoryManager.Instance.GuardarInventarioDeJugador();  // ✅ AÑADIDO: Guardar inventario principal
            QuickInventoryManager.Instance.ActualizarUI();
            QuickInventoryManager.Instance.GuardarQuickInventory();  // ✅ MOVIDO: Guardar después de ActualizarUI
            draggableItem.MarkAsDropped();
        }
    }



}
