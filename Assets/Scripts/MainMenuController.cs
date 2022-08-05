using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void StartGame()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        GameManager.instance.milestones = new Milestones();
        GameManager.instance.milestones.IsFistVisitToCatacomb = true;

        PlayerInfo.instance.ResetPlayerInfo();
        GameManager.instance.healthPotionAvailbility = CollectableDictionaryHelper.GetCollectableDictionaryForEnum<HealthPotionName>();

        GameManager.instance.isIntroCutscenePlaying = true;
        GameManager.instance.LoadScene("SurfaceStage");
    }

    public void ExitGame() => Application.Quit();
}
