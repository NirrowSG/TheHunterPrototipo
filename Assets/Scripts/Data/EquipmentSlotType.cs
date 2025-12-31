using UnityEngine;

public enum EquipmentSlotType
{
    Head,
    Torso,
    Hands,
    Legs,
    Feet,
    MeleeWeapon,
    FireWeapon
}

public enum StatType
{
    Health,
    Sanity,
    Attack,
    Defense
}

[System.Serializable]
public class StatModifier
{
    public StatType statType;
    public int value;

    public StatModifier(StatType type, int val)
    {
        statType = type;
        value = val;
    }
}
