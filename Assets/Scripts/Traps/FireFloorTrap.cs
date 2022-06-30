using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FireFloorTrap : MonoBehaviour
{

    public AnimationClip FlameOnClip;
    public List<Sprite> DamageSprites;
    private SpriteRenderer spriteRenderer;
    private Player playerGameObject;
    private bool hasDeltDamage;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Ronin")
        {
            Player player = collision.GetComponent<Player>();
            playerGameObject = player;
        }
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
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

    private void OnTriggerExit2D(Collider2D collider)
    {
        playerGameObject = null;
    }

}
