using UnityEngine;
using System;

[System.Serializable]
public class PlayerStats
{
    public int baseHealth = 100;
    public int baseSanity = 100;
    public int baseAttack = 7;
    public int baseDefense = 7;

    public int currentHealth;
    public int currentSanity;

    public int bonusHealth;
    public int bonusSanity;
    public int bonusAttack;
    public int bonusDefense;

    public int MaxHealth => baseHealth + bonusHealth;
    public int MaxSanity => baseSanity + bonusSanity;
    public int TotalAttack => baseAttack + bonusAttack;
    public int TotalDefense => baseDefense + bonusDefense;

    public PlayerStats()
    {
        currentHealth = baseHealth;
        currentSanity = baseSanity;
    }

    public void ResetBonuses()
    {
        bonusHealth = 0;
        bonusSanity = 0;
        bonusAttack = 0;
        bonusDefense = 0;
    }

    public void ApplyStatModifier(StatType statType, int value)
    {
        switch (statType)
        {
            case StatType.Health:
                bonusHealth += value;
                break;
            case StatType.Sanity:
                bonusSanity += value;
                break;
            case StatType.Attack:
                bonusAttack += value;
                break;
            case StatType.Defense:
                bonusDefense += value;
                break;
        }
    }

    public void ClampCurrentStats()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0, MaxHealth);
        currentSanity = Mathf.Clamp(currentSanity, 0, MaxSanity);
    }
}

[System.Serializable]
public class SerializablePlayerStats
{
    public int currentHealth;
    public int currentSanity;

    public SerializablePlayerStats(PlayerStats stats)
    {
        currentHealth = stats.currentHealth;
        currentSanity = stats.currentSanity;
    }
}

[System.Serializable]
public class SerializableEquipmentSlot
{
    public string itemID;
    public EquipmentSlotType slotType;

    public SerializableEquipmentSlot(string id, EquipmentSlotType type)
    {
        itemID = id;
        slotType = type;
    }
}
