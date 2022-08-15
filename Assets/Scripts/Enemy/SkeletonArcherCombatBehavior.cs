using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonArcherCombatBehavior : EnemyCombatBehavior
{
    public GameObject arrowPrefab;
    public float arrowLaunchForce;

    protected override void OnAttack()
    {
        canAttack = false;
        isAttacking = true;

        StartCoroutine(HandlePostAttackDelayTimer());
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

    private void FireArrow()
    {
        enemy.animator.SetTrigger("Attack");

        GameObject arrowGameObject = Instantiate(arrowPrefab, transform.position, Quaternion.identity);

        Arrow arrow = arrowGameObject.GetComponent<Arrow>();
        arrow.Launch(enemy.lookDirection, arrowLaunchForce);
    }
}
