using UnityEngine;
using System.Collections.Generic;

public class BaseStashManager : MonoBehaviour
{
    public static BaseStashManager Instance;

    private List<InventoryItem> stashItems = new List<InventoryItem>();
    private const int MAX_STASH_SLOTS = 40;

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
        CargarStashDesdeGuardado();
    }

    public void CargarStashDesdeGuardado()
    {
        if (GameDataManager.Instance != null)
        {
            GameSaveData saveData = GameDataManager.Instance.GetSaveData();
            stashItems = GameDataManager.Instance.ConvertirAInventoryItems(saveData.baseStash);

            while (stashItems.Count < MAX_STASH_SLOTS)
            {
                stashItems.Add(null);
            }

            Debug.Log($"BaseStashManager: {ContarItemsEnStash()} items cargados en el stash");
        }
    }

    public void GuardarStash()
    {
        if (GameDataManager.Instance != null)
        {
            GameSaveData saveData = GameDataManager.Instance.GetSaveData();
            saveData.baseStash = GameDataManager.Instance.ConvertirASerializable(stashItems);
            GameDataManager.Instance.GuardarDatos();
            Debug.Log("BaseStashManager: Stash guardado");
        }
    }

    public bool AgregarItemAlStash(ItemDataSO itemData, int cantidad)
    {
        InventoryItem itemExistente = stashItems.Find(x =>
            x != null &&
            x.itemData == itemData &&
            x.itemData.IsStackable &&
            x.cantidad < itemData.MaxStackSize);

        if (itemExistente != null)
        {
            int total = itemExistente.cantidad + cantidad;
            if (total <= itemData.MaxStackSize)
            {
                itemExistente.cantidad = total;
                GuardarStash();
                return true;
            }
            else
            {
                itemExistente.cantidad = itemData.MaxStackSize;
                int restante = total - itemData.MaxStackSize;
                return AgregarItemAlStash(itemData, restante);
            }
        }
        else
        {
            for (int i = 0; i < stashItems.Count; i++)
            {
                if (stashItems[i] == null)
                {
                    stashItems[i] = new InventoryItem(itemData, cantidad);
                    GuardarStash();
                    return true;
                }
            }

            Debug.LogWarning("BaseStashManager: No hay espacio en el stash");
            return false;
        }
    }

    public List<InventoryItem> GetStashItems()
    {
        return stashItems;
    }

    public int ContarItemsEnStash()
    {
        int count = 0;
        foreach (var item in stashItems)
        {
            if (item != null) count++;
        }
        return count;
    }

    public void RemoverItemDelStash(int index)
    {
        if (index >= 0 && index < stashItems.Count)
        {
            stashItems[index] = null;
            GuardarStash();
        }
    }
}
