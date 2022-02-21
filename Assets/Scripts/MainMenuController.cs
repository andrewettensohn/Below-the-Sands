using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void StartGame()
    {
        GameManager.instance.azkulDialogStatus = new AzkulDialogStatus();
        GameManager.instance.azkulDialogStatus.IsNewGame = true;

        PlayerInfo.instance.ResetPlayerInfo();

        PlayerInfo.instance.nextPlayerPositionOnLoad = new Vector2(-10.5f, 0.6f);
        SceneManager.LoadScene("CatacombEntrance");
    }

    public void ExitGame() => Application.Quit();
}