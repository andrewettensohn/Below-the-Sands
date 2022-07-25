using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombatBehavior : EnemyBehavior
{

    [Range(0.0f, 5.0f)]
    public float dealDamageDelay;

    [Range(0.1f, 10f)]
    public float attackRange;

    [Range(0.0f, 5.0f)]
    public float blockTime;

    [Range(0.1f, 5.0f)]
    public float leaveOpeningTime;

    public bool willBlock;

    public bool isBlocking { get; protected set; }
    public bool isAttacking { get; protected set; }
    public bool isInCombat { get; protected set; }

    public bool canAttack { get; protected set; }
    public bool canBlock { get; protected set; }
    public bool canLeaveOpening { get; protected set; } = true;

    public bool hasKnockBackAttack;
    public float knockBackForce;

    public float detectionSizeModifier = 0.75f;

    public virtual void HandleCombat()
    {
        Collider2D[] players = GetPlayerHits(attackRange);

        if (players.Length == 0)
        {
            isInCombat = false;
            return;
        }

        isInCombat = true;

        if (canLeaveOpening)
        {
            OnLeaveOpening();
        }

        if (canAttack)
        {
            OnAttack();
        }
        else if (canBlock)
        {
            OnBlock();
        }
    }


    protected virtual Collider2D[] GetPlayerHits(float distance)
    {
        return Physics2D.OverlapCircleAll(enemy.attackPoint.position, attackRange, enemy.playerLayer);
    }

    protected virtual void OnAttack()
    {
        canAttack = false;
        isAttacking = true;
        enemy.audioSource.PlayOneShot(enemy.AttackingAudioClip);

        StartCoroutine(HandlePostAttackDelayTimer());
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

    protected virtual IEnumerator HandlePostAttackDelayTimer()
    {
        yield return new WaitForSeconds(dealDamageDelay);

        if (enemy.health > 0)
        {
            HandleAttack();

            enemy.isStaggered = false;
            isAttacking = false;
            canBlock = true;
        }
    }

    protected virtual void HandleAttack()
    {
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(enemy.attackPoint.position, attackRange, enemy.playerLayer);

        if (enemy.health > 0 && hitPlayers.Length > 0 && !enemy.isStaggered)
        {
            foreach (Collider2D hit in hitPlayers)
            {
                bool isPlayerComponentPresent = hit.TryGetComponent<Player>(out Player player);

                if (isPlayerComponentPresent)
                {
                    player.OnDeltDamage(1);
                    enemy.OnSuccessfulAttack();
                    KnockBackEffect(player);
                }
            }
        }
    }

    protected void KnockBackEffect(Player player)
    {
        if(!hasKnockBackAttack) return;

        float force = enemy.lookDirection.x > 0 ? knockBackForce : -knockBackForce;

        if(enemy.lookDirection.x > 0)
        {
            player.movement.rigidbody.AddForce(player.transform.right * knockBackForce);
        }
        else
        {
            player.movement.rigidbody.AddForce(player.transform.right * -knockBackForce);
        }
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
