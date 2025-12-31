using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager Instance;

    public Dictionary<EquipmentSlotType, ItemDataSO> equippedItems = new Dictionary<EquipmentSlotType, ItemDataSO>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InicializarSlots();
            Debug.Log("EquipmentManager: Instancia creada y marcada como persistente");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        CargarEquipamiento();
    }

    private void InicializarSlots()
    {
        equippedItems[EquipmentSlotType.Head] = null;
        equippedItems[EquipmentSlotType.Torso] = null;
        equippedItems[EquipmentSlotType.Hands] = null;
        equippedItems[EquipmentSlotType.Legs] = null;
        equippedItems[EquipmentSlotType.Feet] = null;
        equippedItems[EquipmentSlotType.MeleeWeapon] = null;
        equippedItems[EquipmentSlotType.FireWeapon] = null;
    }

    public bool EquipItem(EquipmentItemSO item)
    {
        if (item == null)
        {
            Debug.LogWarning("EquipmentManager: Item es null");
            return false;
        }

        ItemDataSO previousItem = null;
        if (equippedItems.ContainsKey(item.equipmentSlot))
        {
            previousItem = equippedItems[item.equipmentSlot];
        }

        equippedItems[item.equipmentSlot] = item;

        if (previousItem != null)
        {
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.AgregarItem(previousItem, 1);
            }
            Debug.Log($"EquipmentManager: {previousItem.Name} desequipado y devuelto al inventario");
        }

        if (PlayerStatsManager.Instance != null)
        {
            PlayerStatsManager.Instance.RecalcularStats();
        }

        GuardarEquipamiento();
        Debug.Log($"EquipmentManager: {item.Name} equipado en slot {item.equipmentSlot}");
        return true;
    }

    public ItemDataSO UnequipItem(EquipmentSlotType slotType)
    {
        if (!equippedItems.ContainsKey(slotType) || equippedItems[slotType] == null)
        {
            Debug.LogWarning($"EquipmentManager: No hay item equipado en slot {slotType}");
            return null;
        }

        ItemDataSO item = equippedItems[slotType];
        equippedItems[slotType] = null;

        if (PlayerStatsManager.Instance != null)
        {
            PlayerStatsManager.Instance.RecalcularStats();
        }

        GuardarEquipamiento();
        Debug.Log($"EquipmentManager: {item.Name} desequipado de slot {slotType}");
        return item;
    }

    public ItemDataSO GetEquippedItem(EquipmentSlotType slotType)
    {
        if (equippedItems.ContainsKey(slotType))
        {
            return equippedItems[slotType];
        }
        return null;
    }

    public void GuardarEquipamiento()
    {
        if (GameDataManager.Instance != null)
        {
            GameSaveData saveData = GameDataManager.Instance.GetSaveData();
            saveData.equipment.Clear();

            foreach (var kvp in equippedItems)
            {
                if (kvp.Value != null)
                {
                    saveData.equipment.Add(new SerializableEquipmentSlot(kvp.Value.ID, kvp.Key));
                }
            }

            GameDataManager.Instance.GuardarDatos();
            Debug.Log($"EquipmentManager: Equipamiento guardado - {saveData.equipment.Count} items equipados");
        }
    }

    public void CargarEquipamiento()
    {
        if (GameDataManager.Instance != null && ItemDatabase.Instance != null)
        {
            GameSaveData saveData = GameDataManager.Instance.GetSaveData();

            foreach (var slot in saveData.equipment)
            {
                ItemDataSO item = ItemDatabase.Instance.GetItemByID(slot.itemID);
                if (item != null)
                {
                    equippedItems[slot.slotType] = item;
                }
            }

            if (PlayerStatsManager.Instance != null)
            {
                PlayerStatsManager.Instance.RecalcularStats();
            }

            Debug.Log($"EquipmentManager: Equipamiento cargado - {saveData.equipment.Count} items");
        }
    }
}
