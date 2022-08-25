using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonArcherCombatBehavior : EnemyCombatBehavior
{
    public GameObject arrowPrefab;
    public float arrowLaunchForce;
    public bool isFreeFire;
    public float freeFireShotDelay;

    private void Start()
    {
        canAttack = isFreeFire;
    }

    protected override void Update()
    {
        if(isBehaviorEnabled == false && !isFreeFire) return;

        if(isFreeFire)
        {
            enemy.DisableAllBehaviors();

            if(canAttack)
            {
                OnAttack();
            }

            return;
        }

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

    protected override void OnAttack()
    {
        canAttack = false;
        isAttacking = true;

        if(isFreeFire)
        {
            StartCoroutine(HandlePostFreefireShotDelayTimer());
        }
        else
        {
            StartCoroutine(HandlePostAttackDelayTimer());
        }
    }

    protected override IEnumerator HandlePostAttackDelayTimer()
    {
        yield return new WaitForSeconds(primaryAttackDealDamageDelay);

        if (enemy.health > 0)
        {
            FireArrow();

            isAttacking = false;
            canLeavePostAttackOpening = true;
        }
    }

    private IEnumerator HandlePostFreefireShotDelayTimer()
    {
        yield return new WaitForSeconds(freeFireShotDelay);

        if (enemy.health > 0)
        {
            FireArrow();

            canAttack = true;
        }
    }

    private void FireArrow()
    {
        enemy.animator.SetTrigger("Attack");

        GameObject arrowGameObject = Instantiate(arrowPrefab, transform.position, Quaternion.identity);

        Arrow arrow = arrowGameObject.GetComponent<Arrow>();
        Vector2 launchDirection = enemy.attackPoint.transform.position.x <= enemy.transform.position.x ? Vector2.left : Vector2.right;

        arrow.Launch(launchDirection, arrowLaunchForce);
    }
}
