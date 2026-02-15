using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BaseStashItemSlot : MonoBehaviour, IDropHandler
{
    [Header("Prefab")]
    public GameObject itemPrefab;

    private InventoryItem currentItem;
    private GameObject currentItemObject;
    private int globalStashIndex = -1;  // NUEVO: Índice global en el stash

    public void UpdateSlot(InventoryItem item)
    {
        UpdateSlot(item, -1);
    }

    // NUEVO: Versión que acepta el índice del stash
    public void UpdateSlot(InventoryItem item, int stashIndex)
    {
        currentItem = item;
        globalStashIndex = stashIndex;

        if (currentItemObject != null)
        {
            Destroy(currentItemObject);
        }

        if (item != null && item.itemData != null)
        {
            if (itemPrefab == null)
            {
                Debug.LogError("BaseStashItemSlot: itemPrefab no asignado");
                return;
            }

            currentItemObject = Instantiate(itemPrefab, transform);
            currentItemObject.transform.localPosition = Vector3.zero;

            Image itemIcon = currentItemObject.GetComponent<Image>();
            if (itemIcon != null)
            {
                itemIcon.sprite = item.itemData.Icon;
            }

            DraggableItem draggable = currentItemObject.GetComponent<DraggableItem>();
            if (draggable != null)
            {
                draggable.SetItemData(item.itemData, stashIndex);
                draggable.SetQuantity(item.cantidad);
                draggable.isFromBaseStash = true;  // NUEVO: Marcar como del BaseStash
            }
        }
    }

    public void ClearSlot()
    {
        currentItem = null;
        globalStashIndex = -1;

        if (currentItemObject != null)
        {
            Destroy(currentItemObject);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;
        if (droppedObject == null) return;

        DraggableItem draggableItem = droppedObject.GetComponent<DraggableItem>();
        if (draggableItem == null) return;

        if (BaseStashUIManager.Instance != null)
        {
            BaseStashUIManager.Instance.TransferItemToStash(draggableItem);
        }

        draggableItem.MarkAsDropped();
    }
}
