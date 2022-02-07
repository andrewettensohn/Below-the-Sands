using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void StartGame()
    {
        PlayerInfo.instance.nextPlayerPositionOnLoad = new Vector2(-10.5f, 0.6f);
        SceneManager.LoadScene("CatacombEntrance");
    }
}
