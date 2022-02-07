using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonCombatBehavior : SkeletonBehavior
{

    [Range(0.0f, 5.0f)]
    public float dealDamageDelay;

    [Range(0.1f, 1.5f)]
    public float attackRange;

    [Range(0.0f, 5.0f)]
    public float blockTime;

    [Range(0.1f, 5.0f)]
    public float leaveOpeningTime;

    public bool willBlock;

    public bool isBlocking { get; protected set; }
    public bool isAttacking { get; protected set; }
    public bool isInCombat { get; protected set; }

    public bool canAttack { get; protected set; } = true;
    public bool canBlock { get; protected set; }
    public bool canLeaveOpening { get; protected set; }

    public virtual void HandleCombat()
    {
        RaycastHit2D hit = skeleton.GetPlayerHit(attackRange);
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

    protected virtual void OnAttack(Player player)
    {
        canAttack = false;
        isAttacking = true;
        StartCoroutine(HandleDealDamageDelayTimer(player));
    }

    protected virtual void OnBlock()
    {
        canBlock = false;
        isBlocking = willBlock;
        StartCoroutine(HandleBlockTimer());
    }

    protected virtual void OnLeaveOpening()
    {
        canLeaveOpening = false;
        StartCoroutine(HandleLeaveOpeningTimer());
    }

    protected virtual IEnumerator HandleDealDamageDelayTimer(Player player)
    {
        yield return new WaitForSeconds(dealDamageDelay);

        RaycastHit2D hit = skeleton.GetPlayerHit(attackRange);
        if (skeleton.health > 0 && hit.collider != null)
        {
            player.OnDeltDamage(1);
        }

        isAttacking = false;
        canBlock = true;
    }

    protected virtual IEnumerator HandleBlockTimer()
    {
        yield return new WaitForSeconds(blockTime);

        isBlocking = false;
        canLeaveOpening = true;
    }

    protected virtual IEnumerator HandleLeaveOpeningTimer()
    {
        yield return new WaitForSeconds(leaveOpeningTime);

        canAttack = true;
    }
}
