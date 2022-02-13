using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEyeCombatBehavior : EnemyCombatBehavior
{

    private EnemyEye enemyEye;

    private void Awake()
    {
        enemyEye = GetComponent<EnemyEye>();
    }

    public override void HandleCombat()
    {
        RaycastHit2D hit = enemyEye.GetPlayerHit(attackRange);
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

    protected override void OnAttack(Player player)
    {
        canAttack = false;
        isAttacking = true;
        StartCoroutine(HandleDealDamageDelayTimer(player));
    }

    protected override void OnBlock()
    {
        canBlock = false;
        isBlocking = willBlock;
        StartCoroutine(HandleBlockTimer());
    }

    protected override void OnLeaveOpening()
    {
        canLeaveOpening = false;
        StartCoroutine(HandleLeaveOpeningTimer());
    }

    protected override IEnumerator HandleDealDamageDelayTimer(Player player)
    {
        yield return new WaitForSeconds(dealDamageDelay);

        RaycastHit2D hit = enemyEye.GetPlayerHit(attackRange);
        if (enemyEye.health > 0 && hit.collider != null && !enemyEye.isStaggered)
        {
            player.OnDeltDamage(1);
        }

        isAttacking = false;
        canBlock = true;
    }

    protected override IEnumerator HandleBlockTimer()
    {
        yield return new WaitForSeconds(blockTime);

        isBlocking = false;
        canLeaveOpening = true;
    }

    protected override IEnumerator HandleLeaveOpeningTimer()
    {
        yield return new WaitForSeconds(leaveOpeningTime);

        canAttack = true;
    }
}
