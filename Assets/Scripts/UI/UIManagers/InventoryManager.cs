using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public List<InventoryItem> inventarioItems = new List<InventoryItem>();
    public InventorySlot[] slots;
    public Transform slotsPadre;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("InventoryManager: Instancia creada y marcada como persistente");
        }
        else if (Instance != this)
        {
            Debug.Log("InventoryManager: Instancia duplicada encontrada, destruyendo...");
            Destroy(gameObject);
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
        Debug.Log($"InventoryManager: Escena {scene.name} cargada, reinicializando...");
        ReinicializarSlots();

        if (GameDataManager.Instance != null)
        {
            GameSaveData saveData = GameDataManager.Instance.GetSaveData();

            if (saveData != null && saveData.playerInventory != null && saveData.playerInventory.Count > 0)
            {
                Debug.Log("InventoryManager: Recargando inventario desde datos guardados tras cambio de escena...");
                CargarInventarioDeJugador();
            }
            else
            {
                Debug.Log("InventoryManager: No hay datos guardados, solo actualizando UI");
            }
        }
        else
        {
            Debug.LogWarning("InventoryManager: GameDataManager no disponible aún");
        }
    }



    void Start()
    {
        InicializarInventario();

        if (GameDataManager.Instance != null && GameDataManager.Instance.GetSaveData().playerInventory.Count > 0)
        {
            CargarInventarioDeJugador();
            Debug.Log("InventoryManager: Inventario cargado desde datos guardados");
        }
    }

    private void ReinicializarSlots()
    {
        if (slotsPadre == null)
        {
            Debug.LogWarning("InventoryManager: slotsPadre es null, buscando en escena...");
            GameObject slotsContainer = GameObject.Find("SlotsContainer");
            if (slotsContainer != null)
            {
                slotsPadre = slotsContainer.transform;
            }
        }

        if (slotsPadre != null)
        {
            slots = slotsPadre.GetComponentsInChildren<InventorySlot>(true);
            Debug.Log($"InventoryManager: Slots reinicializados - {slots.Length} slots encontrados");
            ActualizarUI();
        }
    }

    private void InicializarInventario()
    {
        if (slotsPadre == null)
        {
            Debug.LogError("InventoryManager: slotsPadre no está asignado");
            return;
        }

        slots = slotsPadre.GetComponentsInChildren<InventorySlot>(true);
        Debug.Log($"InventoryManager: Se encontraron {slots.Length} slots");

        while (inventarioItems.Count < slots.Length)
        {
            inventarioItems.Add(null);
        }

        Debug.Log($"InventoryManager: Lista inicializada con {inventarioItems.Count} espacios");
        ActualizarUI();
    }

    public void ActualizarUI()
    {
        if (slots == null || slots.Length == 0)
        {
            Debug.LogWarning("InventoryManager: Slots no inicializados, intentando inicializar...");
            ReinicializarSlots();
            return;
        }

        Debug.Log("InventoryManager: ActualizarUI llamado");
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null)
            {
                if (i < inventarioItems.Count)
                {
                    slots[i].ActualizarSlot(inventarioItems[i]);
                }
                else
                {
                    slots[i].LimpiarSlot();
                }
            }
        }
    }

    public void AgregarItem(ItemDataSO item, int cantidadAgregada)
    {
        Debug.Log($"InventoryManager: Intentando agregar {cantidadAgregada}x {item.Name}");

        InventoryItem itemExistente = inventarioItems.Find(x =>
            x != null &&
            x.itemData == item &&
            x.itemData.IsStackable &&
            x.cantidad < 99);

        if (itemExistente != null)
        {
            Debug.Log($"InventoryManager: Item existente encontrado, cantidad actual: {itemExistente.cantidad}");
            int total = itemExistente.cantidad + cantidadAgregada;

            if (total <= 99)
            {
                itemExistente.AumentarCantidad(cantidadAgregada);
                Debug.Log($"InventoryManager: Cantidad aumentada a {itemExistente.cantidad}");
            }
            else
            {
                int sobrante = total - 99;
                itemExistente.cantidad = 99;
                AgregarNuevoItem(item, sobrante);
            }
        }
        else
        {
            Debug.Log("InventoryManager: No se encontró item existente, creando nuevo");
            AgregarNuevoItem(item, cantidadAgregada);
        }

        ActualizarUI();
        GuardarInventarioDeJugador();
    }

    void AgregarNuevoItem(ItemDataSO item, int cantidad)
    {
        Debug.Log($"InventoryManager: AgregarNuevoItem - {item.Name} x{cantidad}");

        for (int i = 0; i < inventarioItems.Count; i++)
        {
            if (inventarioItems[i] == null)
            {
                Debug.Log($"InventoryManager: Slot vacío encontrado en índice {i}");
                inventarioItems[i] = new InventoryItem(item, cantidad);
                return;
            }
        }

        Debug.LogWarning("InventoryManager: No hay slots vacíos disponibles!");
    }

    public void IntercambiarItems(int indexA, int indexB)
    {
        if (indexA < 0 || indexA >= inventarioItems.Count ||
            indexB < 0 || indexB >= inventarioItems.Count)
        {
            Debug.LogWarning("InventoryManager: Índices fuera de rango");
            return;
        }

        if (indexA == indexB)
        {
            Debug.Log("InventoryManager: Mismo slot, no se realiza ninguna acción");
            return;
        }

        InventoryItem itemA = inventarioItems[indexA];
        InventoryItem itemB = inventarioItems[indexB];

        if (itemA == null)
        {
            Debug.LogWarning("InventoryManager: Item origen es null");
            return;
        }

        if (itemA.itemData == null)
        {
            Debug.LogError($"InventoryManager: itemA.itemData es null en slot {indexA}, eliminando item corrupto");
            inventarioItems[indexA] = null;
            ActualizarUI();
            return;
        }

        if (itemB == null)
        {
            Debug.Log($"InventoryManager: Moviendo {itemA.itemData.Name} de slot {indexA} a slot vacío {indexB}");
            inventarioItems[indexB] = itemA;
            inventarioItems[indexA] = null;
            ActualizarUI();
            return;
        }

        if (itemB.itemData == null)
        {
            Debug.LogError($"InventoryManager: itemB.itemData es null en slot {indexB}, reemplazando con item válido");
            inventarioItems[indexB] = itemA;
            inventarioItems[indexA] = null;
            ActualizarUI();
            return;
        }

        if (itemA.itemData == itemB.itemData && itemA.itemData.IsStackable)
        {
            Debug.Log($"InventoryManager: Intentando apilar {itemA.itemData.Name} - {itemA.cantidad} + {itemB.cantidad}");

            int maxStack = itemA.itemData.MaxStackSize;
            int total = itemA.cantidad + itemB.cantidad;

            if (total <= maxStack)
            {
                itemB.cantidad = total;
                inventarioItems[indexA] = null;
                Debug.Log($"InventoryManager: Stack completo - Total: {total}");
            }
            else
            {
                itemB.cantidad = maxStack;
                itemA.cantidad = total - maxStack;
                Debug.Log($"InventoryManager: Stack parcial - Destino: {maxStack}, Origen restante: {itemA.cantidad}");
            }
        }
        else
        {
            Debug.Log($"InventoryManager: Intercambiando {itemA.itemData.Name} (slot {indexA}) con {itemB.itemData.Name} (slot {indexB})");
            InventoryItem temp = inventarioItems[indexA];
            inventarioItems[indexA] = inventarioItems[indexB];
            inventarioItems[indexB] = temp;
        }

        ActualizarUI();
        GuardarInventarioDeJugador();
    }

    public void DividirItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= inventarioItems.Count)
        {
            Debug.LogWarning($"InventoryManager: Índice de slot inválido: {slotIndex}");
            return;
        }

        InventoryItem itemOriginal = inventarioItems[slotIndex];

        if (itemOriginal == null || itemOriginal.cantidad <= 1)
        {
            Debug.LogWarning("InventoryManager: No se puede dividir este item");
            return;
        }

        if (!itemOriginal.itemData.IsStackable)
        {
            Debug.LogWarning("InventoryManager: Este item no es apilable");
            return;
        }

        int slotVacioIndex = -1;
        for (int i = 0; i < inventarioItems.Count; i++)
        {
            if (inventarioItems[i] == null)
            {
                slotVacioIndex = i;
                break;
            }
        }

        if (slotVacioIndex == -1)
        {
            Debug.LogWarning("InventoryManager: No hay slots vacíos para dividir");
            return;
        }

        int cantidadOriginal = itemOriginal.cantidad;
        int mitad = cantidadOriginal / 2;
        int restante = cantidadOriginal - mitad;

        itemOriginal.cantidad = restante;
        inventarioItems[slotVacioIndex] = new InventoryItem(itemOriginal.itemData, mitad);

        Debug.Log($"InventoryManager: Item dividido - Slot {slotIndex}: {restante}, Slot {slotVacioIndex}: {mitad}");

        ActualizarUI();
    }

    public void DescartarItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= inventarioItems.Count)
        {
            Debug.LogWarning($"InventoryManager: Índice de slot inválido: {slotIndex}");
            return;
        }

        InventoryItem item = inventarioItems[slotIndex];

        if (item == null)
        {
            Debug.LogWarning("InventoryManager: No hay item para descartar en este slot");
            return;
        }

        Debug.Log($"InventoryManager: Descartando {item.itemData.Name} x{item.cantidad} del slot {slotIndex}");

        inventarioItems[slotIndex] = null;
        ActualizarUI();
        GuardarInventarioDeJugador();
    }

    public void GuardarInventarioDeJugador()
    {
        if (GameDataManager.Instance != null)
        {
            GameSaveData saveData = GameDataManager.Instance.GetSaveData();
            saveData.playerInventory = GameDataManager.Instance.ConvertirASerializable(inventarioItems);
            GameDataManager.Instance.GuardarDatos();
            Debug.Log("InventoryManager: Inventario del jugador guardado");
        }
    }

    public void CargarInventarioDeJugador()
    {
        if (GameDataManager.Instance != null)
        {
            GameSaveData saveData = GameDataManager.Instance.GetSaveData();
            inventarioItems = GameDataManager.Instance.ConvertirAInventoryItems(saveData.playerInventory);

            while (inventarioItems.Count < slots.Length)
            {
                inventarioItems.Add(null);
            }

            LimpiarItemsCorruptos();
            ActualizarUI();
            Debug.Log($"InventoryManager: Inventario del jugador cargado - {inventarioItems.Count} slots");
        }
    }

    private void LimpiarItemsCorruptos()
    {
        int itemsLimpiados = 0;
        for (int i = 0; i < inventarioItems.Count; i++)
        {
            if (inventarioItems[i] != null && inventarioItems[i].itemData == null)
            {
                Debug.LogWarning($"InventoryManager: Item corrupto encontrado en slot {i}, limpiando...");
                inventarioItems[i] = null;
                itemsLimpiados++;
            }
        }

        if (itemsLimpiados > 0)
        {
            Debug.Log($"InventoryManager: {itemsLimpiados} items corruptos limpiados");
            GuardarInventarioDeJugador();
        }
    }
    public void TransferirTodoAlStash()
    {
        if (BaseStashManager.Instance == null)
        {
            Debug.LogWarning("InventoryManager: BaseStashManager no disponible");
            return;
        }

        int itemsTransferidos = 0;

        for (int i = 0; i < inventarioItems.Count; i++)
        {
            if (inventarioItems[i] != null && inventarioItems[i].itemData != null)
            {
                bool exito = BaseStashManager.Instance.AgregarItemAlStash(
                    inventarioItems[i].itemData,
                    inventarioItems[i].cantidad
                );

                if (exito)
                {
                    inventarioItems[i] = null;
                    itemsTransferidos++;
                }
            }
        }

        ActualizarUI();
        GuardarInventarioDeJugador();

        if (BaseStashUIManager.Instance != null)
        {
            BaseStashUIManager.Instance.UpdateAllCategories();
        }

        Debug.Log($"InventoryManager: {itemsTransferidos} items transferidos al almacenamiento");
    }


    public void VaciarInventario()
    {
        for (int i = 0; i < inventarioItems.Count; i++)
        {
            inventarioItems[i] = null;
        }
        ActualizarUI();
        GuardarInventarioDeJugador();
    }
}
