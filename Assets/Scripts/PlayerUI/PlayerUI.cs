using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using System;

public class PlayerUI : MonoBehaviour
{
    public GameObject healthPotObject;
    private TMP_Text healthPotCount;
    public GameObject scoreObject;
    private TMP_Text score;
    private List<HealthHeart> hearts;

    private void Awake()
    {
        hearts = GetComponentsInChildren<HealthHeart>().ToList();
        healthPotCount = healthPotObject.GetComponent<TMP_Text>();
        score = scoreObject.GetComponent<TMP_Text>();
    }

    private void Start()
    {
        SyncHealthPotCount();
        score.text = GameManager.instance.score.ToString();
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
