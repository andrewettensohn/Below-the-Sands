using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerInfo : MonoBehaviour
{
    public static PlayerInfo instance { get; private set; }

    public Vector2 nextPlayerPositionOnLoad { get; set; }

    public int healthPotionCount { get; set; }

    public int health { get; set; }

    public readonly int fullHealth = 5;

    public bool isInDoorway { get; set; }

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
        healthPotionCount = 1;
        health = fullHealth;
        EquippedAbility = PlayerAbility.Dash;
    }
}
