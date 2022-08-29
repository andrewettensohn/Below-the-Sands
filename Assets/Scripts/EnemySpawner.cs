using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemySpawner : MonoBehaviour
{
    public bool isActive = false;
    public bool areSpawnedEnemiesDefeated;
    public int spawnLimit;
    public int numberSpawned;
    public float timeBetweenSpawns;
    public bool shouldTrackedSpawnedEnemies = true;
    public GameObject enemyPrefab;
    public List<GameObject> spawnedEnemies { get; private set; } = new List<GameObject>();
    private bool canSpawn = true;

    private void Update()
    {
        if(!isActive) return;

        SpawnEnemy();

        if(shouldTrackedSpawnedEnemies)
        {
            areSpawnedEnemiesDefeated = (spawnedEnemies?.All(x => !x.activeSelf)).GetValueOrDefault() && numberSpawned >= spawnLimit;
        }
    }

    private void SpawnEnemy()
    {
        if(!canSpawn || numberSpawned >= spawnLimit) return;

        canSpawn = false;
        numberSpawned += 1;

        GameObject enemyGameObject = Instantiate(enemyPrefab, transform.position, Quaternion.identity);

        spawnedEnemies.Add(enemyGameObject);

        StartCoroutine(HandleSpawnCooldownTimer());
    }

    protected virtual IEnumerator HandleSpawnCooldownTimer()
    {
        yield return new WaitForSeconds(timeBetweenSpawns);

        canSpawn = true;
    }
}
