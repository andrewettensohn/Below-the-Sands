using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DamageableEnemy : MonoBehaviour
{
    public abstract void OnDeltDamage(float damage, Player player = null);
}
