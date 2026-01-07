using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Combat/Enemy Data")]
public class EnemyDataSO : ScriptableObject
{
    [Header("General Info")]
    public string enemyName;
    public Sprite sprite;

    [Header("Base Stats")]
    public int baseHealth = 50;
    public int baseAttack = 5;
    public int baseDefense = 3;
    public float attackSpeed = 2f;

    [Header("Rewards")]
    public int coinReward = 10;
    public List<ItemDrop> itemDrops = new List<ItemDrop>();

    [Header("Animation Settings")]
    public AnimationClip idleAnimation;
    public AnimationClip attackAnimation;
    public AnimationClip hurtAnimation;
    public AnimationClip deathAnimation;
}

[System.Serializable]
public class ItemDrop
{
    public ItemDataSO item;
    [Range(0f, 100f)]
    public float dropChance = 50f;
    public int minQuantity = 1;
    public int maxQuantity = 1;
}
