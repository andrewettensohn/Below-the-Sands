using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using System;

public class PlayerUI : MonoBehaviour
{
    public GameObject healthPotCountObject;
    public GameObject healthPotIconObject;
    public GameObject dashIconObject;
    public GameObject deflectIconObject;
    public GameObject rapidIconObject;
    public GameObject AbilityUseBackground;
    private TMP_Text healthPotCount;
    public GameObject scoreObject;
    private TMP_Text score;
    private List<HealthHeart> hearts;

    private void Awake()
    {
        hearts = GetComponentsInChildren<HealthHeart>().ToList();
        healthPotCount = healthPotCountObject.GetComponent<TMP_Text>();
        score = scoreObject.GetComponent<TMP_Text>();
    }

    private void Start()
    {
        SyncHealthPotCount();
        score.text = GameManager.instance.score.ToString();

        SetAllAbilityIconsToInactive();
        SetAbilityUseBackground(false);
        dashIconObject.SetActive(true);
    }

    private void Update()
    {
        score.text = GameManager.instance.score.ToString();
    }

    public void SyncHealthPotCount() => healthPotCount.text = PlayerInfo.instance.healthPotionCount.ToString();

    public void SyncHearts()
    {
        int heartsNeededToChange = PlayerInfo.instance.fullHealth - PlayerInfo.instance.health;

        ChangeHealthHearts(heartsNeededToChange, false);
    }

    public void SwapAbilityIcon(PlayerAbility newAbility)
    {
        SetAllAbilityIconsToInactive();

        if (newAbility == PlayerAbility.Dash)
        {
            dashIconObject.SetActive(true);
        }
        else if (newAbility == PlayerAbility.Deflect)
        {
            deflectIconObject.SetActive(true);
        }
        else if (newAbility == PlayerAbility.RapidAttack)
        {
            rapidIconObject.SetActive(true);
        }
    }

    public void SetAllAbilityIconsToInactive()
    {
        dashIconObject.SetActive(false);
        rapidIconObject.SetActive(false);
        deflectIconObject.SetActive(false);
    }

    public void SetAbilityUseBackground(bool isActive)
    {
        AbilityUseBackground.SetActive(isActive);
    }

    public void ChangeHealthHearts(int heartsNeededToChange, bool isActive)
    {
        int heartsChanged = 0;
        for (int i = hearts.Count - 1; i > -1; i--)
        {
            if (heartsChanged >= heartsNeededToChange) break;

            if (hearts[i].isHealthy == true && isActive == false)
            {
                hearts[i].SetUnhealthy();
                heartsChanged++;
            }
            else if (hearts[i].isHealthy == false && isActive == true)
            {
                hearts[i].SetHealthy();
                heartsChanged++;
            }
        }
    }
}
