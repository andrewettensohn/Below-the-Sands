using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    public TMP_InputField debugLevelInput;

    public void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void StartGame()
    {
        GameManager.instance.milestones = new Milestones();
        GameManager.instance.milestones.IsFistVisitToCatacomb = true;

        GameManager.instance.ResetProgress();
        PlayerInfo.instance.ResetPlayerInfo();
        GameManager.instance.healthPotionAvailbility = CollectableDictionaryHelper.GetCollectableDictionaryForEnum<HealthPotionName>();

        GameManager.instance.isIntroCutscenePlaying = true;
        GameManager.instance.LoadScene("IntroCutscene");
    }

    public void ContinueGame()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        bool isLastSceneKeyPresent = PlayerPrefs.HasKey("LastScene");

        if(isLastSceneKeyPresent == false)
        {
            StartGame();
            return;
        }
        
        GameManager.instance.LoadProgress();
    }

    public void LoadLevel()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        GameManager.instance.ResetProgress();
        PlayerInfo.instance.nextPlayerPositionOnLoad = Vector2.zero;

        string level = debugLevelInput.text;
        SceneManager.LoadScene(level);
    }

    public void ExitGame() => Application.Quit();
}
