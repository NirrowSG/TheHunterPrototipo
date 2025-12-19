using UnityEngine;
using System.Collections.Generic;

public class QuickInventoryManager : MonoBehaviour
{
    public static QuickInventoryManager Instance;

    public List<InventoryItem> quickItems = new List<InventoryItem>();
    public QuickInventorySlot[] quickSlots;
    public Transform quickSlotsPadre;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        Debug.Log("QuickInventoryManager: Awake llamado");
    }

    void Start()
    {
        quickSlots = quickSlotsPadre.GetComponentsInChildren<QuickInventorySlot>();
        Debug.Log($"QuickInventoryManager: Se encontraron {quickSlots.Length} quick slots");

        while (quickItems.Count < quickSlots.Length)
        {
            quickItems.Add(null);
        }

        Debug.Log($"QuickInventoryManager: Lista inicializada con {quickItems.Count} espacios");
        ActualizarUI();
    }

    public void ActualizarUI()
    {
        Debug.Log($"QuickInventoryManager: ActualizarUI llamado - {quickSlots.Length} slots, {quickItems.Count} items");
        for (int i = 0; i < quickSlots.Length; i++)
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

        if (itemB == null)
        {
            Debug.Log($"QuickInventoryManager: Moviendo item de slot {indexA} a slot vacío {indexB}");
            quickItems[indexB] = itemA;
            quickItems[indexA] = null;
            ActualizarUI();
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
        return item;
    }
}
