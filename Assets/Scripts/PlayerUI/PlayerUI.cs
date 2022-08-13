using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using System;

public class PlayerUI : MonoBehaviour
{
    public GameObject healthPotCountObject;
    public GameObject focusPointCountObject;
    public GameObject healthPotIconObject;

    public GameObject dashUseBackground;
    public GameObject deflectUseBackground;

    public GameObject spiritSuiteIconObject;
    public GameObject spiritUseBackground;

    public GameObject rapidSuiteObject;
    public GameObject rapidUseBackground;

    public GameObject ProgressSavedGameObject;
    private TMP_Text healthPotCount;
    private TMP_Text focusPointCount;

    private List<HealthHeart> hearts;

    private void Awake()
    {
        hearts = GetComponentsInChildren<HealthHeart>().ToList();
        healthPotCount = healthPotCountObject.GetComponent<TMP_Text>();
        focusPointCount = focusPointCountObject.GetComponent<TMP_Text>();
    }

    private void Start()
    {
        SyncHealthPotCount();

        SetAllAbilityUseToInactive();
        SwapToNormalAbilities();
        // SetAllAbilityIconsToInactive();
        // SetAbilityUseBackground(false);

        SyncHearts();

        ProgressSavedGameObject.SetActive(false);
    }

    public void SyncHealthPotCount() => healthPotCount.text = PlayerInfo.instance.healthPotionCount.ToString();

    public void SyncHearts()
    {
        int heartsNeededToChange = PlayerInfo.instance.fullHealth - PlayerInfo.instance.health;

        ChangeHealthHearts(heartsNeededToChange, false);
    }

    public void SetAllAbilityUseToInactive()
    {
        dashUseBackground.SetActive(false);
        deflectUseBackground.SetActive(false);
        rapidUseBackground.SetActive(false);
        spiritUseBackground.SetActive(false);
    }

    public void SetAbilityUseActive(PlayerAbility playerAbility, bool isActive)
    {
        switch (playerAbility)
        {
            case PlayerAbility.Dash:
                dashUseBackground.SetActive(isActive);
                break; 
            case PlayerAbility.Deflect:
                deflectUseBackground.SetActive(isActive);
                break; 
            case PlayerAbility.RapidAttack:
                rapidUseBackground.SetActive(isActive);
                break; 
            case PlayerAbility.SpiritBlast:
                spiritUseBackground.SetActive(isActive);
                break;
        }
    }

    public void SwapToSpiritAbilities()
    {
        rapidSuiteObject.SetActive(false);
        spiritSuiteIconObject.SetActive(true);
    }

    public void SwapToNormalAbilities()
    {
        rapidSuiteObject.SetActive(true);
        spiritSuiteIconObject.SetActive(false);
    }

    // public void SwapAbilityIcon(PlayerAbility newAbility)
    // {
    //     SetAllAbilityIconsToInactive();

    //     if (newAbility == PlayerAbility.Dash)
    //     {
    //         dashIconObject.SetActive(true);
    //     }
    //     else if (newAbility == PlayerAbility.Deflect)
    //     {
    //         deflectIconObject.SetActive(true);
    //     }
    //     else if (newAbility == PlayerAbility.RapidAttack)
    //     {
    //         rapidIconObject.SetActive(true);
    //     }
    //     else if(newAbility == PlayerAbility.SpiritBlast)
    //     {
    //         spiritBlastIconObject.SetActive(true);
    //     }
    // }

    // public void SetAllAbilityIconsToInactive()
    // {
    //     dashIconObject.SetActive(false);
    //     rapidIconObject.SetActive(false);
    //     deflectIconObject.SetActive(false);
    //     spiritBlastIconObject.SetActive(false);
    // }

    // public void SetAbilityUseBackground(bool isActive)
    // {
    //     AbilityUseBackground.SetActive(isActive);
    // }

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

    public void DisplayProgressSavedMessage()
    {
        ProgressSavedGameObject.SetActive(true);

        StartCoroutine(HandleProgressSavedMessageTimer());
    }

    private IEnumerator HandleProgressSavedMessageTimer()
    {
        yield return new WaitForSeconds(3.0f);

        ProgressSavedGameObject.SetActive(false);
    } 
}
