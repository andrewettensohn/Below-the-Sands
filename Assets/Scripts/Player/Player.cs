using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    public float attackDelay;

    [Range(0.1f, 1.5f)]
    public float attackRange;

    [Range(1, 10)]
    public int maxSwordBlockedDamage;

    [Range(1, 10)]
    public float blessedTime;

    public LayerMask enemyLayer;
    public GameObject PlayerUICanvas;
    public Movement movement { get; private set; }

    public ParticleSystem blessedEffect;

    public AudioClip runningAudioClip;
    public AudioClip swingWithSwordAudioClip;
    public AudioClip hitWithSwordAudioClip;
    public AudioClip blockedAudioClip;

    private bool isAttacking;
    private bool canAttack = true;
    private Animator animator;
    private PlayerUI playerUI;

    private AudioSource audioSource;

    private bool isBlessed;
    private bool isBlocking;
    private int blockedDamage;
    private bool isStaggered;
    private int numberOfSwordBlockIcons = 2;

    private void Awake()
    {
        movement = GetComponent<Movement>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        playerUI = PlayerUICanvas.GetComponent<PlayerUI>();
    }

    private void Start()
    {
        PlayerInfo.instance.hasShield = true;

        if (PlayerInfo.instance.nextPlayerPositionOnLoad != Vector2.zero)
        {
            movement.rigidbody.MovePosition(PlayerInfo.instance.nextPlayerPositionOnLoad);
        }

        playerUI.SyncHearts();
        playerUI.SyncHealthPotions();
        playerUI.SyncRelicSlots();
        playerUI.SyncPrayerSlots();

        if (!PlayerInfo.instance.isShieldEquipped)
        {
            playerUI.ToggleBlockIconsActive(numberOfSwordBlockIcons, false);
        }
        else
        {
            animator.SetBool("Using Shield", true);
            playerUI.ToggleBlockIconsActive(numberOfSwordBlockIcons, true);
        }
    }

    private void Update()
    {
        if (GameManager.instance.isGamePaused) return;

        GetUserInput();
        AnimateMovement();
        PlaySound();
        HandleCombat();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isBlessed)
        {
            DamageableEnemy enemy = collision.collider.GetComponent<DamageableEnemy>();
            if (enemy == null) return;

            enemy.OnDeltDamage(5);
        }
    }

    private void GetUserInput()
    {
        isBlocking = Input.GetKey(KeyCode.Mouse1);

        if (Input.GetKeyDown(KeyCode.Z) && PlayerInfo.instance.hasShield)
        {
            HandleShieldEquipped();
        }

        if (PlayerInfo.instance.prayerCount > 0 && Input.GetKeyDown(KeyCode.Alpha1))
        {
            HandlePrayerUsed();
        }

        if (PlayerInfo.instance.healthPotionCount > 0 && Input.GetKeyDown(KeyCode.Alpha2))
        {
            HandleHealthPotionUsed();
        }

        if (isBlocking)
        {
            movement.rigidbody.velocity = new Vector2(0f, movement.rigidbody.velocity.y);
        }

        movement.SetDirection(new Vector2(Input.GetAxis("Horizontal"), 0f));
        movement.isJumping = (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space)) && !isBlocking;
    }

    private void HandleShieldEquipped()
    {
        if (PlayerInfo.instance.isShieldEquipped)
        {
            PlayerInfo.instance.isShieldEquipped = false;
            animator.SetBool("Using Shield", false);
            playerUI.ToggleBlockIconsActive(numberOfSwordBlockIcons, false);
        }
        else
        {
            PlayerInfo.instance.isShieldEquipped = true;
            animator.SetBool("Using Shield", true);
            playerUI.ToggleBlockIconsActive(numberOfSwordBlockIcons, true);
        }
    }

    private void HandleCombat()
    {
        if (isBlocking) return;

        blockedDamage = 0;
        playerUI.ResetBlockIcons();

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

    private void HandlePrayerUsed()
    {
        PlayerInfo.instance.prayerCount -= 1;
        playerUI.ChangePrayerSlots(1, false);

        isBlessed = true;
        blessedEffect.Play();
        // ParticleSystem.EmissionModule em = blessedEffect.emission;
        // em.enabled = true;
        StartCoroutine(HandleBlessedTimer());
    }

    private IEnumerator HandleBlessedTimer()
    {
        yield return new WaitForSeconds(blessedTime);
        blessedEffect.Stop();
        isBlessed = false;
    }

    private void HandleHealthPotionUsed()
    {
        PlayerInfo.instance.healthPotionCount -= 1;
        PlayerInfo.instance.health = PlayerInfo.instance.fullHealth;

        playerUI.ChangeHealthHearts(PlayerInfo.instance.fullHealth, true);
        playerUI.ChangeHealthPotionSlots(1, false);
    }

    private RaycastHit2D GetEnemyHit(float distance)
    {
        return Physics2D.BoxCast(transform.position, Vector2.one * 0.75f, 0.0f, movement.lookDirection, distance, enemyLayer);
    }

    public void OnDeltDamage(int damage)
    {
        damage = Mathf.Abs(damage);

        if (isBlessed || damage <= 0) return;

        if (isBlocking && blockedDamage < playerUI.blockIcons.Count)
        {
            audioSource.PlayOneShot(blockedAudioClip);
            blockedDamage += damage;
            playerUI.SetBlockIconsToBroken(damage);
            return;
        }

        PlayerInfo.instance.health -= damage;
        playerUI.ChangeHealthHearts(damage, false);

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
        PlayerInfo.instance.healthPotionCount += 1;
        playerUI.ChangeHealthPotionSlots(1, true);
    }

    public void OnRelicPickedUp()
    {
        PlayerInfo.instance.relicCount += 1;
        playerUI.ChangeRelicSlots(1, true);
    }

    public void OnPrayerPickedUp()
    {
        PlayerInfo.instance.prayerCount += 1;
        playerUI.ChangePrayerSlots(1, true);
    }

    public void OnDisable()
    {
        gameObject.SetActive(false);
    }
}
