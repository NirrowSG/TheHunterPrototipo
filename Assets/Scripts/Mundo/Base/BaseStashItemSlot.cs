using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BaseStashItemSlot : MonoBehaviour, IDropHandler
{
    [Header("Prefab")]
    public GameObject itemPrefab;

    private InventoryItem currentItem;
    private GameObject currentItemObject;
    private DraggableItem currentDraggableItem;
    private Image currentItemImage;
    private int globalStashIndex = -1;

    public void UpdateSlot(InventoryItem item)
    {
        UpdateSlot(item, -1);
    }

    public void UpdateSlot(InventoryItem item, int stashIndex)
    {
        currentItem = item;
        globalStashIndex = stashIndex;

        if (item != null && item.itemData != null)
        {
            // OPTIMIZACIÓN: Crear el objeto solo una vez y reutilizarlo
            if (currentItemObject == null)
            {
                if (itemPrefab == null)
                {
                    Debug.LogError("BaseStashItemSlot: itemPrefab no asignado");
                    return;
                }

                currentItemObject = Instantiate(itemPrefab, transform);
                currentItemObject.transform.localPosition = Vector3.zero;
                currentDraggableItem = currentItemObject.GetComponent<DraggableItem>();
                currentItemImage = currentItemObject.GetComponent<Image>();
            }

            // Actualizar los datos del item existente
            if (currentItemImage != null && item.itemData.Icon != null)
            {
                currentItemImage.sprite = item.itemData.Icon;
                currentItemImage.enabled = true;
            }

            if (currentDraggableItem != null)
            {
                currentDraggableItem.SetItemData(item.itemData, stashIndex);
                currentDraggableItem.SetQuantity(item.cantidad);
                currentDraggableItem.isFromBaseStash = true;
            }

            currentItemObject.SetActive(true);
        }
        else
        {
            // OPTIMIZACIÓN: Desactivar en lugar de destruir
            ClearSlot();
        }
    }

    public void ClearSlot()
    {
        currentItem = null;
        globalStashIndex = -1;

        // OPTIMIZACIÓN: Desactivar en lugar de destruir
        if (currentItemObject != null)
        {
            currentItemObject.SetActive(false);
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
