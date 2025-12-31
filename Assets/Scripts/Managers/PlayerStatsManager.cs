using UnityEngine;
using System;

public class PlayerStatsManager : MonoBehaviour
{
    public static PlayerStatsManager Instance;

    public PlayerStats stats = new PlayerStats();

    public event Action OnStatsChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("PlayerStatsManager: Instancia creada");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        CargarStats();
    }

    public void RecalcularStats()
    {
        stats.ResetBonuses();

        if (EquipmentManager.Instance != null)
        {
            foreach (var kvp in EquipmentManager.Instance.equippedItems)
            {
                if (kvp.Value != null && kvp.Value is EquipmentItemSO equipItem)
                {
                    foreach (var modifier in equipItem.statModifiers)
                    {
                        stats.ApplyStatModifier(modifier.statType, modifier.value);
                    }
                }
            }
        }

        stats.ClampCurrentStats();
        OnStatsChanged?.Invoke();
        Debug.Log($"PlayerStatsManager: Stats recalculadas - HP:{stats.currentHealth}/{stats.MaxHealth} ATK:{stats.TotalAttack} DEF:{stats.TotalDefense}");
    }

    public void TakeDamage(int damage)
    {
        int actualDamage = Mathf.Max(1, damage - stats.TotalDefense);
        stats.currentHealth -= actualDamage;
        stats.currentHealth = Mathf.Max(0, stats.currentHealth);
        OnStatsChanged?.Invoke();
        GuardarStats();
        Debug.Log($"PlayerStatsManager: Daño recibido {actualDamage} - HP: {stats.currentHealth}/{stats.MaxHealth}");
    }

    public void TakeSanityDamage(int damage)
    {
        stats.currentSanity -= damage;
        stats.currentSanity = Mathf.Max(0, stats.currentSanity);
        OnStatsChanged?.Invoke();
        GuardarStats();
        Debug.Log($"PlayerStatsManager: Cordura perdida {damage} - Sanity: {stats.currentSanity}/{stats.MaxSanity}");
    }

    public void Heal(int amount)
    {
        stats.currentHealth += amount;
        stats.currentHealth = Mathf.Min(stats.currentHealth, stats.MaxHealth);
        OnStatsChanged?.Invoke();
        GuardarStats();
        Debug.Log($"PlayerStatsManager: Curación {amount} - HP: {stats.currentHealth}/{stats.MaxHealth}");
    }

    public void RestoreSanity(int amount)
    {
        stats.currentSanity += amount;
        stats.currentSanity = Mathf.Min(stats.currentSanity, stats.MaxSanity);
        OnStatsChanged?.Invoke();
        GuardarStats();
        Debug.Log($"PlayerStatsManager: Cordura restaurada {amount} - Sanity: {stats.currentSanity}/{stats.MaxSanity}");
    }

    public void GuardarStats()
    {
        if (GameDataManager.Instance != null)
        {
            GameSaveData saveData = GameDataManager.Instance.GetSaveData();
            saveData.playerStats = new SerializablePlayerStats(stats);
            GameDataManager.Instance.GuardarDatos();
        }
    }

    public void CargarStats()
    {
        if (GameDataManager.Instance != null)
        {
            GameSaveData saveData = GameDataManager.Instance.GetSaveData();
            if (saveData.playerStats != null)
            {
                stats.currentHealth = saveData.playerStats.currentHealth;
                stats.currentSanity = saveData.playerStats.currentSanity;
            }
            else
            {
                stats.currentHealth = stats.baseHealth;
                stats.currentSanity = stats.baseSanity;
            }

            stats.ClampCurrentStats();
            OnStatsChanged?.Invoke();
            Debug.Log($"PlayerStatsManager: Stats cargadas - HP:{stats.currentHealth}/{stats.MaxHealth} Sanity:{stats.currentSanity}/{stats.MaxSanity}");
        }
    }

    public EquipmentItemSO GetEquippedWeapon(EquipmentSlotType weaponType)
    {
        if (EquipmentManager.Instance != null)
        {
            if (EquipmentManager.Instance.equippedItems.TryGetValue(weaponType, out ItemDataSO item))
            {
                return item as EquipmentItemSO;
            }
        }
        return null;
    }
}
