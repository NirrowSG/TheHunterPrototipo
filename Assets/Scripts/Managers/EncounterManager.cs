using UnityEngine;
using System;

public class EncounterManager : MonoBehaviour
{
    [Header("Encounter Configuration")]
    public EncounterConfigSO encounterConfig;

    [Header("Spawn Settings")]
    public Transform enemySpawnPoint;
    public GameObject enemyPrefab;

    private int remainingEncounters;
    private Enemy currentEnemy;

    public event Action<Enemy> OnEnemySpawned;
    public event Action OnAllEncountersComplete;

    public Enemy CurrentEnemy => currentEnemy;
    public int RemainingEncounters => remainingEncounters;

    private void Start()
    {
        Debug.Log($"EncounterManager.Start() llamado");

        if (encounterConfig == null)
        {
            Debug.LogError("EncounterManager: NO HAY EncounterConfig asignado!");
            return;
        }

        Debug.Log($"EncounterManager: EncounterConfig encontrado: {encounterConfig.encounterName}");
        Debug.Log($"EncounterManager: Enemigos posibles: {encounterConfig.possibleEnemies.Count}");

        remainingEncounters = UnityEngine.Random.Range(
            encounterConfig.minEncounters,
            encounterConfig.maxEncounters + 1
        );

        Debug.Log($"EncounterManager: Se generarán {remainingEncounters} encuentros");
    }


    public void SpawnNextEnemy()
    {
        if (remainingEncounters <= 0)
        {
            OnAllEncountersComplete?.Invoke();
            return;
        }

        EnemyDataSO enemyData = encounterConfig.GetRandomEnemy();
        if (enemyData == null)
        {
            Debug.LogError("No se pudo obtener un enemigo del encounter config.");
            return;
        }

        Vector3 spawnPosition = enemySpawnPoint != null ?
            enemySpawnPoint.position : Vector3.zero;

        GameObject enemyObject = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        currentEnemy = enemyObject.GetComponent<Enemy>();

        if (currentEnemy != null)
        {
            currentEnemy.Initialize(enemyData);
            currentEnemy.OnEnemyDeath += HandleEnemyDeath;
            remainingEncounters--;

            OnEnemySpawned?.Invoke(currentEnemy);
        }
        else
        {
            Debug.LogError("El prefab de enemigo no tiene el componente Enemy.");
        }
    }

    private void HandleEnemyDeath(Enemy enemy)
    {
        enemy.OnEnemyDeath -= HandleEnemyDeath;
        currentEnemy = null;
    }
}
