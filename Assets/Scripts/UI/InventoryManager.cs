using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public List<InventoryItem> inventarioItems = new List<InventoryItem>();
    public InventorySlot[] slots;
    public Transform slotsPadre;

    void Awake()
    {
        Instance = this;
        Debug.Log("InventoryManager: Awake llamado");
    }

    void Start()
    {
        slots = slotsPadre.GetComponentsInChildren<InventorySlot>();
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
        Debug.Log("InventoryManager: ActualizarUI llamado");
        for (int i = 0; i < slots.Length; i++)
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

        if (itemB == null)
        {
            Debug.Log($"InventoryManager: Moviendo item de slot {indexA} a slot vacío {indexB}");
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
            Debug.Log($"InventoryManager: Intercambiando items de slot {indexA} y slot {indexB}");
            InventoryItem temp = inventarioItems[indexA];
            inventarioItems[indexA] = inventarioItems[indexB];
            inventarioItems[indexB] = temp;
        }

        ActualizarUI();
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
    }


}
