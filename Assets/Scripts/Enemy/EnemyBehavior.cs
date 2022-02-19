using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    public Enemy enemy { get; private set; }
    public bool isBehaviorEnabled;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }
}
