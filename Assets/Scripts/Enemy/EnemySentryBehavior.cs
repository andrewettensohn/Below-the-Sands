using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySentryBehavior : EnemyBehavior
{
    public void FixedUpdate()
    {
        TryDetectPlayer(enemy.movement.lookDirection);
        TryDetectPlayer(-enemy.movement.lookDirection);
    }

    private void TryDetectPlayer(Vector2 direction)
    {
        if (!isBehaviorEnabled) return;

        RaycastHit2D hit = Physics2D.CircleCast(transform.position, 10.0f, direction, 0.0f, enemy.playerLayer);

        if (hit.collider != null)
        {
            enemy.playerDetected = true;
        }
    }
}