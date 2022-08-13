using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DangerZone : MonoBehaviour
{
    public bool shouldDoDamage;
    public List<Sprite> DamageSprites;
    public GameObject parentTrap;
    private MagicTrap magicTrap;

    private void Start()
    {
        magicTrap = parentTrap.GetComponent<MagicTrap>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CheckIfPlayerShouldBeDamaged();
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        CheckIfPlayerShouldBeDamaged();
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        shouldDoDamage = false;
    }

    private void CheckIfPlayerShouldBeDamaged()
    {
        shouldDoDamage = DamageSprites.Any(x => x.name == magicTrap.spriteRenderer.sprite.name);
    }
}
