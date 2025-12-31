using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class EquipmentSlot : MonoBehaviour, IDropHandler
{
    [Header("Slot Configuration")]
    public EquipmentSlotType slotType;

    [Header("UI References")]
    public Image itemIcon;
    public TextMeshProUGUI slotLabel;

    private ItemDataSO currentItem;

    private void Start()
    {
        ActualizarSlot();
    }

    public void ActualizarSlot()
    {
        if (EquipmentManager.Instance != null)
        {
            currentItem = EquipmentManager.Instance.GetEquippedItem(slotType);
        }

        if (currentItem != null)
        {
            if (itemIcon != null)
            {
                itemIcon.sprite = currentItem.Icon;
                itemIcon.enabled = true;
            }
        }
        else
        {
            if (itemIcon != null)
            {
                itemIcon.sprite = null;
                itemIcon.enabled = false;
            }
        }

        if (slotLabel != null)
        {
            slotLabel.text = GetSlotName(slotType);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        if (dropped == null) return;

        DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();
        if (draggableItem == null)
        {
            Debug.LogWarning("EquipmentSlot: El objeto arrastrado no tiene DraggableItem");
            return;
        }

        Transform originalParent = draggableItem.parentAfterDrag;
        if (originalParent == null)
        {
            Debug.LogWarning("EquipmentSlot: parentAfterDrag es null");
            return;
        }

        InventorySlot inventorySlot = originalParent.GetComponent<InventorySlot>();
        if (inventorySlot == null)
        {
            Debug.LogWarning("EquipmentSlot: Solo puedes equipar items desde el inventario principal");
            return;
        }

        int originalIndex = inventorySlot.slotIndex;
        if (InventoryManager.Instance == null)
        {
            Debug.LogError("EquipmentSlot: InventoryManager.Instance es null");
            return;
        }

        if (originalIndex < 0 || originalIndex >= InventoryManager.Instance.inventarioItems.Count)
        {
            Debug.LogError($"EquipmentSlot: Índice fuera de rango {originalIndex}");
            return;
        }

        InventoryItem inventoryItem = InventoryManager.Instance.inventarioItems[originalIndex];
        if (inventoryItem == null || inventoryItem.itemData == null)
        {
            Debug.LogWarning("EquipmentSlot: El item del inventario es null");
            return;
        }

        EquipmentItemSO equipmentItem = inventoryItem.itemData as EquipmentItemSO;
        if (equipmentItem == null)
        {
            Debug.LogWarning($"EquipmentSlot: {inventoryItem.itemData.Name} no es un item equipable");
            return;
        }

        if (equipmentItem.equipmentSlot != slotType)
        {
            Debug.LogWarning($"EquipmentSlot: {equipmentItem.Name} no puede ser equipado en slot {slotType}");
            return;
        }

        InventoryManager.Instance.inventarioItems[originalIndex] = null;

        if (EquipmentManager.Instance != null)
        {
            EquipmentManager.Instance.EquipItem(equipmentItem);
        }

        InventoryManager.Instance.ActualizarUI();
        InventoryManager.Instance.GuardarInventarioDeJugador();  // ✅ AÑADIDO: Guardar inventario

        ActualizarSlot();

        if (EquipmentUIManager.Instance != null)
        {
            EquipmentUIManager.Instance.ActualizarTodosLosSlots();
        }

        draggableItem.MarkAsDropped();

        Debug.Log($"EquipmentSlot: {equipmentItem.Name} equipado en {slotType}");
    }


    public void OnClickSlot()
    {
        if (currentItem == null)
        {
            Debug.Log($"EquipmentSlot: No hay item equipado en {slotType}");
            return;
        }

        if (InventoryManager.Instance == null || EquipmentManager.Instance == null)
        {
            Debug.LogError("EquipmentSlot: Managers no disponibles");
            return;
        }

        int slotsVacios = 0;
        foreach (var item in InventoryManager.Instance.inventarioItems)
        {
            if (item == null) slotsVacios++;
        }

        Debug.Log($"EquipmentSlot: Slots vacíos disponibles: {slotsVacios}");

        if (slotsVacios == 0)
        {
            Debug.LogWarning("EquipmentSlot: No hay espacio en el inventario");
            return;
        }

        string itemName = currentItem.Name;

        InventoryManager.Instance.AgregarItem(currentItem, 1);
        EquipmentManager.Instance.UnequipItem(slotType);
        ActualizarSlot();

        if (EquipmentUIManager.Instance != null)
        {
            EquipmentUIManager.Instance.ActualizarTodosLosSlots();
        }

        Debug.Log($"EquipmentSlot: {itemName} desequipado y devuelto al inventario");
    }





    private string GetSlotName(EquipmentSlotType type)
    {
        switch (type)
        {
            case EquipmentSlotType.Head:
                return "Cabeza";
            case EquipmentSlotType.Torso:
                return "Torso";
            case EquipmentSlotType.Hands:
                return "Manos";
            case EquipmentSlotType.Legs:
                return "Piernas";
            case EquipmentSlotType.Feet:
                return "Pies";
            case EquipmentSlotType.MeleeWeapon:
                return "Arma C.C.";
            case EquipmentSlotType.FireWeapon:
                return "Arma Fuego";
            default:
                return type.ToString();
        }
    }
}
