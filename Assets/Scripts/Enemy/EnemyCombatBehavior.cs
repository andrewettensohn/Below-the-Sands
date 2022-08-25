using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombatBehavior : EnemyBehavior
{

    [Range(0.0f, 5.0f)]
    public float primaryAttackDealDamageDelay;

    [Range(0.0f, 5.0f)]
    public float secondaryAttackDealDamageDelay;

    [Range(0.1f, 10f)]
    public float attackRange;

    [Range(0.0f, 5.0f)]
    public float blockTime;

    public bool isBlocking { get; protected set; }
    public bool isAttacking { get; protected set; }
    public bool isInCombat { get; protected set; }

    public bool primaryAttackHasKnockBack;
    public bool secondaryAttackHasKnockBack;
    public float knockBackForce;
    
    public bool hasSecondaryAttack;
    public bool hasBlock;
    public bool hasContinuousAttack;

    public float initalLeaveOpeningTime;
    public float postAttackLeaveOpeningTime;
    public float postBlockLeaveOpeningTime;
    public float postSecondaryAttackLeaveOpeningTime;

    protected bool canLeaveInitalOpening = true;
    protected bool canLeavePostAttackOpening;
    protected bool canLeavePostBlockOpening;
    protected bool canLeavePostSecondaryAttackOpening;

    protected bool canAttack;
    protected bool canBlock;
    protected bool canDoSecondaryAttack;

    protected enum AttackStage
    {
        InitalOpening,
        Attack,
        Block,
        SecondaryAttack,
    }

    protected virtual void Update()
    {
        if(isBehaviorEnabled == false) return;

        Collider2D[] players = GetPlayerHits(attackRange);

        if (players.Length == 0)
        {
            isInCombat = false;
            isAttacking = false;
            this.isBehaviorEnabled = false;
            enemy.sentry.isBehaviorEnabled = true;

            return;
        }

        HandleAttackCycle();
    }

    protected virtual void HandleAttackCycle()
    {
        isInCombat = true;
        
        if(canLeaveInitalOpening)
        {
            LeaveOpening(AttackStage.InitalOpening);
        }
        
        if(canAttack && hasContinuousAttack)
        {
            OnContinuousAttack();
        }
        else if(canAttack)
        {
            OnAttack();
        }
        else if(canLeavePostAttackOpening)
        {
            LeaveOpening(AttackStage.Attack);
        }
        else if(canBlock && hasBlock)
        {
            OnBlock();
        }
        else if(canLeavePostBlockOpening && hasBlock)
        {
            LeaveOpening(AttackStage.Block);
        }
        else if(canDoSecondaryAttack && hasSecondaryAttack)
        {
            OnSecondaryAttack();
        }
        else if(canLeavePostSecondaryAttackOpening && hasSecondaryAttack)
        {
            LeaveOpening(AttackStage.SecondaryAttack);
        }
    }

    protected virtual Collider2D[] GetPlayerHits(float distance)
    {
        return Physics2D.OverlapCircleAll(enemy.attackPoint.position, attackRange, enemy.playerLayer);
    }

    protected virtual void LeaveOpening(AttackStage stage)
    {
        float leaveOpeningTime = 0.0f;

        switch (stage)
        {
            case AttackStage.InitalOpening:
                leaveOpeningTime = initalLeaveOpeningTime;
                canLeaveInitalOpening = false;
                break; 
            case AttackStage.Attack:
                leaveOpeningTime = postAttackLeaveOpeningTime;
                canLeavePostAttackOpening = false;
                break; 
            case AttackStage.Block:
                leaveOpeningTime = postBlockLeaveOpeningTime;
                canLeavePostBlockOpening = false;
                break; 
            case AttackStage.SecondaryAttack:
                leaveOpeningTime = postSecondaryAttackLeaveOpeningTime;
                canLeavePostSecondaryAttackOpening = false;
                break;
        }

        StartCoroutine(HandleLeaveOpeningTimer(leaveOpeningTime, stage));
    }

    protected virtual IEnumerator HandleLeaveOpeningTimer(float time, AttackStage stage)
    {
        yield return new WaitForSeconds(time);

        if(stage == AttackStage.InitalOpening || (stage == AttackStage.Attack && !hasBlock && !hasSecondaryAttack) || stage == AttackStage.SecondaryAttack)
        {
            canAttack = true;
        }
        else if(stage == AttackStage.Attack && hasBlock)
        {
            canBlock = true;
        }
        else if(stage == AttackStage.Block && !hasSecondaryAttack)
        {
            canAttack = true;
        }
        else if((stage == AttackStage.Block || stage == AttackStage.Attack) && hasSecondaryAttack)
        {
            canDoSecondaryAttack = true;
        }
    }

    protected virtual void OnContinuousAttack()
    {
        canAttack = false;
        isAttacking = true;
        enemy.animator.SetTrigger("Attack");
        
        StartCoroutine(HandleContinuousAttackDelayTimer());
    }

    protected virtual IEnumerator HandleContinuousAttackDelayTimer()
    {
        yield return new WaitForSeconds(primaryAttackDealDamageDelay);

        if (enemy.health > 0)
        {
            HandleAttack(primaryAttackHasKnockBack);

            canAttack = true;
        }
    }

    protected virtual void OnAttack()
    {
        canAttack = false;
        isAttacking = true;
        enemy.audioSource.PlayOneShot(enemy.AttackingAudioClip);
        enemy.animator.SetTrigger("Attack");

        StartCoroutine(HandlePostAttackDelayTimer());
    }

    protected virtual IEnumerator HandlePostAttackDelayTimer()
    {
        yield return new WaitForSeconds(primaryAttackDealDamageDelay);

        if (enemy.health > 0)
        {
            HandleAttack(primaryAttackHasKnockBack);

            isAttacking = false;
            canLeavePostAttackOpening = true;   
        }
    }

    protected virtual void HandleAttack(bool isKnockbackAttack)
    {
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(enemy.attackPoint.position, attackRange, enemy.playerLayer);

        if (enemy.health > 0 && hitPlayers.Length > 0)
        {
            foreach (Collider2D hit in hitPlayers)
            {
                bool isPlayerComponentPresent = hit.TryGetComponent<Player>(out Player player);

                if (isPlayerComponentPresent)
                {
                    player.OnDeltDamage(1);
                    enemy.OnSuccessfulAttack();

                    if(isKnockbackAttack) KnockBackEffect(player);
                }
            }
        }
    }

    protected void KnockBackEffect(Player player)
    {
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

    protected virtual void OnBlock()
    {
        canBlock = false;
        isBlocking = true;

        StartCoroutine(HandleBlockTimer());
    }

    protected virtual IEnumerator HandleBlockTimer()
    {
        yield return new WaitForSeconds(blockTime);

        isBlocking = false;
        canLeavePostBlockOpening = true;
    }

    protected virtual void OnSecondaryAttack()
    {
        canDoSecondaryAttack = false;
        enemy.animator.SetTrigger("Secondary Attack");

        StartCoroutine(HandlePostSecondaryAttackDelayTimer());
    }

    protected virtual IEnumerator HandlePostSecondaryAttackDelayTimer()
    {
        yield return new WaitForSeconds(secondaryAttackDealDamageDelay);

        if (enemy.health > 0)
        {
            HandleAttack(secondaryAttackHasKnockBack);

            canLeavePostSecondaryAttackOpening = true;
        }
    }
}