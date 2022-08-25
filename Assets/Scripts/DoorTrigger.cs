using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DoorTrigger : MonoBehaviour
{
    public string levelName;
    public float horizontalPositionAfterLoad;
    public float verticalPositionAfterLoad;
    public bool isLocked;
    public GameObject bossLockObject;
    public List<EnemySpawner> bossLockSpawners;
    public Sprite UnlockedSprite;
    private SpriteRenderer spriteRenderer;
    private bool isLockedBySpawners;
    private bool isLockedByBoss;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        isLockedBySpawners = bossLockSpawners.Any();
        isLockedByBoss = bossLockObject != null;
    }

    private void FixedUpdate()
    {
        if(isLockedBySpawners && bossLockSpawners.All(x => x.areSpawnedEnemiesDefeated))
        {
            UnlockDoor();
        }
        else if(isLockedByBoss && !bossLockObject.activeSelf)
        {
            UnlockDoor();
        }
    }

    private void UnlockDoor()
    {
        isLocked = false;
        spriteRenderer.sprite = UnlockedSprite;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        TransportPlayer();
    }

    public void TransportPlayer()
    {
        if (isLocked) return;

        GameManager.instance.LoadScene(levelName.ToString(), new Vector2(horizontalPositionAfterLoad, verticalPositionAfterLoad));
    }
}
