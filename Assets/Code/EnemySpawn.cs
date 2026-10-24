using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject enemyPrefab;
    public float spawnInterval = 2f;

    [Header("Spawn Points")]
    public Transform[] spawnPoints; // Drag your 4 spawn transforms here in the Inspector

    void Start()
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning("Enemy Prefab is not assigned on the EnemySpawner!");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("Spawn Points are not assigned on the EnemySpawner!");
            return;
        }

        // Start spawning loop
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        if (spawnPoints.Length == 0) return;

        // Pick a random spawn point from the array
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform targetSpawn = spawnPoints[randomIndex];

        // Instantiate the enemy prefab at the chosen spawn point
        Instantiate(enemyPrefab, targetSpawn.position, targetSpawn.rotation);
    }
}