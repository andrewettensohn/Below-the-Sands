using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Player : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    public float attackDelay;

    [Range(0.1f, 1.5f)]
    public float attackRange;

    [Range(1, 10)]
    public int maxSwordBlockedDamage;

    public LayerMask enemyLayer;
    public GameObject PlayerUICanvas;
    public Movement movement { get; private set; }

    public AudioClip runningAudioClip;
    public AudioClip swingWithSwordAudioClip;
    public AudioClip hitWithSwordAudioClip;
    public AudioClip blockedAudioClip;

    private bool isAttacking;
    private bool canAttack = true;
    private Animator animator;
    private List<HealthHeart> hearts;
    private List<BlockIcon> blockIcons;
    private AudioSource audioSource;
    private bool isBlocking;
    private int blockedDamage;
    private bool isStaggered;

    private void Awake()
    {
        movement = GetComponent<Movement>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        hearts = PlayerUICanvas.GetComponentsInChildren<HealthHeart>().ToList();
        blockIcons = PlayerUICanvas.GetComponentsInChildren<BlockIcon>().ToList();
    }

    private void Start()
    {
        if (PlayerInfo.instance.nextPlayerPositionOnLoad != Vector2.zero)
        {
            movement.rigidbody.MovePosition(PlayerInfo.instance.nextPlayerPositionOnLoad);
        }

        SyncHearts();
    }

    private void Update()
    {
        GetUserInput();
        AnimateMovement();
        PlaySound();
        HandleShieldEquipped();
        HandleHealthPotionUsed();
        HandleCombat();
    }

    private void GetUserInput()
    {
        isBlocking = Input.GetKey(KeyCode.Mouse1);

        if (isBlocking)
        {
            movement.rigidbody.velocity = new Vector2(0f, movement.rigidbody.velocity.y);
        }

        movement.SetDirection(new Vector2(Input.GetAxis("Horizontal"), 0f));
        movement.isJumping = (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space)) && !isBlocking;
    }

    private void HandleShieldEquipped()
    {
        if (!Input.GetKeyDown(KeyCode.Z) || !PlayerInfo.instance.hasShield) return;

        if (PlayerInfo.instance.isShieldEquipped)
        {
            PlayerInfo.instance.isShieldEquipped = false;
            animator.SetBool("Using Shield", false);
        }
        else
        {
            PlayerInfo.instance.isShieldEquipped = true;
            animator.SetBool("Using Shield", true);
        }
    }

    private void HandleCombat()
    {
        if (isBlocking) return;

        blockedDamage = 0;
        ResetBlockIcons();

        if (!Input.GetKeyDown(KeyCode.Mouse0) || !canAttack) return;

        isAttacking = true;
        canAttack = false;
        audioSource.PlayOneShot(swingWithSwordAudioClip);
        StartCoroutine(HandleAttackDelayTimer());
    }

    private IEnumerator HandleAttackDelayTimer()
    {
        if (!canAttack)
        {
            yield return new WaitForSeconds(attackDelay);

            RaycastHit2D hit = GetEnemyHit(attackRange);
            if (hit.collider != null && !isStaggered)
            {
                audioSource.PlayOneShot(hitWithSwordAudioClip);
                Skeleton skeleton = hit.collider.GetComponent<Skeleton>();
                skeleton.OnDeltDamage(1);
            }

            isAttacking = false;
            isStaggered = false;
            canAttack = true;
        }
    }

    private RaycastHit2D GetEnemyHit(float distance)
    {
        return Physics2D.BoxCast(transform.position, Vector2.one * 0.75f, 0.0f, movement.lookDirection, distance, enemyLayer);
    }

    private void SyncHearts()
    {
        int heartsNeededToChange = PlayerInfo.instance.fullHealth - PlayerInfo.instance.health;

        SetHeartsUnhealthy(heartsNeededToChange);
    }

    private void OnPlayerDamaged(int damage)
    {
        PlayerInfo.instance.health -= damage;

        if (damage <= 0) return;

        SetHeartsUnhealthy(damage);
    }

    private void SetHeartsUnhealthy(int heartsNeededToChange)
    {
        int heartsChanged = 0;
        for (int i = hearts.Count - 1; i > -1; i--)
        {
            if (hearts[i].isHealthy && heartsChanged < heartsNeededToChange)
            {
                hearts[i].SetUnhealthy();
                heartsChanged++;
            }
        }
    }

    private void HandleHealthPotionUsed()
    {
        if (PlayerInfo.instance.healthPotions <= 0 || !Input.GetKeyDown(KeyCode.Alpha1)) return;

        PlayerInfo.instance.healthPotions -= 1;
        PlayerInfo.instance.health = PlayerInfo.instance.fullHealth;

        foreach (HealthHeart heart in hearts)
        {
            heart.SetHealthy();
        }
    }

    private void ResetBlockIcons()
    {
        foreach (BlockIcon blockIcon in blockIcons)
        {
            blockIcon.SetBlocking();
        }
    }

    private void SetBlockIconsToBroken(int blockIconsToChange)
    {
        int blockIconsChanged = 0;
        for (int i = blockIcons.Count - 1; i > -1; i--)
        {
            if (blockIcons[i].isBlocking && blockIconsChanged < blockIconsToChange)
            {
                blockIcons[i].SetBroken();
                blockIconsChanged++;
            }
        }
    }

    public void OnDeltDamage(int damage)
    {
        damage = Mathf.Abs(damage);

        if (isBlocking && blockedDamage < blockIcons.Count)
        {
            audioSource.PlayOneShot(blockedAudioClip);
            blockedDamage += damage;
            SetBlockIconsToBroken(damage);
            return;
        }

        OnPlayerDamaged(damage);

        if (PlayerInfo.instance.health <= 0)
        {
            animator.SetTrigger("Die");
            Invoke(nameof(OnDisable), 1.3f);
            GameManager.instance.LoadMainMenu();
        }
        else if (!isAttacking)
        {
            animator.SetTrigger("Hit");
            isStaggered = true;
        }
    }

    public void PlaySound()
    {
        if (movement.rigidbody.velocity.x != 0 && movement.isGrounded && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(runningAudioClip);
        }
    }

    public void AnimateMovement()
    {
        animator.SetBool("Attacking", isAttacking && !isStaggered);
        animator.SetFloat("Speed", movement.rigidbody.velocity.sqrMagnitude);
        animator.SetFloat("Move Y", movement.rigidbody.velocity.y);
        animator.SetFloat("Look X", movement.lookDirection.x);
        animator.SetBool("Is Grounded", movement.isGrounded);
        animator.SetBool("Blocking", isBlocking);
    }


    public void OnDisable()
    {
        gameObject.SetActive(false);
    }
}
