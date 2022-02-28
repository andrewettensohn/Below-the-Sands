using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostKnightCombatBehavior : EnemyCombatBehavior
{
    private GhostKnight ghostKnight;

    private void Awake()
    {
        ghostKnight = GetComponent<GhostKnight>();
    }

    public override void HandleCombat()
    {
        RaycastHit2D hit = GetPlayerHit(attackRange);
        Player player = hit.collider?.GetComponent<Player>();

        if (hit.collider == null)
        {
            isInCombat = false;
            return;
        }

        isInCombat = true;

        if (canAttack)
        {
            OnAttack(player);
        }

        else if (canBlock)
        {
            OnBlock();
        }

        if (canLeaveOpening)
        {
            OnLeaveOpening();
        }
    }

    protected override RaycastHit2D GetPlayerHit(float distance)
    {
        return Physics2D.CircleCast(transform.position, attackRange, ghostKnight.aiPath.desiredVelocity, distance, ghostKnight.playerLayer);
    }

    protected override IEnumerator HandleDealDamageDelayTimer(Player player)
    {
        yield return new WaitForSeconds(dealDamageDelay);

        RaycastHit2D hit = GetPlayerHit(attackRange);
        if (ghostKnight.health > 0 && hit.collider != null && !ghostKnight.isStaggered)
        {
            player.OnDeltDamage(1);
        }

        isAttacking = false;
        canBlock = true;
    }
}
