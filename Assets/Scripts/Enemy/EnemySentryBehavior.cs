using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySentryBehavior : EnemyBehavior
{
    public float detectionSizeModifier = 0.75f;
    public LayerMask obstacleLayer;
    public LayerMask nodeLayer;

    public void FixedUpdate()
    {
        if (isBehaviorEnabled == false) return;

        enemy.playerDetected = IsPlayerDetected(enemy.lookDirection) || IsPlayerDetected(-enemy.lookDirection);
    }

    private bool IsPlayerDetected(Vector2 direction)
    {
        RaycastHit2D playerHit = Physics2D.CircleCast(transform.position, 7.0f, enemy.lookDirection, 1.0f, enemy.playerLayer);
        // RaycastHit2D playerHit = Physics2D.BoxCast(transform.position, 7.0f, enemy.lookDirection, 1.0f, enemy.playerLayer);

        if (playerHit.collider == null) return false;

        bool isPlayerPresent = playerHit.collider.TryGetComponent<Player>(out Player player);

        if (!isPlayerPresent) return false;

        Vector3 heading = (player.transform.position - enemy.transform.position);
        RaycastHit2D wallDetectionHit = Physics2D.Raycast(enemy.transform.position, heading / heading.magnitude, heading.magnitude, obstacleLayer);

        if (wallDetectionHit.collider != null) return false;

        RaycastHit2D nodeDetectionHit = Physics2D.Raycast(enemy.transform.position, heading / heading.magnitude, heading.magnitude, nodeLayer);

        return nodeDetectionHit.collider == null;
    }
}
