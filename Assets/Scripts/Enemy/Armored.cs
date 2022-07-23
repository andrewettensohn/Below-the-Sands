using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Armored : Enemy
{
    public override void OnSuccessfulAttack()
    {
        enemyWaypointBehavior.isBehaviorEnabled = true;
    }
}
