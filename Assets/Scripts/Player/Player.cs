using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Player : MonoBehaviour
{
    [Range(0.0f, 5.0f)]
    public float attackDelay;

    [Range(0.1f, 5.0f)]
    public float attackRange;

    public Transform attackPoint;
    public bool isGodModeEnabled;

    public LayerMask enemyLayer;
    public LayerMask npcLayer;
    public LayerMask transportTriggerLayer;
    public LayerMask projectileLayer;

    public GameObject PlayerUICanvas;
    public Movement movement { get; private set; }
    public PlayerUI playerUI { get; private set; }

    public AudioClip runningAudioClip;
    public AudioClip attackAudioClip;
    public AudioClip hitAudioClip;

    public float dashAbilityLength;
    public float deflectAbilityLength;
    public float rapidAttackAbilityLength;

    public bool isUsingAbility { get; private set; }

    private bool isAttacking;
    private bool canAttack = true;
    private Animator animator;
    private AudioSource audioSource;
    private bool isStaggered;
    private float defaultSpeed;
    private float defaultAttackRange;

    private void Awake()
    {
        movement = GetComponent<Movement>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        playerUI = PlayerUICanvas.GetComponent<PlayerUI>();
    }

    private void Start()
    {

        if (PlayerInfo.instance.nextPlayerPositionOnLoad != Vector2.zero)
        {
            movement.rigidbody.MovePosition(PlayerInfo.instance.nextPlayerPositionOnLoad);
        }

        defaultSpeed = movement.speed;
        defaultAttackRange = attackRange;
        playerUI.SyncHearts();
    }

    private void Update()
    {
        if (GameManager.instance.isGamePaused) return;

        if (GameManager.instance.isPlayerControlRestricted || (isAttacking && movement.isGrounded))
        {
            movement.rigidbody.velocity = new Vector2(0f, movement.rigidbody.velocity.y);
            AnimateMovement();
            return;
        }

        AnimateMovement();
        GetUserInput();
        HandleCombat();
    }

    private void GetUserInput()
    {

        if (PlayerInfo.instance.healthPotionCount > 0 && Input.GetKeyDown(KeyCode.E))
        {
            HandleHealthPotionUsed();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SwitchEquippedAbility();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            HandleEquippedAbility();
        }

        movement.SetDirection(new Vector2(Input.GetAxis("Horizontal"), 0f));
        movement.isJumping = (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space));
    }

    private void SwitchEquippedAbility()
    {
        if(isUsingAbility) return;

        int currentIndex = PlayerInfo.instance.AbilityOrder.IndexOf(PlayerInfo.instance.EquippedAbility);

        int newAbilityIndex = currentIndex + 1 >= PlayerInfo.instance.AbilityOrder.Count ? 0 : currentIndex + 1;

        PlayerInfo.instance.EquippedAbility = PlayerInfo.instance.AbilityOrder[newAbilityIndex];
        playerUI.SwapAbilityIcon(PlayerInfo.instance.EquippedAbility);

        Debug.Log($"Equipped {PlayerInfo.instance.EquippedAbility}");
    }

    private void HandleEquippedAbility()
    {
        if(isUsingAbility || PlayerInfo.instance.focusPoints <= 0) return;

        isUsingAbility = true;
        PlayerInfo.instance.focusPoints -= 1;

        if (PlayerInfo.instance.EquippedAbility == PlayerAbility.Dash)
        {
            Dash();
            playerUI.SetAbilityUseBackground(true);
            StartCoroutine(HandleAbilityTimer(dashAbilityLength));
        }
        else if (PlayerInfo.instance.EquippedAbility == PlayerAbility.Deflect)
        {
            Deflect();
            playerUI.SetAbilityUseBackground(true);
            StartCoroutine(HandleAbilityTimer(deflectAbilityLength));
        }
        else if (PlayerInfo.instance.EquippedAbility == PlayerAbility.RapidAttack)
        {
            playerUI.SetAbilityUseBackground(true);
            StartCoroutine(HandleAbilityTimer(rapidAttackAbilityLength));
        }
        else
        {
            isUsingAbility = false;
        }
    }

    private IEnumerator HandleAbilityTimer(float timerLength)
    {
        if (isUsingAbility)
        {
            yield return new WaitForSeconds(timerLength);

            isUsingAbility = false;
            attackRange = defaultAttackRange;
            movement.speed = defaultSpeed;
            playerUI.SetAbilityUseBackground(false);
        }
    }

    private void HandleDoorInteraction()
    {
        if (PlayerInfo.instance.isInDoorway)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, movement.lookDirection, 5.0f, transportTriggerLayer);

            DoorTrigger door = hit.collider.GetComponent<DoorTrigger>();
            door.TransportPlayer();
        }
    }

    private void Dash()
    {
        movement.speed = 8;
    }

    private void Deflect()
    {
        attackRange = 2.4f;
        HandleCombat(true, true);
    }

    private void HandleDialougeInteraction()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, movement.lookDirection, 5.0f, npcLayer);

        if (hit.collider != null)
        {
            Azkul azkul = hit.collider.GetComponent<Azkul>();
            azkul.OpenDialog();
            GameManager.instance.isPlayerControlRestricted = true;
        }
    }

    private void HandleCombat(bool overrideUserInput = false, bool canHitArrows = false)
    {

        if ((!Input.GetKeyDown(KeyCode.Mouse0) || !canAttack) && !overrideUserInput) return;

        isAttacking = true;
        canAttack = false;
        int damageToDeal = 1;

        if (isUsingAbility && PlayerInfo.instance.EquippedAbility == PlayerAbility.RapidAttack)
        {
            animator.SetTrigger("Rapid Attack");
            damageToDeal = 2;
        }
        else
        {
            animator.SetTrigger("Attack");
        }

        audioSource.PlayOneShot(attackAudioClip);

        List<Collider2D> hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer).ToList();

        if (canHitArrows)
        {
            hitEnemies.AddRange(Physics2D.OverlapCircleAll(attackPoint.position, attackRange, projectileLayer));
        }

        foreach (Collider2D hit in hitEnemies)
        {
            if (hit != null)
            {
                DamageableEnemy enemy = hit.GetComponent<DamageableEnemy>();

                enemy.OnDeltDamage(damageToDeal, this);
            }
        }

        StartCoroutine(HandleAttackDelayTimer());
    }

    private IEnumerator HandleAttackDelayTimer()
    {
        if (!canAttack)
        {
            yield return new WaitForSeconds(attackDelay);

            isAttacking = false;
            isStaggered = false;
            canAttack = true;
            animator.ResetTrigger("Attack");
            animator.ResetTrigger("RapidAttack");
        }
    }

    private void HandleHealthPotionUsed()
    {
        PlayerInfo.instance.healthPotionCount -= 1;
        PlayerInfo.instance.health = PlayerInfo.instance.fullHealth;

        playerUI.ChangeHealthHearts(PlayerInfo.instance.fullHealth, true);
        playerUI.SyncHealthPotCount();
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    public void OnDeltDamage(int damage, bool overrideBlocking = false)
    {
        damage = Mathf.Abs(damage);

        if (damage <= 0) return;

        if (isGodModeEnabled)
        {
            animator.SetTrigger("Hit");
            isStaggered = true;
            return;
        }

        PlayerInfo.instance.health -= damage;
        playerUI.ChangeHealthHearts(damage, false);
        audioSource.PlayOneShot(hitAudioClip);

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

    private void AnimateMovement()
    {
        animator.SetFloat("Speed", movement.rigidbody.velocity.sqrMagnitude);
        animator.SetFloat("Move Y", movement.rigidbody.velocity.y);
        animator.SetFloat("Look X", 1f);
        animator.SetBool("Is Grounded", movement.isGrounded);
    }

    public void OnHealthPotionPickedUp()
    {
        PlayerInfo.instance.healthPotionCount += 1;
        playerUI.SyncHealthPotCount();
    }

    public void OnDisable()
    {
        gameObject.SetActive(false);
    }
}
