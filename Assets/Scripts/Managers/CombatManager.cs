using UnityEngine;
using System;
using System.Collections;

public class CombatManager : MonoBehaviour
{
    [Header("Managers")]
    public EncounterManager encounterManager;

    [Header("Combat Settings")]
    public float meleeAttackCooldown = 1f;
    public float fireAttackCooldown = 0.5f;

    private PlayerStatsManager playerStatsManager;
    private EquipmentManager equipmentManager;
    private bool isCombatActive = false;
    private float enemyAttackTimer = 0f;
    private bool canMeleeAttack = true;
    private bool canFireAttack = true;

    public event Action OnCombatStart;
    public event Action<int, int, bool> OnPlayerAttack;
    public event Action<int, int> OnEnemyAttack;
    public event Action OnPlayerVictory;
    public event Action OnPlayerDefeat;

    public bool IsCombatActive => isCombatActive;
    public Enemy CurrentEnemy => encounterManager?.CurrentEnemy;
    public bool CanMeleeAttack => canMeleeAttack && isCombatActive;
    public bool CanFireAttack => canFireAttack && isCombatActive;

    private void Awake()
    {
        playerStatsManager = PlayerStatsManager.Instance;
        equipmentManager = EquipmentManager.Instance;
    }

    private void Start()
    {
        if (playerStatsManager == null)
        {
            Debug.LogError("CombatManager: No se encontró PlayerStatsManager.Instance");
        }

        if (encounterManager != null)
        {
            encounterManager.OnEnemySpawned += StartCombat;
            encounterManager.OnAllEncountersComplete += HandleAllEncountersComplete;
        }
        else
        {
            Debug.LogError("CombatManager: EncounterManager no asignado!");
            return;
        }

        StartCoroutine(StartCombatAfterFrame());
    }

    private IEnumerator StartCombatAfterFrame()
    {
        yield return null;

        Debug.Log("CombatManager: Iniciando primer encuentro...");
        StartNewEncounter();
    }

    private void OnDestroy()
    {
        if (encounterManager != null)
        {
            encounterManager.OnEnemySpawned -= StartCombat;
            encounterManager.OnAllEncountersComplete -= HandleAllEncountersComplete;
        }
    }

    private void Update()
    {
        if (!isCombatActive || CurrentEnemy == null || CurrentEnemy.IsDead)
            return;

        enemyAttackTimer += Time.deltaTime;

        if (enemyAttackTimer >= CurrentEnemy.enemyData.attackSpeed)
        {
            PerformEnemyAttack();
            enemyAttackTimer = 0f;
        }
    }

    public void StartNewEncounter()
    {
        if (encounterManager != null)
        {
            Debug.Log($"CombatManager: Spawneando enemigo... Encuentros restantes: {encounterManager.RemainingEncounters}");
            encounterManager.SpawnNextEnemy();
        }
    }

    private void StartCombat(Enemy enemy)
    {
        isCombatActive = true;
        enemyAttackTimer = 0f;
        enemy.OnEnemyDeath += HandleEnemyDeath;
        OnCombatStart?.Invoke();

        Debug.Log($"CombatManager: Combate iniciado contra {enemy.enemyData.enemyName}");
    }

    public void PlayerMeleeAttack()
    {
        if (!isCombatActive || CurrentEnemy == null || CurrentEnemy.IsDead || !canMeleeAttack)
        {
            Debug.Log("CombatManager: No se puede atacar con melee");
            return;
        }

        int damage = CalculateMeleeDamage();
        if (damage <= 0)
        {
            Debug.LogWarning("CombatManager: No tienes un arma cuerpo a cuerpo equipada");
            return;
        }

        CurrentEnemy.TakeDamage(damage);
        OnPlayerAttack?.Invoke(damage, CurrentEnemy.CurrentHealth, true);

        StartCoroutine(MeleeAttackCooldown());

        Debug.Log($"CombatManager: Ataque Melee - Daño: {damage}");
    }

    public void PlayerFireAttack()
    {
        if (!isCombatActive || CurrentEnemy == null || CurrentEnemy.IsDead || !canFireAttack)
        {
            Debug.Log("CombatManager: No se puede atacar con fuego");
            return;
        }

        int damage = CalculateFireDamage();
        if (damage <= 0)
        {
            Debug.LogWarning("CombatManager: No tienes un arma de fuego equipada");
            return;
        }

        CurrentEnemy.TakeDamage(damage);
        OnPlayerAttack?.Invoke(damage, CurrentEnemy.CurrentHealth, false);

        StartCoroutine(FireAttackCooldown());

        Debug.Log($"CombatManager: Ataque Fuego - Daño: {damage}");
    }

    private IEnumerator MeleeAttackCooldown()
    {
        canMeleeAttack = false;
        yield return new WaitForSeconds(meleeAttackCooldown);
        canMeleeAttack = true;
    }

    private IEnumerator FireAttackCooldown()
    {
        canFireAttack = false;
        yield return new WaitForSeconds(fireAttackCooldown);
        canFireAttack = true;
    }

    private void PerformEnemyAttack()
    {
        if (CurrentEnemy == null || CurrentEnemy.IsDead || playerStatsManager == null)
            return;

        CurrentEnemy.PlayAttackAnimation();

        int enemyDamage = CurrentEnemy.CalculateAttackDamage();
        int playerDefense = playerStatsManager.stats.TotalDefense;
        int finalDamage = Mathf.Max(1, enemyDamage - playerDefense);

        playerStatsManager.stats.currentHealth -= finalDamage;
        playerStatsManager.stats.ClampCurrentStats();

        OnEnemyAttack?.Invoke(finalDamage, playerStatsManager.stats.currentHealth);

        Debug.Log($"CombatManager: Enemigo atacó - Daño: {finalDamage}, HP Jugador: {playerStatsManager.stats.currentHealth}");

        if (playerStatsManager.stats.currentHealth <= 0)
        {
            HandlePlayerDefeat();
        }
    }

    private int CalculateMeleeDamage()
    {
        if (playerStatsManager == null || equipmentManager == null)
            return 0;

        int baseDamage = playerStatsManager.stats.TotalAttack;

        var meleeWeapon = equipmentManager.GetEquippedItem(EquipmentSlotType.MeleeWeapon);
        if (meleeWeapon != null && meleeWeapon is EquipmentItemSO equipWeapon)
        {
            if (equipWeapon.isWeapon)
            {
                baseDamage += equipWeapon.weaponDamage;
            }
        }
        else
        {
            return 0;
        }

        return baseDamage;
    }

    private int CalculateFireDamage()
    {
        if (playerStatsManager == null || equipmentManager == null)
            return 0;

        int baseDamage = playerStatsManager.stats.TotalAttack;

        var fireWeapon = equipmentManager.GetEquippedItem(EquipmentSlotType.FireWeapon);
        if (fireWeapon != null && fireWeapon is EquipmentItemSO equipWeapon)
        {
            if (equipWeapon.isWeapon)
            {
                baseDamage += equipWeapon.weaponDamage;
            }
        }
        else
        {
            return 0;
        }

        return baseDamage;
    }

    private void HandleEnemyDeath(Enemy enemy)
    {
        enemy.OnEnemyDeath -= HandleEnemyDeath;
        isCombatActive = false;

        Debug.Log($"CombatManager: Enemigo {enemy.enemyData.enemyName} derrotado");

        GiveRewards(enemy);

        StartCoroutine(SpawnNextEnemyAfterDelay(2f));
    }

    private IEnumerator SpawnNextEnemyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (encounterManager.RemainingEncounters > 0)
        {
            Debug.Log($"CombatManager: Spawneando siguiente enemigo... Quedan {encounterManager.RemainingEncounters}");
            encounterManager.SpawnNextEnemy();
        }
        else
        {
            Debug.Log("CombatManager: No quedan más encuentros");
        }
    }

    private void GiveRewards(Enemy enemy)
    {
        int coins = enemy.enemyData.coinReward;
        Debug.Log($"Ganaste {coins} monedas");

        ItemDrop[] droppedItems = enemy.GetDroppedItems();
        foreach (var drop in droppedItems)
        {
            int quantity = UnityEngine.Random.Range(drop.minQuantity, drop.maxQuantity + 1);
            Debug.Log($"Conseguiste {quantity}x {drop.item.Name}");
        }
    }

    private void HandleAllEncountersComplete()
    {
        OnPlayerVictory?.Invoke();
        Debug.Log("¡Has completado todos los encuentros!");
    }

    private void HandlePlayerDefeat()
    {
        isCombatActive = false;
        OnPlayerDefeat?.Invoke();
        Debug.Log("Has sido derrotado...");
    }
}
