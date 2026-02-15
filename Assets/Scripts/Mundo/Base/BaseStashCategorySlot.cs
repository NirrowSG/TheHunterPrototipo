using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class BaseStashCategorySlot : MonoBehaviour
{
    [Header("Configuration")]
    public ItemType category;

    [Header("UI References")]
    public Button categoryButton;
    public GameObject expandedPanel;
    public Transform itemsGridContainer;

    [Header("UI Elements")]
    public TextMeshProUGUI categoryNameText;
    public TextMeshProUGUI itemCountText;
    public Image categoryIcon;

    private List<BaseStashItemSlot> itemSlots = new List<BaseStashItemSlot>();
    private bool isExpanded = false;

    private void Awake()
    {
        if (categoryButton != null)
        {
            categoryButton.onClick.AddListener(ToggleExpand);
        }

        if (expandedPanel != null)
        {
            expandedPanel.SetActive(false);
        }

        InitializeSlots();
        UpdateCategoryName();
    }

    private void InitializeSlots()
    {
        itemSlots.Clear();

        if (itemsGridContainer != null)
        {
            foreach (Transform child in itemsGridContainer)
            {
                BaseStashItemSlot slot = child.GetComponent<BaseStashItemSlot>();
                if (slot != null)
                {
                    itemSlots.Add(slot);
                }
            }
        }

        Debug.Log($"BaseStashCategorySlot [{category}]: {itemSlots.Count} slots inicializados");
    }

    private void UpdateCategoryName()
    {
        if (categoryNameText != null)
        {
            categoryNameText.text = GetCategoryDisplayName();
        }
    }

    private string GetCategoryDisplayName()
    {
        switch (category)
        {
            case ItemType.Materials: return "Materiales";
            case ItemType.MeleeWeapon: return "Armas Cuerpo a Cuerpo";
            case ItemType.FireWeapon: return "Armas de Fuego";
            case ItemType.Equipment: return "Equipamiento";
            case ItemType.Consumable: return "Consumibles";
            case ItemType.KeyItem: return "Objetos Clave";
            default: return category.ToString();
        }
    }

    public void ToggleExpand()
    {
        isExpanded = !isExpanded;

        if (expandedPanel != null)
        {
            expandedPanel.SetActive(isExpanded);
        }

        Debug.Log($"BaseStashCategorySlot [{category}]: {(isExpanded ? "Expandido" : "Contraído")}");
    }

    public void UpdateCategory(List<InventoryItem> allItems)
    {
        Debug.Log($"=== BaseStashCategorySlot [{category}]: UpdateCategory llamado ===");
        Debug.Log($"Total items recibidos: {allItems.Count}");

        // Crear lista de pares (item, índice global)
        List<(InventoryItem item, int globalIndex)> categoryItemsWithIndex = new List<(InventoryItem, int)>();

        for (int i = 0; i < allItems.Count; i++)
        {
            if (allItems[i] != null && allItems[i].itemData != null)
            {
                Debug.Log($"Item {i}: {allItems[i].itemData.Name} | Type: {allItems[i].itemData.Type} | Clase: {allItems[i].itemData.GetType().Name}");

                if (allItems[i].itemData.Type == category)
                {
                    categoryItemsWithIndex.Add((allItems[i], i));
                }
            }
        }

        Debug.Log($"Items filtrados para [{category}]: {categoryItemsWithIndex.Count}");

        // Mostrar items filtrados
        foreach (var (item, index) in categoryItemsWithIndex)
        {
            Debug.Log($"  - {item.itemData.Name} (Type: {item.itemData.Type}, Global Index: {index})");
        }

        if (itemCountText != null)
        {
            itemCountText.text = categoryItemsWithIndex.Count.ToString();
        }

        Debug.Log($"Slots disponibles: {itemSlots.Count}");

        for (int i = 0; i < itemSlots.Count; i++)
        {
            if (i < categoryItemsWithIndex.Count)
            {
                var (item, globalIndex) = categoryItemsWithIndex[i];
                Debug.Log($"Actualizando slot {i} con {item.itemData.Name} (índice global: {globalIndex})");
                itemSlots[i].UpdateSlot(item, globalIndex);
            }
            else
            {
                itemSlots[i].ClearSlot();
            }
        }

        Debug.Log($"=== BaseStashCategorySlot [{category}]: Actualización completada ===");
    }
}
