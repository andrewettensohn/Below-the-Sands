using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MagicTrap : VerticalTrap
{
    public List<GameObject> dangerZoneGameObjects;
    private List<DangerZone> dangerZones;

    private void Start()
    {
        dangerZones = new List<DangerZone>();

        foreach(GameObject gameObject in dangerZoneGameObjects)
        {
            bool isDangerZone = gameObject.TryGetComponent<DangerZone>(out DangerZone dangerZone);

            if(isDangerZone) dangerZones.Add(dangerZone);
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == GameManager.instance.playerCharacterName)
        {
            Player player = collision.GetComponent<Player>();
            playerGameObject = player;
        }
    }

    protected override void OnTriggerStay2D(Collider2D collider)
    {
        if (playerGameObject == null) return;

        bool shouldDealDamage = dangerZones.Any(x => x.shouldDoDamage);

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

    protected override void OnTriggerExit2D(Collider2D collider)
    {
        playerGameObject = null;
    }
}
