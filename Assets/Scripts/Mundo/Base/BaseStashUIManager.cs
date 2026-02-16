using UnityEngine;
using System.Collections.Generic;

public class BaseStashUIManager : MonoBehaviour
{
    public static BaseStashUIManager Instance;

    [Header("UI References")]
    public GameObject baseStashPanel;
    public List<BaseStashCategorySlot> categorySlots = new List<BaseStashCategorySlot>();

    [Header("Controles de Juego")]
    [Tooltip("Joystick que se desactivará cuando el almacenamiento esté abierto")]
    public GameObject joystick;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (baseStashPanel != null)
        {
            baseStashPanel.SetActive(false);
        }
    }

    public void OpenStashUI()
    {
        if (baseStashPanel != null)
        {
            baseStashPanel.SetActive(true);
            UpdateAllCategories();

            if (joystick != null)
            {
                joystick.SetActive(false);
                //Debug.Log("BaseStashUIManager: Joystick desactivado");
            }
        }
    }

    public void CloseStashUI()
    {
        if (baseStashPanel != null)
        {
            baseStashPanel.SetActive(false);

            if (joystick != null)
            {
                joystick.SetActive(true);
                //Debug.Log("BaseStashUIManager: Joystick activado");
            }

            // NUEVO: Notificar al BaseStashInteractable que el panel se cerró
            BaseStashInteractable interactable = FindObjectOfType<BaseStashInteractable>();
            if (interactable != null)
            {
                interactable.OnStashClosed();
                //Debug.Log("BaseStashUIManager: BaseStashInteractable notificado del cierre");
            }
        }
    }


    public void UpdateAllCategories()
    {
        if (BaseStashManager.Instance == null)
        {
            Debug.LogWarning("BaseStashUIManager: BaseStashManager no disponible");
            return;
        }

        List<InventoryItem> allStashItems = BaseStashManager.Instance.GetStashItems();

        foreach (var categorySlot in categorySlots)
        {
            if (categorySlot != null)
            {
                categorySlot.UpdateCategory(allStashItems);
            }
        }

        //Debug.Log("BaseStashUIManager: Todas las categorías actualizadas");
    }

    public void OnCategoryExpanded(BaseStashCategorySlot expandedCategory)
    {
        //Debug.Log($"BaseStashUIManager: Categoría {expandedCategory.category} expandida, cerrando las demás");

        foreach (var categorySlot in categorySlots)
        {
            if (categorySlot != null && categorySlot != expandedCategory)
            {
                categorySlot.Collapse();
            }
        }
    }

    public void TransferItemToStash(DraggableItem draggableItem)
    {
        if (draggableItem == null || draggableItem.itemData == null) return;

        int sourceSlotIndex = draggableItem.slotIndex;

        if (draggableItem.isQuickSlot)
        {
            TransferFromQuickInventory(sourceSlotIndex);
        }
        else
        {
            TransferFromMainInventory(sourceSlotIndex);
        }
    }

    private void TransferFromMainInventory(int slotIndex)
    {
        if (InventoryManager.Instance == null) return;

        if (slotIndex < 0 || slotIndex >= InventoryManager.Instance.inventarioItems.Count) return;

        InventoryItem item = InventoryManager.Instance.inventarioItems[slotIndex];

        if (item != null && item.itemData != null)
        {
            bool success = BaseStashManager.Instance.AgregarItemAlStash(item.itemData, item.cantidad);

            if (success)
            {
                InventoryManager.Instance.inventarioItems[slotIndex] = null;
                InventoryManager.Instance.ActualizarUI();
                InventoryManager.Instance.GuardarInventarioDeJugador();
                UpdateAllCategories();
                //Debug.Log($"BaseStashUIManager: {item.itemData.Name} transferido al stash");
            }
        }
    }

    private void TransferFromQuickInventory(int slotIndex)
    {
        if (QuickInventoryManager.Instance == null) return;

        if (slotIndex < 0 || slotIndex >= QuickInventoryManager.Instance.quickItems.Count) return;

        InventoryItem item = QuickInventoryManager.Instance.quickItems[slotIndex];

        if (item != null && item.itemData != null)
        {
            bool success = BaseStashManager.Instance.AgregarItemAlStash(item.itemData, item.cantidad);

            if (success)
            {
                QuickInventoryManager.Instance.quickItems[slotIndex] = null;
                QuickInventoryManager.Instance.ActualizarUI();
                QuickInventoryManager.Instance.GuardarQuickInventory();
                UpdateAllCategories();
                //Debug.Log($"BaseStashUIManager: {item.itemData.Name} transferido al stash desde quick inventory");
            }
        }
    }
}
