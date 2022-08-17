using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySentryBehavior : EnemyBehavior
{
    public LayerMask obstacleLayer;
    public LayerMask nodeLayer;
    public bool canSeeThroughWalls;
    public bool isNodeFilterOff;

    public void FixedUpdate()
    {
        if (isBehaviorEnabled == false) return;

        enemy.StopMovement();
        enemy.playerDetected = IsPlayerDetected(enemy.lookDirection) || IsPlayerDetected(-enemy.lookDirection);

        if(enemy.playerDetected)
        {
            enemy.chase.isBehaviorEnabled = true;
            this.isBehaviorEnabled = false;
        }
    }

    private bool IsPlayerDetected(Vector2 direction)
    {
        RaycastHit2D playerHit = Physics2D.CircleCast(transform.position, 7.0f * enemy.detectionSizeModifier, Vector2.zero, 0.0f, enemy.playerLayer);

        if (playerHit.collider == null) return false;

        bool isPlayerPresent = playerHit.collider.TryGetComponent<Player>(out Player player);

        if (!isPlayerPresent) return false;

        Vector3 heading = (player.transform.position - enemy.transform.position);
        RaycastHit2D wallDetectionHit = Physics2D.Raycast(enemy.transform.position, heading / heading.magnitude, heading.magnitude, obstacleLayer);

        if (wallDetectionHit.collider != null && !canSeeThroughWalls) return false;

        RaycastHit2D nodeDetectionHit = Physics2D.Raycast(enemy.transform.position, heading / heading.magnitude, heading.magnitude, nodeLayer);

        return nodeDetectionHit.collider == null || isNodeFilterOff;
    }

    
}
