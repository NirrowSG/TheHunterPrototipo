using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Equipment Item")]
public class EquipmentItemSO : ItemDataSO
{
    [Header("Equipment Settings")]
    public EquipmentSlotType equipmentSlot;

    [Header("Stat Modifiers")]
    public List<StatModifier> statModifiers = new List<StatModifier>();

    [Header("Weapon Settings (if applicable)")]
    public bool isWeapon;
    public int weaponDamage;
    public float attackSpeed = 1f;
    public float weaponRange = 1f;

    public int GetStatModifier(StatType statType)
    {
        foreach (var modifier in statModifiers)
        {
            if (modifier.statType == statType)
            {
                return modifier.value;
            }
        }
        return 0;
    }
}
