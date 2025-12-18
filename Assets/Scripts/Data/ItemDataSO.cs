using UnityEngine;

// Enum para filtrar rápidamente en el inventario
public enum ItemType { Materials, FireWeapon, MeleeWeapon, Consumable, Equipment, KeyItem }

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Generic Item")]
public class ItemDataSO : ScriptableObject
{
    [Header("General Info")]
    public string ID; // Único (o usa el nombre del archivo)
    public string Name;
    [TextArea] public string Description;
    public Sprite Icon;
    public ItemType Type;

    [Header("Stacking")]
    public bool IsStackable;
    public int MaxStackSize = 99;
}