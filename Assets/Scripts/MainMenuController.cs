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

        GameManager.instance.ResetProgress();
        PlayerInfo.instance.ResetPlayerInfo();
        GameManager.instance.healthPotionAvailbility = CollectableDictionaryHelper.GetCollectableDictionaryForEnum<HealthPotionName>();

        GameManager.instance.isIntroCutscenePlaying = true;
        GameManager.instance.LoadScene("SurfaceStage");
    }

    public void ContinueGame()
    {
        bool isLastSceneKeyPresent = PlayerPrefs.HasKey("LastScene");

        if(isLastSceneKeyPresent == false)
        {
            StartGame();
            return;
        }
        
        GameManager.instance.LoadProgress();
    }

    public void ExitGame() => Application.Quit();
}
