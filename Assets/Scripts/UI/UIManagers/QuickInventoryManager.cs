using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class QuickInventoryManager : MonoBehaviour
{
    public static QuickInventoryManager Instance;

    public List<InventoryItem> quickItems = new List<InventoryItem>();
    public QuickInventorySlot[] quickSlots;
    public Transform quickSlotsPadre;

    private bool yaInicializado = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(transform.root.gameObject);  // ← Cambio aquí (línea 20)
            Debug.Log("QuickInventoryManager: Canvas raíz marcado como persistente");
        }
        else if (Instance != this)
        {
            Debug.Log("QuickInventoryManager: Instancia duplicada encontrada, destruyendo Canvas duplicado...");
            Destroy(transform.root.gameObject);  // ← Cambio aquí (línea 26)
            return;
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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"QuickInventoryManager: Escena {scene.name} cargada, reinicializando slots...");
        ReinicializarSlots();

        if (GameDataManager.Instance != null)
        {
            GameSaveData saveData = GameDataManager.Instance.GetSaveData();

            if (saveData != null && saveData.quickInventory != null && saveData.quickInventory.Count > 0)
            {
                Debug.Log("QuickInventoryManager: Recargando QuickInventory desde datos guardados tras cambio de escena...");
                CargarQuickInventory();
            }
            else
            {
                Debug.Log("QuickInventoryManager: No hay datos de QuickInventory guardados, solo actualizando UI");
            }
        }
        else
        {
            Debug.LogWarning("QuickInventoryManager: GameDataManager no disponible aún");
        }
    }

    void Start()
    {
        InicializarQuickInventory();
    }

    private void InicializarQuickInventory()
    {
        if (quickSlotsPadre == null)
        {
            Debug.LogError("QuickInventoryManager: quickSlotsPadre no está asignado");
            return;
        }

        quickSlots = quickSlotsPadre.GetComponentsInChildren<QuickInventorySlot>(true);
        Debug.Log($"QuickInventoryManager: Se encontraron {quickSlots.Length} quick slots");

        CargarQuickInventory();

        Debug.Log($"QuickInventoryManager: QuickInventory inicializado con {quickItems.Count} espacios");
        yaInicializado = true;
    }

    private void ReinicializarSlots()
    {
        if (quickSlotsPadre == null)
        {
            Debug.LogWarning("QuickInventoryManager: quickSlotsPadre es null, no se puede reinicializar");
            return;
        }

        quickSlots = quickSlotsPadre.GetComponentsInChildren<QuickInventorySlot>(true);
        Debug.Log($"QuickInventoryManager: Slots reinicializados - {quickSlots.Length} slots encontrados");

        ActualizarUI();
    }

    public void ActualizarUI()
    {
        if (quickSlots == null || quickSlots.Length == 0)
        {
            Debug.LogWarning("QuickInventoryManager: Slots no inicializados, intentando reinicializar...");
            if (quickSlotsPadre != null)
            {
                quickSlots = quickSlotsPadre.GetComponentsInChildren<QuickInventorySlot>(true);
            }

            if (quickSlots == null || quickSlots.Length == 0)
            {
                Debug.LogError("QuickInventoryManager: No se pudieron reinicializar los slots");
                return;
            }
        }

        Debug.Log($"QuickInventoryManager: ActualizarUI llamado - {quickSlots.Length} slots, {quickItems.Count} items");

        for (int i = 0; i < quickSlots.Length; i++)
        {
            if (quickSlots[i] != null)
            {
                if (i < quickItems.Count)
                {
                    quickSlots[i].ActualizarSlot(quickItems[i]);
                }
                else
                {
                    quickSlots[i].LimpiarSlot();
                }
            }
        }
    }

    public void IntercambiarItems(int indexA, int indexB)
    {
        Debug.Log($"QuickInventoryManager: IntercambiarItems llamado - {indexA} <-> {indexB}, quickItems.Count = {quickItems.Count}");

        if (indexA < 0 || indexA >= quickItems.Count ||
            indexB < 0 || indexB >= quickItems.Count)
        {
            Debug.LogWarning($"QuickInventoryManager: Índices fuera de rango - indexA:{indexA}, indexB:{indexB}, Count:{quickItems.Count}");
            return;
        }

        if (indexA == indexB)
        {
            Debug.Log("QuickInventoryManager: Mismo slot, no se realiza ninguna acción");
            return;
        }

        InventoryItem itemA = quickItems[indexA];
        InventoryItem itemB = quickItems[indexB];

        if (itemA == null)
        {
            Debug.LogWarning("QuickInventoryManager: Item origen es null");
            return;
        }

        if (itemA.itemData == null)
        {
            Debug.LogError($"QuickInventoryManager: itemA.itemData es null en slot {indexA}, eliminando item corrupto");
            quickItems[indexA] = null;
            ActualizarUI();
            return;
        }

        if (itemB == null)
        {
            Debug.Log($"QuickInventoryManager: Moviendo item de slot {indexA} a slot vacío {indexB}");
            quickItems[indexB] = itemA;
            quickItems[indexA] = null;
            ActualizarUI();
            GuardarQuickInventory();
            return;
        }

        if (itemB.itemData == null)
        {
            Debug.LogError($"QuickInventoryManager: itemB.itemData es null en slot {indexB}, reemplazando con item válido");
            quickItems[indexB] = itemA;
            quickItems[indexA] = null;
            ActualizarUI();
            GuardarQuickInventory();
            return;
        }

        if (itemA.itemData == itemB.itemData && itemA.itemData.IsStackable)
        {
            Debug.Log($"QuickInventoryManager: Intentando apilar {itemA.itemData.Name}");
            int maxStack = itemA.itemData.MaxStackSize;
            int total = itemA.cantidad + itemB.cantidad;

            if (total <= maxStack)
            {
                itemB.cantidad = total;
                quickItems[indexA] = null;
            }
            else
            {
                itemB.cantidad = maxStack;
                itemA.cantidad = total - maxStack;
            }
        }
        else
        {
            Debug.Log($"QuickInventoryManager: Intercambiando items de slot {indexA} y slot {indexB}");
            InventoryItem temp = quickItems[indexA];
            quickItems[indexA] = quickItems[indexB];
            quickItems[indexB] = temp;
        }

        ActualizarUI();
        GuardarQuickInventory();
    }

    public void AgregarItemEnSlot(InventoryItem item, int slotIndex)
    {
        Debug.Log($"QuickInventoryManager: AgregarItemEnSlot - slotIndex:{slotIndex}, quickItems.Count:{quickItems.Count}");

        if (slotIndex < 0 || slotIndex >= quickItems.Count)
        {
            Debug.LogWarning($"QuickInventoryManager: Índice inválido - slotIndex:{slotIndex}, Count:{quickItems.Count}");
            return;
        }

        quickItems[slotIndex] = item;
        Debug.Log($"QuickInventoryManager: Item {item.itemData.Name} agregado en slot {slotIndex}");
        ActualizarUI();
        GuardarQuickInventory();
    }

    public InventoryItem RemoverItemDeSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= quickItems.Count)
        {
            Debug.LogWarning($"QuickInventoryManager: Índice inválido al remover - slotIndex:{slotIndex}, Count:{quickItems.Count}");
            return null;
        }

        InventoryItem item = quickItems[slotIndex];
        quickItems[slotIndex] = null;
        ActualizarUI();
        GuardarQuickInventory();
        return item;
    }

    public void GuardarQuickInventory()
    {
        if (GameDataManager.Instance != null)
        {
            GameSaveData saveData = GameDataManager.Instance.GetSaveData();
            saveData.quickInventory = GameDataManager.Instance.ConvertirASerializable(quickItems);
            GameDataManager.Instance.GuardarDatos();
            Debug.Log($"QuickInventoryManager: QuickInventory guardado - {quickItems.Count} slots");
        }
    }

    public void CargarQuickInventory()
    {
        if (GameDataManager.Instance != null)
        {
            GameSaveData saveData = GameDataManager.Instance.GetSaveData();
            quickItems = GameDataManager.Instance.ConvertirAInventoryItems(saveData.quickInventory);

            while (quickItems.Count < quickSlots.Length)
            {
                quickItems.Add(null);
            }

            LimpiarItemsCorruptos();
            ActualizarUI();
            Debug.Log($"QuickInventoryManager: QuickInventory cargado - {quickItems.Count} slots");
        }
    }

    private void LimpiarItemsCorruptos()
    {
        int itemsLimpiados = 0;
        for (int i = 0; i < quickItems.Count; i++)
        {
            if (quickItems[i] != null && quickItems[i].itemData == null)
            {
                Debug.LogWarning($"QuickInventoryManager: Item corrupto encontrado en slot {i}, limpiando...");
                quickItems[i] = null;
                itemsLimpiados++;
            }
        }

        if (itemsLimpiados > 0)
        {
            Debug.Log($"QuickInventoryManager: {itemsLimpiados} items corruptos limpiados");
            GuardarQuickInventory();
        }
    }
}
