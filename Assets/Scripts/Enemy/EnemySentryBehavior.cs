using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySentryBehavior : EnemyBehavior
{
    public float detectionSizeModifier = 0.75f;

    public void FixedUpdate()
    {
        if (isBehaviorEnabled == false) return;

        enemy.playerDetected = IsPlayerDetected(enemy.lookDirection) || IsPlayerDetected(-enemy.lookDirection);
    }

    private bool IsPlayerDetected(Vector2 direction)
    {
        RaycastHit2D playerHit = Physics2D.CircleCast(transform.position, 7.0f, enemy.lookDirection, 1.0f, enemy.playerLayer);

        return playerHit.collider != null;
    }
}
