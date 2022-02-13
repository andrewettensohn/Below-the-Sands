using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

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
    private List<HealthPotSlot> healthPotSlots;
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
        healthPotSlots = PlayerUICanvas.GetComponentsInChildren<HealthPotSlot>().ToList();
    }

    private void Start()
    {
        if (PlayerInfo.instance.nextPlayerPositionOnLoad != Vector2.zero)
        {
            movement.rigidbody.MovePosition(PlayerInfo.instance.nextPlayerPositionOnLoad);
        }

        SyncHearts();
        SyncHealthPotions();
    }

    private void Update()
    {
        if (GameManager.instance.isGamePaused) return;

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
                DamageableEnemy enemy = hit.collider.GetComponent<DamageableEnemy>();
                enemy.OnDeltDamage(1);
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

    private void SyncHealthPotions()
    {
        int healthPotionsNeededToDisable = 3 - PlayerInfo.instance.healthPotions;
        DisableHealthPotionSlots(healthPotionsNeededToDisable);
    }

    private void DisableHealthPotionSlots(int healthPotionsNeededToDisable)
    {
        int healthPotionsChanged = 0;

        for (int i = healthPotSlots.Count - 1; i > -1; i--)
        {
            if (healthPotSlots[i].isActiveAndEnabled && healthPotionsChanged < healthPotionsNeededToDisable)
            {
                healthPotSlots[i].gameObject.SetActive(false);
                healthPotionsChanged++;
            }
        }
    }

    private void EnableHealthPotionSlots(int healthPotionsNeededToEnable)
    {
        int healthPotionsChanged = 0;

        for (int i = 0; i < healthPotSlots.Count; i++)
        {
            if (!healthPotSlots[i].isActiveAndEnabled && healthPotionsChanged < healthPotionsNeededToEnable)
            {
                healthPotSlots[i].gameObject.SetActive(true);
                healthPotionsChanged++;
            }
        }
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

        DisableHealthPotionSlots(1);
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

    private void PlaySound()
    {
        if (movement.rigidbody.velocity.x != 0 && movement.isGrounded && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(runningAudioClip);
        }
    }

    private void AnimateMovement()
    {
        animator.SetBool("Attacking", isAttacking && !isStaggered);
        animator.SetFloat("Speed", movement.rigidbody.velocity.sqrMagnitude);
        animator.SetFloat("Move Y", movement.rigidbody.velocity.y);
        animator.SetFloat("Look X", movement.lookDirection.x);
        animator.SetBool("Is Grounded", movement.isGrounded);
        animator.SetBool("Blocking", isBlocking);
    }

    public void OnHealthPotionPickedUp()
    {
        PlayerInfo.instance.healthPotions += 1;
        EnableHealthPotionSlots(1);
    }


    public void OnDisable()
    {
        gameObject.SetActive(false);
    }
}
