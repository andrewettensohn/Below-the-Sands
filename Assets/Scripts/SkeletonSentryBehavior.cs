using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonSentryBehavior : SkeletonBehavior
{
    public void FixedUpdate()
    {
        TryDetectPlayer(skeleton.movement.lookDirection);
        TryDetectPlayer(-skeleton.movement.lookDirection);
    }

    private void TryDetectPlayer(Vector2 direction)
    {
        if (!isBehaviorEnabled) return;

        RaycastHit2D hit = Physics2D.BoxCast(transform.position, Vector2.one * 0.75f, 0.0f, direction, 10.0f, skeleton.playerLayer);

        if (hit.collider != null)
        {
            skeleton.playerDetected = true;
        }
    }
}
