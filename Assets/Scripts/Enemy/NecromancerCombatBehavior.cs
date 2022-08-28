using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NecromancerCombatBehavior : EnemyCombatBehavior
{
    public GameObject skeletonPrefab;

    protected override IEnumerator HandlePostSecondaryAttackDelayTimer()
    {
        yield return new WaitForSeconds(secondaryAttackDealDamageDelay);

        if (enemy.health > 0)
        {
            RaiseTheDead();

            enemy.isStaggered = false;
            canLeavePostSecondaryAttackOpening = true;
        }
    }

    private void RaiseTheDead()
    {
        GameObject skeletonGameObject = Instantiate(skeletonPrefab, enemy.attackPoint.position, Quaternion.identity);
    }
}
