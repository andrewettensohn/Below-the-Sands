using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonBehavior : MonoBehaviour
{
    public Skeleton skeleton { get; private set; }
    public bool isBehaviorEnabled;

    private void Awake()
    {
        skeleton = GetComponent<Skeleton>();
    }
}
