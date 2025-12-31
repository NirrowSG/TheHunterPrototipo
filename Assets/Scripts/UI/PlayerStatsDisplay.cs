using UnityEngine;
using TMPro;

public class PlayerStatsDisplay : MonoBehaviour
{
    [Header("Referencias a Textos - Stats Básicas")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI sanityText;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI defenseText;

    [Header("Referencias a Textos - Armas")]
    public TextMeshProUGUI meleeWeaponText;
    public TextMeshProUGUI fireWeaponText;

    private void OnEnable()
    {
        SuscribirEventos();
        ActualizarDisplay();
    }

    private void OnDisable()
    {
        DesuscribirEventos();
    }

    private void Start()
    {
        SuscribirEventos();
        ActualizarDisplay();
    }

    private void SuscribirEventos()
    {
        if (PlayerStatsManager.Instance != null)
        {
            PlayerStatsManager.Instance.OnStatsChanged -= ActualizarDisplay;
            PlayerStatsManager.Instance.OnStatsChanged += ActualizarDisplay;
        }
    }

    private void DesuscribirEventos()
    {
        if (PlayerStatsManager.Instance != null)
        {
            PlayerStatsManager.Instance.OnStatsChanged -= ActualizarDisplay;
        }
    }

    public void ActualizarDisplay()
    {
        if (PlayerStatsManager.Instance == null)
        {
            return;
        }

        PlayerStats stats = PlayerStatsManager.Instance.stats;

        if (healthText != null)
        {
            healthText.text = $"HP: {stats.currentHealth}/{stats.MaxHealth}";
        }

        if (sanityText != null)
        {
            sanityText.text = $"Sanity: {stats.currentSanity}/{stats.MaxSanity}";
        }

        if (attackText != null)
        {
            string attackStr = $"Attack: {stats.TotalAttack}";
            if (stats.bonusAttack != 0)
            {
                attackStr += $" ({stats.baseAttack}+{stats.bonusAttack})";
            }
            attackText.text = attackStr;
        }

        if (defenseText != null)
        {
            string defenseStr = $"Defense: {stats.TotalDefense}";
            if (stats.bonusDefense != 0)
            {
                defenseStr += $" ({stats.baseDefense}+{stats.bonusDefense})";
            }
            defenseText.text = defenseStr;
        }

        ActualizarArmasEquipadas();
    }

    private void ActualizarArmasEquipadas()
    {
        if (PlayerStatsManager.Instance == null || EquipmentManager.Instance == null)
        {
            return;
        }

        int baseAttack = PlayerStatsManager.Instance.stats.TotalAttack;

        EquipmentItemSO meleeWeapon = PlayerStatsManager.Instance.GetEquippedWeapon(EquipmentSlotType.MeleeWeapon);
        if (meleeWeaponText != null)
        {
            if (meleeWeapon != null && meleeWeapon.isWeapon)
            {
                int totalMeleeDamage = baseAttack + meleeWeapon.weaponDamage;
                meleeWeaponText.text = $"Melee: {meleeWeapon.Name} ({totalMeleeDamage} dmg)";
            }
            else
            {
                meleeWeaponText.text = $"Melee: Sin arma ({baseAttack} dmg)";
            }
        }

        EquipmentItemSO fireWeapon = PlayerStatsManager.Instance.GetEquippedWeapon(EquipmentSlotType.FireWeapon);
        if (fireWeaponText != null)
        {
            if (fireWeapon != null && fireWeapon.isWeapon)
            {
                int totalFireDamage = baseAttack + fireWeapon.weaponDamage;
                fireWeaponText.text = $"Fire: {fireWeapon.Name} ({totalFireDamage} dmg)";
            }
            else
            {
                fireWeaponText.text = $"Fire: Sin arma ({baseAttack} dmg)";
            }
        }
    }
}
