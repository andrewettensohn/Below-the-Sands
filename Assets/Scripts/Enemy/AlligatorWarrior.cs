using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlligatorWarrior : Enemy
{

    public override void OnSuccessfulAttack()
    {
        enemyWaypointBehavior.isBehaviorEnabled = true;
    }
}
