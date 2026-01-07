using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Encounter Config", menuName = "Combat/Encounter Config")]
public class EncounterConfigSO : ScriptableObject
{
    [Header("Encounter Settings")]
    public string encounterName;
    public List<EnemySpawnData> possibleEnemies = new List<EnemySpawnData>();

    [Header("Encounter Limits")]
    public int minEncounters = 3;
    public int maxEncounters = 7;

    public EnemyDataSO GetRandomEnemy()
    {
        if (possibleEnemies.Count == 0)
        {
            Debug.LogError("No hay enemigos configurados en el encounter.");
            return null;
        }

        float totalWeight = 0f;
        foreach (var enemyData in possibleEnemies)
        {
            totalWeight += enemyData.spawnWeight;
        }

        float randomValue = Random.Range(0f, totalWeight);
        float currentWeight = 0f;

        foreach (var enemyData in possibleEnemies)
        {
            currentWeight += enemyData.spawnWeight;
            if (randomValue <= currentWeight)
            {
                return enemyData.enemyData;
            }
        }

        return possibleEnemies[0].enemyData;
    }
}

[System.Serializable]
public class EnemySpawnData
{
    public EnemyDataSO enemyData;
    [Range(0f, 100f)]
    public float spawnWeight = 50f;
}
