using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySentryBehavior : EnemyBehavior
{
    public void FixedUpdate()
    {
        if (isBehaviorEnabled == false) return;

        enemy.playerDetected = IsPlayerDetected(enemy.movement.lookDirection) || IsPlayerDetected(-enemy.movement.lookDirection);
    }

    private bool IsPlayerDetected(Vector2 direction)
    {

        RaycastHit2D playerHit = Physics2D.BoxCast(transform.position, Vector2.one * 0.75f, 0.0f, direction, 10.0f, enemy.playerLayer);

        return playerHit.collider != null;
    }
}
