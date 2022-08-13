using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowTrap : MonoBehaviour
{
    public Sprite arrowFiredSprite;
    public GameObject arrowPrefab;
    public Transform arrowLaunchPointLeft;
    public Transform arrowLaunchPointRight;
    public float direction;
    public float fireDelay;
    public int launchForce;
    private SpriteRenderer spriteRenderer;
    private Transform launchDirection;
    private bool canFire;
    private bool hasFired;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        launchDirection = direction > 0 ? arrowLaunchPointRight : arrowLaunchPointLeft;
    }

    private void Update()
    {
        canFire = spriteRenderer.sprite.name == arrowFiredSprite.name;

        OnAttack();
    }

    protected virtual void OnAttack()
    {
        if(!canFire || hasFired) return;

        hasFired = true;

        GameObject arrowGameObject = Instantiate(arrowPrefab, launchDirection.position, Quaternion.Euler(0, 180, 0));
        Arrow arrow = arrowGameObject.GetComponent<Arrow>();
        arrow.Launch(new Vector2(direction, 0), launchForce);

        StartCoroutine(HandlePostAttackDelayTimer());
    }

    protected virtual IEnumerator HandlePostAttackDelayTimer()
    {
        yield return new WaitForSeconds(fireDelay);

        hasFired = false;
    }
}
