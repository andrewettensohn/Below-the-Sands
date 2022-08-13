using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class VerticalTrap : MonoBehaviour
{

    public AnimationClip TrapAnimationClip;
    public List<Sprite> DamageSprites;
    public float animationStartDelay;
    public SpriteRenderer spriteRenderer { get; private set; }
    protected Animator animator;
    protected Player playerGameObject;
    protected bool hasDeltDamage;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        StartCoroutine(HandleAnimationStartDelay());
    }

    private void Update()
    {
        HandleTrapDamage();
    }

    protected virtual void HandleTrapDamage()
    {
        if (playerGameObject == null) return;

        bool shouldDealDamage = DamageSprites.Any(x => x.name == spriteRenderer.sprite.name);

        if (shouldDealDamage && !hasDeltDamage)
        {
            hasDeltDamage = true;
            playerGameObject.OnDeltDamage(-1, true);
        }

        if (!shouldDealDamage)
        {
            hasDeltDamage = false;
        }
    }

    protected virtual IEnumerator HandleAnimationStartDelay()
    {
        yield return new WaitForSeconds(animationStartDelay);

        animator.Play(TrapAnimationClip.name);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == GameManager.instance.playerCharacterName)
        {
            Player player = collision.GetComponent<Player>();
            playerGameObject = player;
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collider)
    {
        playerGameObject = null;
    }
}
