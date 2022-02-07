using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    public Dictionary<string, bool> healthPotionAvailbility = new Dictionary<string, bool>
    {
        { "FirstLevelTreasureRoom1", true },
        { "FirstLevelTreasureRoom2", true },
        { "FirstLevelTrapRoom", true },
    };

    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void LoadMainMenu()
    {
        PlayerInfo.instance.ResetPlayerInfo();
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadScene(string sceneName, Vector2 positionAfterLoad)
    {
        PlayerInfo.instance.nextPlayerPositionOnLoad = positionAfterLoad;
        SceneManager.LoadScene(sceneName);
    }
}
