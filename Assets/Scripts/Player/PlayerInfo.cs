using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerInfo : MonoBehaviour
{
    public static PlayerInfo instance { get; private set; }

    public Vector2 nextPlayerPositionOnLoad { get; set; }

    public int healthPotionCount { get; set; }

    public int relicCount { get; set; }

    public int prayerCount { get; set; }

    public int health { get; set; }

    public readonly int fullHealth = 5;

    public bool isShieldEquipped { get; set; }

    public bool hasShield { get; set; }

    public bool isTwoHandSwordEquipped { get; set; }

    public bool hasTwoHandSword { get; set; }

    public bool isInDoorway { get; set; }

    public bool isBlessed;

    public int focusPoints;

    public PlayerAbility EquippedAbility = PlayerAbility.Dash;

    public List<PlayerAbility> AbilityOrder = new List<PlayerAbility> { PlayerAbility.Dash, PlayerAbility.Deflect, PlayerAbility.RapidAttack };

    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
            health = fullHealth;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void ResetPlayerInfo()
    {
        hasShield = false;
        hasTwoHandSword = false;
        isShieldEquipped = false;
        isTwoHandSwordEquipped = false;
        healthPotionCount = 1;
        relicCount = 0;
        prayerCount = 0;
        health = fullHealth;
        focusPoints = 3;
        EquippedAbility = PlayerAbility.Dash;
    }
}
