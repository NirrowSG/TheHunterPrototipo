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

        Debug.Log($"ItemDatabase: Inicializando con {allItems.Count} items...");

        foreach (ItemDataSO item in allItems)
        {
            if (item != null && !string.IsNullOrEmpty(item.ID))
            {
                if (!itemDictionary.ContainsKey(item.ID))
                {
                    itemDictionary.Add(item.ID, item);
                    Debug.Log($"ItemDatabase: Item registrado - ID: {item.ID}, Name: {item.Name}");
                }
                else
                {
                    Debug.LogWarning($"ItemDatabase: ID duplicado encontrado - {item.ID}");
                }
            }
            else
            {
                Debug.LogError($"ItemDatabase: Item null o con ID vacío detectado");
            }
        }

        Debug.Log($"ItemDatabase: {itemDictionary.Count} items cargados en diccionario");
    }

    public ItemDataSO GetItemByID(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            Debug.LogError("ItemDatabase: ID es null o vacío");
            return null;
        }

        if (itemDictionary.TryGetValue(id, out ItemDataSO item))
        {
            return item;
        }

        Debug.LogWarning($"ItemDatabase: Item con ID '{id}' no encontrado en diccionario de {itemDictionary.Count} items");
        return null;
    }

    public bool ItemExists(string id)
    {
        return itemDictionary.ContainsKey(id);
    }
}
