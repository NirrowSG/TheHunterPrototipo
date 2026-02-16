using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class TransferToInventorySlot : MonoBehaviour, IDropHandler
{
    [Header("Visual Feedback")]
    public Image slotImage;
    public Color normalColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
    public Color highlightColor = new Color(0.3f, 0.6f, 0.3f, 0.8f);

    private void Start()
    {
        if (slotImage != null)
        {
            slotImage.color = normalColor;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;
        if (droppedObject == null)
        {
            Debug.LogWarning("TransferToInventorySlot: Objeto soltado es null");
            return;
        }

        DraggableItem draggableItem = droppedObject.GetComponent<DraggableItem>();
        if (draggableItem == null)
        {
            Debug.LogWarning("TransferToInventorySlot: No tiene componente DraggableItem");
            return;
        }

        if (!draggableItem.isFromBaseStash)
        {
            Debug.LogWarning("TransferToInventorySlot: Este item no viene del BaseStash");
            return;
        }

        //Debug.Log($"TransferToInventorySlot: Transfiriendo {draggableItem.itemData.Name} al inventario del jugador");

        TransferirItemAlInventario(draggableItem);
        draggableItem.MarkAsDropped();
    }

    private void TransferirItemAlInventario(DraggableItem draggableItem)
    {
        if (draggableItem.itemData == null)
        {
            Debug.LogError("TransferToInventorySlot: itemData es null");
            return;
        }

        int stashIndex = draggableItem.slotIndex;

        if (BaseStashManager.Instance == null)
        {
            Debug.LogError("TransferToInventorySlot: BaseStashManager no disponible");
            return;
        }

        if (InventoryManager.Instance == null)
        {
            Debug.LogError("TransferToInventorySlot: InventoryManager no disponible");
            return;
        }

        List<InventoryItem> stashItems = BaseStashManager.Instance.GetStashItems();

        if (stashIndex < 0 || stashIndex >= stashItems.Count)
        {
            Debug.LogError($"TransferToInventorySlot: Índice {stashIndex} fuera de rango");
            return;
        }

        InventoryItem itemDelStash = stashItems[stashIndex];

        if (itemDelStash == null || itemDelStash.itemData == null)
        {
            Debug.LogError("TransferToInventorySlot: Item del stash es null");
            return;
        }

        int slotVacioEnInventario = EncontrarSlotVacio();

        if (slotVacioEnInventario == -1)
        {
            Debug.LogWarning("TransferToInventorySlot: No hay espacio en el inventario del jugador");
            return;
        }

        InventoryManager.Instance.inventarioItems[slotVacioEnInventario] = new InventoryItem(itemDelStash.itemData, itemDelStash.cantidad);

        BaseStashManager.Instance.RemoverItemDelStash(stashIndex);

        InventoryManager.Instance.ActualizarUI();
        InventoryManager.Instance.GuardarInventarioDeJugador();

        if (BaseStashUIManager.Instance != null)
        {
            BaseStashUIManager.Instance.UpdateAllCategories();
        }

        //Debug.Log($"TransferToInventorySlot: {itemDelStash.itemData.Name} x{itemDelStash.cantidad} transferido al inventario slot {slotVacioEnInventario}");
    }

    private int EncontrarSlotVacio()
    {
        if (InventoryManager.Instance == null) return -1;

        for (int i = 0; i < InventoryManager.Instance.inventarioItems.Count; i++)
        {
            if (InventoryManager.Instance.inventarioItems[i] == null)
            {
                return i;
            }
        }

        return -1;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (slotImage != null)
        {
            slotImage.color = highlightColor;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (slotImage != null)
        {
            slotImage.color = normalColor;
        }
    }
}
