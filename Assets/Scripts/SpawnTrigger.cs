using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTrigger : MonoBehaviour
{
    public List<EnemySpawner> enemySpawners;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == GameManager.instance.playerCharacterName)
        {
            enemySpawners.ForEach(x => x.isActive = true);
        }
    }
}
