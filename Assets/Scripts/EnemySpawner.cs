using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public bool isActive = false;
    public int spawnLimit;
    public int numberSpawned;
    public float timeBetweenSpawns;
    public GameObject enemyPrefab;
    public List<GameObject> spawnedEnemies { get; private set; } = new List<GameObject>();
    private bool canSpawn = true;

    private void Update()
    {
        if(!isActive) return;
        
        SpawnEnemy();
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
