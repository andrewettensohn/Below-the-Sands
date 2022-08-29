using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MainMenuEnemy : MonoBehaviour
{
    public float speed;
    public Transform target;
    public NavMeshAgent navMeshAgent { get; private set; }
    public Animator animator;
    public Vector2 lookDirection;

    protected virtual void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    protected virtual void Start()
    {
        navMeshAgent.stoppingDistance = 0;
        navMeshAgent.speed = speed;
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;

        navMeshAgent.SetDestination(GameObject.Find("KillBox").gameObject.transform.position);
    }

    protected virtual void Update()
    {
        DetermineLookDirection();
        Animate();
    }

     protected virtual void DetermineLookDirection()
    {
        lookDirection = navMeshAgent.desiredVelocity;

        if (lookDirection.x > 0.1)
        {
            transform.rotation = Quaternion.identity;
        }
        else if (lookDirection.x < -0.1)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    protected void Animate()
    {
        animator.SetFloat("Speed", navMeshAgent.velocity.sqrMagnitude);
        animator.SetFloat("Move Y", navMeshAgent.velocity.y);
        animator.SetFloat("Look X", 1);
    }
}
