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

    [Range(1, 60)]
    public float blessedTime;

    public LayerMask enemyLayer;
    public LayerMask npcLayer;
    public GameObject PlayerUICanvas;
    public Movement movement { get; private set; }
    public PlayerUI playerUI { get; private set; }
    public bool isInteractKeyDown { get; private set; }

    public ParticleSystem blessedEffect;

    public AudioClip runningAudioClip;
    public AudioClip swingWithSwordAudioClip;
    public AudioClip hitWithSwordAudioClip;
    public AudioClip blockedAudioClip;

    private bool isAttacking;
    private bool canAttack = true;
    private Animator animator;

    private AudioSource audioSource;

    private bool isBlessed;
    private bool isBlocking;
    private int blockedDamage;
    private bool isStaggered;

    private void Awake()
    {
        movement = GetComponent<Movement>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        playerUI = PlayerUICanvas.GetComponent<PlayerUI>();
    }

    private void Start()
    {

        if (PlayerInfo.instance.isShieldEquipped)
        {
            ToggleShieldEquipped(true);
        }
        else if (PlayerInfo.instance.isTwoHandSwordEquipped)
        {
            ToggleTwoHandSwordEquipped(true);
        }
        else
        {
            playerUI.ToggleBlockIconsActive(4, false);
        }

        if (PlayerInfo.instance.nextPlayerPositionOnLoad != Vector2.zero)
        {
            movement.rigidbody.MovePosition(PlayerInfo.instance.nextPlayerPositionOnLoad);
        }

        playerUI.SyncHearts();
    }

    private void Update()
    {
        if (GameManager.instance.isGamePaused) return;

        if (GameManager.instance.isPlayerControlRestricted)
        {
            movement.rigidbody.velocity = new Vector2(0f, movement.rigidbody.velocity.y);
            AnimateMovement();
            return;
        }

        AnimateMovement();
        GetUserInput();
        PlaySound();
        HandleCombat();
    }

    private void FixedUpdate()
    {
        if (isBlessed)
        {
            HandleBlessedAttack();
        }
    }

    private void HandleBlessedAttack()
    {
        RaycastHit2D hit = GetEnemyHit(1.0f);
        DamageableEnemy enemy = hit.collider?.GetComponent<DamageableEnemy>();
        if (enemy == null) return;

        enemy.OnDeltDamage(5);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (isInteractKeyDown)
        {
            UseDoor(collider);
        }
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (isInteractKeyDown)
        {
            UseDoor(collider);
        }
    }

    private void UseDoor(Collider2D collider)
    {
        if (collider.tag == "Doors")
        {
            DoorTrigger door = collider.GetComponent<DoorTrigger>();
            door.TransportPlayer();
        }
    }

    private void GetUserInput()
    {
        isBlocking = Input.GetKey(KeyCode.Mouse1);

        isInteractKeyDown = Input.GetKeyDown(KeyCode.E);

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            HandleEquippedEquipment();
        }

        if (PlayerInfo.instance.prayerCount > 0 && Input.GetKeyDown(KeyCode.Alpha2))
        {
            HandlePrayerUsed();
        }

        if (PlayerInfo.instance.healthPotionCount > 0 && Input.GetKeyDown(KeyCode.Alpha1))
        {
            HandleHealthPotionUsed();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            HandleInteractAction();
        }

        if (isBlocking && !PlayerInfo.instance.isShieldEquipped)
        {
            movement.rigidbody.velocity = new Vector2(0f, movement.rigidbody.velocity.y);
        }

        movement.SetDirection(new Vector2(Input.GetAxis("Horizontal"), 0f));
        movement.isJumping = (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space)) && (!isBlocking || PlayerInfo.instance.isShieldEquipped);
    }

    private void HandleInteractAction()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, movement.lookDirection, 5.0f, npcLayer);

        if (hit.collider != null)
        {
            Azkul azkul = hit.collider.GetComponent<Azkul>();
            azkul.OpenDialog();
            GameManager.instance.isPlayerControlRestricted = true;
        }
    }

    private void HandleEquippedEquipment()
    {

        if (!PlayerInfo.instance.hasShield && !PlayerInfo.instance.hasTwoHandSword) return;

        bool shouldEquipNormalSword = !PlayerInfo.instance.hasTwoHandSword && PlayerInfo.instance.hasShield && PlayerInfo.instance.isShieldEquipped;
        bool shouldEquipShield = PlayerInfo.instance.hasShield && !PlayerInfo.instance.isShieldEquipped;
        bool shouldEquipTwoHandSword = PlayerInfo.instance.hasTwoHandSword && !PlayerInfo.instance.isTwoHandSwordEquipped;

        if (shouldEquipNormalSword)
        {
            ToggleShieldEquipped(false);
        }
        else if (shouldEquipShield)
        {
            ToggleShieldEquipped(true);
            ToggleTwoHandSwordEquipped(false);
        }
        else if (shouldEquipTwoHandSword)
        {
            ToggleShieldEquipped(false);
            ToggleTwoHandSwordEquipped(true);
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

                int damageToDeal = PlayerInfo.instance.isTwoHandSwordEquipped ? 3 : 1;
                DamageableEnemy enemy = hit.collider.GetComponent<DamageableEnemy>();
                enemy.OnDeltDamage(damageToDeal);
            }

            isAttacking = false;
            isStaggered = false;
            canAttack = true;
        }
    }

    private void HandlePrayerUsed()
    {
        PlayerInfo.instance.prayerCount -= 1;
        playerUI.SyncPrayerCount();

        isBlessed = true;
        blessedEffect.Play();
        StartCoroutine(nameof(HandleBlessedTimer));
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
        playerUI.SyncHealthPotCount();
    }

    private RaycastHit2D GetEnemyHit(float distance)
    {
        return Physics2D.BoxCast(transform.position, Vector2.one * 0.75f, 0.0f, movement.lookDirection, distance, enemyLayer);
    }

    public void ToggleShieldEquipped(bool isShiledEquipped)
    {
        PlayerInfo.instance.isShieldEquipped = isShiledEquipped;
        animator.SetBool("Using Shield", isShiledEquipped);
        playerUI.ToggleBlockIconsActive(4, !isShiledEquipped);
    }

    public void ToggleTwoHandSwordEquipped(bool isTwoHandSwordEquipped)
    {
        PlayerInfo.instance.isTwoHandSwordEquipped = isTwoHandSwordEquipped;
        animator.SetBool("Using Two Hand Sword", isTwoHandSwordEquipped);
        playerUI.ToggleBlockIconsActive(5, !isTwoHandSwordEquipped);
    }

    public void OnDeltDamage(int damage, bool overrideBlocking = false)
    {
        damage = Mathf.Abs(damage);

        if (isBlessed || damage <= 0) return;

        if (isBlocking && !overrideBlocking && blockedDamage < playerUI.blockIcons.Count(x => x.isBlockIconActive))
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
            GameManager.instance.GameOver();
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
        playerUI.SyncHealthPotCount();
    }

    public void OnRelicPickedUp()
    {
        PlayerInfo.instance.relicCount += 1;
        playerUI.SyncRelicCount();
    }

    public void OnPrayerPickedUp()
    {
        PlayerInfo.instance.prayerCount += 1;
        playerUI.SyncPrayerCount();
    }

    public void OnDisable()
    {
        gameObject.SetActive(false);
    }
}
