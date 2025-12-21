using UnityEngine;
using System.Collections.Generic;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance;

    [Header("Todos los Items del Juego")]
    public List<ItemDataSO> allItems = new List<ItemDataSO>();

    private Dictionary<string, ItemDataSO> itemDictionary = new Dictionary<string, ItemDataSO>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InicializarDatabase();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InicializarDatabase()
    {
        itemDictionary.Clear();

        foreach (ItemDataSO item in allItems)
        {
            if (item != null && !string.IsNullOrEmpty(item.ID))
            {
                if (!itemDictionary.ContainsKey(item.ID))
                {
                    itemDictionary.Add(item.ID, item);
                }
                else
                {
                    Debug.LogWarning($"ItemDatabase: ID duplicado encontrado - {item.ID}");
                }
            }
        }

        Debug.Log($"ItemDatabase: {itemDictionary.Count} items cargados");
    }

    public ItemDataSO GetItemByID(string id)
    {
        if (itemDictionary.TryGetValue(id, out ItemDataSO item))
        {
            return item;
        }

        Debug.LogWarning($"ItemDatabase: Item con ID '{id}' no encontrado");
        return null;
    }

    public bool ItemExists(string id)
    {
        return itemDictionary.ContainsKey(id);
    }
}
