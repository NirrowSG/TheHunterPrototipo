using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Inventory/Weapon")]
public class WeaponDataSO : ItemDataSO
{
    [Header("Combat Stats")]
    public float Damage;
    public float AttackRate;
    public float Range;
    public GameObject ProjectilePrefab; // Para armas de fuego
    public bool IsMelee;
}