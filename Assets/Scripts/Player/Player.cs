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

    public GameObject fireballPrefab;
    public float fireballLaunchForce;

    public GameObject PlayerUICanvas;
    public Movement movement { get; private set; }
    public PlayerUI playerUI { get; private set; }

    public AudioClip runningAudioClip;
    public AudioClip attackAudioClip;
    public AudioClip hitAudioClip;

    public float dashAbilityLength;
    public float deflectAbilityLength;
    public float rapidAttackAbilityLength;
    public float spiritBlastCoolDownLength;
    public float staggerTime;

    public bool isUsingAbility { get; private set; }

    private bool isAttacking;
    private bool canAttack = true;
    private Animator animator;
    private AudioSource audioSource;
    private bool isStaggered;
    private bool isStaggerTimerActive;
    public float defaultSpeed { get; private set; }
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
        PlayerInfo.instance.isSpirit = false;
        PlayerInfo.instance.health = PlayerInfo.instance.fullHealth;

        if (PlayerInfo.instance.nextPlayerPositionOnLoad != Vector2.zero)
        {
            Debug.Log(PlayerInfo.instance.nextPlayerPositionOnLoad.y);
            transform.position = PlayerInfo.instance.nextPlayerPositionOnLoad;
        }

        defaultSpeed = movement.speed;
        defaultAttackRange = attackRange;
    }

    private void Update()
    {
        PlayerInfo.instance.playerPosition = transform.position;

        if (GameManager.instance.isGamePaused) return;

        if(isStaggered && !isStaggerTimerActive)
        {
            isStaggerTimerActive = true;
            StartCoroutine(HandleStaggerTimer());
            return;
        }

        if(isStaggered)
        {
            movement.speed = defaultSpeed * 0.50f;
        }

        if (GameManager.instance.isPlayerControlRestricted || (isAttacking && movement.isGrounded))
        {
            movement.rigidbody.velocity = new Vector2(0f, movement.rigidbody.velocity.y);
            AnimateMovement();
            return;
        }

        movement.canFloat = PlayerInfo.instance.isSpirit;

        AnimateMovement();
        GetUserInput();
        HandleCombat();
    }

    private void GetUserInput()
    {

        if (PlayerInfo.instance.healthPotionCount > 0 && Input.GetKeyDown(KeyCode.E) && !PlayerInfo.instance.isSpirit)
        {
            HandleHealthPotionUsed();
        }

        if(Input.GetKeyDown(KeyCode.LeftShift) && !isUsingAbility)
        {
            Dash();
        }

        if(Input.GetKeyDown(KeyCode.Q) && !isUsingAbility)
        {
            Deflect();
        }

        if(Input.GetKeyDown(KeyCode.Mouse1) && !isUsingAbility)
        {
            if(PlayerInfo.instance.isSpirit)
            {
                SpiritBlast();
            }
            else
            {
                RapidAttack();
            }
        }

        if(PlayerInfo.instance.isSpirit)
        {
            movement.SetDirection(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
        }
        else
        {
            movement.SetDirection(new Vector2(Input.GetAxis("Horizontal"), 0f));
        }

        movement.isJumping = (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space));
        
    }

    private IEnumerator HandleAbilityTimer(float timerLength, PlayerAbility playerAbility)
    {
        if (isUsingAbility)
        {
            yield return new WaitForSeconds(timerLength);

            isUsingAbility = false;
            attackRange = defaultAttackRange;
            movement.speed = defaultSpeed;
            playerUI.SetAbilityUseActive(playerAbility, false);
        }
    }

    private IEnumerator HandleStaggerTimer()
    {
        yield return new WaitForSeconds(staggerTime);

        isStaggered = false;
        isStaggerTimerActive = false; 
        movement.speed = defaultSpeed;
    }

    private void Dash()
    {
        isUsingAbility = true;
        PlayerInfo.instance.EquippedAbility = PlayerAbility.Dash;

        playerUI.SetAbilityUseActive(PlayerAbility.Dash, true);
        StartCoroutine(HandleAbilityTimer(dashAbilityLength, PlayerAbility.Dash));

        movement.speed = 8;
    }

    private void Deflect()
    {
        isUsingAbility = true;
        PlayerInfo.instance.EquippedAbility = PlayerAbility.Deflect;

        playerUI.SetAbilityUseActive(PlayerAbility.Deflect, true);
        StartCoroutine(HandleAbilityTimer(deflectAbilityLength, PlayerAbility.Deflect));

        attackRange = 2.4f;
        HandleCombat(true, true);
    }

    private void SpiritBlast()
    {
        isUsingAbility = true;

        PlayerInfo.instance.EquippedAbility = PlayerAbility.SpiritBlast;

        playerUI.SetAbilityUseActive(PlayerAbility.SpiritBlast, true);
        StartCoroutine(HandleAbilityTimer(spiritBlastCoolDownLength, PlayerAbility.SpiritBlast));

        animator.SetTrigger("Attack");
        GameObject fireballGameObject = Instantiate(fireballPrefab, attackPoint.transform.position, Quaternion.identity);

        Fireball fireball = fireballGameObject.GetComponent<Fireball>();
        fireball.Launch(new Vector2(movement.lookDirection.x, 0), fireballLaunchForce);
    }

    private void RapidAttack()
    {
        isUsingAbility = true;
        PlayerInfo.instance.EquippedAbility = PlayerAbility.RapidAttack;
        playerUI.SetAbilityUseActive(PlayerAbility.RapidAttack, true);

        StartCoroutine(HandleAbilityTimer(rapidAttackAbilityLength, PlayerAbility.RapidAttack));
    }

    private void HandleCombat(bool overrideUserInput = false, bool canHitArrows = false)
    {

        if ((!Input.GetKeyDown(KeyCode.Mouse0) || !canAttack) && !overrideUserInput || isStaggered) return;

        movement.canMove = false;
        isAttacking = true;
        canAttack = false;
        int damageToDeal =  PlayerInfo.instance.isSpirit ? 2 : 1;

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
            canAttack = true;
            movement.canMove = true;
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

        if (damage <= 0 || isStaggered) return;

        if (isGodModeEnabled)
        {
            animator.SetTrigger("Hit");
            isStaggered = true;
            return;
        }

        PlayerInfo.instance.health -= damage;
        playerUI.ChangeHealthHearts(damage, false);
        audioSource.PlayOneShot(hitAudioClip);

        if (PlayerInfo.instance.health <= 0 && !PlayerInfo.instance.isSpirit)
        {
            animator.SetTrigger("Die");
            Invoke(nameof(OnDisable), 1.3f);
            GameManager.instance.GameOver();
        }
        else if(PlayerInfo.instance.health <= 0 && PlayerInfo.instance.isSpirit)
        {
            animator.SetTrigger("Leave Spirit Form");
            PlayerInfo.instance.health = PlayerInfo.instance.fullHealth;
            playerUI.ChangeHealthHearts(PlayerInfo.instance.fullHealth, true);
            PlayerInfo.instance.isSpirit = false;
            playerUI.SwapToNormalAbilities();
        }
        else
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
        animator.SetBool("Is Spirit", PlayerInfo.instance.isSpirit);
    }

    public void OnHealthPotionPickedUp()
    {
        PlayerInfo.instance.healthPotionCount += 1;
        playerUI.SyncHealthPotCount();
    }

    public void OnSpiritPowerupPickedUp()
    {
        animator.SetTrigger("Become Spirit");
        PlayerInfo.instance.isSpirit = true;
        PlayerInfo.instance.health = PlayerInfo.instance.fullHealth;
        playerUI.ChangeHealthHearts(PlayerInfo.instance.fullHealth, true);
        playerUI.SwapToSpiritAbilities();
    }

    public void OnDisable()
    {
        gameObject.SetActive(false);
    }
}
