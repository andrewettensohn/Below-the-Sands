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

    public LayerMask enemyLayer;
    public LayerMask npcLayer;
    public LayerMask transportTriggerLayer;
    public GameObject PlayerUICanvas;
    public Movement movement { get; private set; }
    public PlayerUI playerUI { get; private set; }
    public bool isInteractKeyDown { get; private set; }

    public AudioClip runningAudioClip;
    public AudioClip attackAudioClip;
    public AudioClip hitAudioClip;

    private bool isAttacking;
    private bool canAttack = true;
    private Animator animator;

    private AudioSource audioSource;

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
        HandleCombat();
    }

    private void GetUserInput()
    {

        isInteractKeyDown = Input.GetKeyDown(KeyCode.E);

        if (PlayerInfo.instance.healthPotionCount > 0 && Input.GetKeyDown(KeyCode.Alpha1))
        {
            HandleHealthPotionUsed();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            HandleDoorInteraction();
            HandleDialougeInteraction();
        }

        movement.SetDirection(new Vector2(Input.GetAxis("Horizontal"), 0f));
        movement.isJumping = (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space));
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

    private void HandleCombat()
    {
        playerUI.ResetBlockIcons();

        if (!Input.GetKeyDown(KeyCode.Mouse0) || !canAttack) return;

        isAttacking = true;
        canAttack = false;
        audioSource.PlayOneShot(attackAudioClip);
        animator.SetTrigger("Attacking");
        RaycastHit2D hit = GetEnemyHit(attackRange);
        if (hit.collider != null && !isStaggered)
        {
            int damageToDeal = PlayerInfo.instance.isTwoHandSwordEquipped ? 3 : 1;
            DamageableEnemy enemy = hit.collider.GetComponent<DamageableEnemy>();
            enemy.OnDeltDamage(damageToDeal);
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
        }
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
        //return Physics2D.BoxCast(transform.position, Vector2.one * 0.75f, 0.0f, movement.lookDirection, distance, enemyLayer);
        return Physics2D.CircleCast(transform.position, distance, movement.lookDirection, distance, enemyLayer);
    }

    public void OnDeltDamage(int damage, bool overrideBlocking = false)
    {
        damage = Mathf.Abs(damage);

        if (damage <= 0) return;

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
        animator.SetFloat("Look X", movement.lookDirection.x);
        animator.SetBool("Is Grounded", movement.isGrounded);
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
