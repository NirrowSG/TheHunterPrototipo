using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance;

    private GameSaveData currentSaveData;
    private string saveFilePath;
    private const string SAVE_FILE_NAME = "gamesave.json";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            saveFilePath = Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
            Debug.Log($"GameDataManager: Ruta de guardado - {saveFilePath}");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        CargarDatos();
    }

    public void CargarDatos()
    {
        if (File.Exists(saveFilePath))
        {
            try
            {
                string json = File.ReadAllText(saveFilePath);
                currentSaveData = JsonUtility.FromJson<GameSaveData>(json);
                Debug.Log($"GameDataManager: Datos cargados exitosamente - {currentSaveData.baseStash.Count} items en stash");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"GameDataManager: Error al cargar datos - {e.Message}");
                currentSaveData = new GameSaveData();
            }
        }
        else
        {
            Debug.Log("GameDataManager: No se encontró archivo de guardado, creando nuevo");
            currentSaveData = new GameSaveData();
        }
    }

    public void GuardarDatos()
    {
        try
        {
            currentSaveData.lastSaveTime = System.DateTime.Now;
            string json = JsonUtility.ToJson(currentSaveData, true);
            File.WriteAllText(saveFilePath, json);
            Debug.Log($"GameDataManager: Datos guardados exitosamente en {saveFilePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"GameDataManager: Error al guardar datos - {e.Message}");
        }
    }

    public GameSaveData GetSaveData()
    {
        return currentSaveData;
    }

    public void BorrarDatos()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
            Debug.Log("GameDataManager: Datos borrados");
        }
        currentSaveData = new GameSaveData();
    }

    public List<InventoryItem> ConvertirAInventoryItems(List<SerializableInventoryItem> serializableItems)
    {
        List<InventoryItem> items = new List<InventoryItem>();

        foreach (var serItem in serializableItems)
        {
            ItemDataSO itemData = ItemDatabase.Instance.GetItemByID(serItem.itemID);
            if (itemData != null)
            {
                items.Add(new InventoryItem(itemData, serItem.cantidad));
            }
            else
            {
                Debug.LogWarning($"GameDataManager: No se pudo cargar item con ID {serItem.itemID}");
            }
        }

        return items;
    }

    public List<SerializableInventoryItem> ConvertirASerializable(List<InventoryItem> items)
    {
        List<SerializableInventoryItem> serializableItems = new List<SerializableInventoryItem>();

        foreach (var item in items)
        {
            if (item != null && item.itemData != null)
            {
                serializableItems.Add(new SerializableInventoryItem(item.itemData.ID, item.cantidad));
            }
        }

        return serializableItems;
    }

    private void OnApplicationQuit()
    {
        GuardarDatos();
    }
}
