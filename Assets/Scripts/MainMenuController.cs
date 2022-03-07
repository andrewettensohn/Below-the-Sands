using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void StartGame()
    {
        GameManager.instance.milestones = new Milestones();
        GameManager.instance.milestones.IsFistVisitToCatacomb = true;

        PlayerInfo.instance.ResetPlayerInfo();
        GameManager.instance.relicAvailbility = CollectableDictionaryHelper.GetCollectableDictionaryForEnum<RelicName>();
        GameManager.instance.healthPotionAvailbility = CollectableDictionaryHelper.GetCollectableDictionaryForEnum<HealthPotionName>();
        GameManager.instance.prayerAvailbility = CollectableDictionaryHelper.GetCollectableDictionaryForEnum<PrayerName>();

        PlayerInfo.instance.nextPlayerPositionOnLoad = new Vector2(-10.5f, 0.6f);
        SceneManager.LoadScene("CatacombEntrance");
    }

    public void ExitGame() => Application.Quit();
}
