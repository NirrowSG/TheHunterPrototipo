using UnityEngine;
using UnityEngine.SceneManagement;
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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        CargarDatos();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"GameDataManager: Escena '{scene.name}' cargada, restaurando datos");
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

        Debug.Log($"GameDataManager: Convirtiendo {serializableItems.Count} items serializables...");

        if (ItemDatabase.Instance == null)
        {
            Debug.LogError("GameDataManager: ItemDatabase.Instance es NULL!");
            return items;
        }

        foreach (var serItem in serializableItems)
        {
            if (serItem == null || string.IsNullOrEmpty(serItem.itemID))
            {
                items.Add(null);
                continue;
            }

            Debug.Log($"GameDataManager: Buscando item con ID: '{serItem.itemID}'");
            ItemDataSO itemData = ItemDatabase.Instance.GetItemByID(serItem.itemID);

            if (itemData != null)
            {
                items.Add(new InventoryItem(itemData, serItem.cantidad));
                Debug.Log($"GameDataManager: Item '{itemData.Name}' cargado exitosamente");
            }
            else
            {
                Debug.LogWarning($"GameDataManager: Item con ID '{serItem.itemID}' no encontrado en ItemDatabase, agregando slot vacío");
                items.Add(null);
            }
        }

        Debug.Log($"GameDataManager: Conversión completada - {items.Count} items en lista");
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
