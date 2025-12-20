using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        // 1. Verificamos si lo que cayó encima es un objeto arrastrable
        GameObject droppedObj = eventData.pointerDrag;
        DraggableItem draggableItem = droppedObj.GetComponent<DraggableItem>();

        if (draggableItem != null)
        {
            // Opcional: Solo aceptar si el slot está vacío
            if (transform.childCount == 0)
            {
                // 2. Le decimos a la madera: "Tu nuevo padre soy yo (este slot)"
                draggableItem.parentAfterDrag = transform;
            }
        }
    }
}