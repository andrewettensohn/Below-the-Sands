using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    public bool isGamePaused;

    public Dictionary<string, bool> healthPotionAvailbility = new Dictionary<string, bool>
    {
        { "FirstLevelTreasureRoom1", true },
        { "FirstLevelTreasureRoom2", true },
        { "FirstLevelTrapRoom", true },
    };

    private AudioSource audioSource;
    private MusicTracks musicTracks;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        musicTracks = GetComponent<MusicTracks>();

        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
            HandleMusic("MainMenu");
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void HandleMusic(string sceneName)
    {
        if (sceneName == "MainMenu")
        {
            audioSource.Stop();
            audioSource.PlayOneShot(musicTracks.NightsRespite);
        }
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1;
        PlayerInfo.instance.ResetPlayerInfo();
        SceneManager.LoadScene("MainMenu");
        HandleMusic("MainMenu");
    }

    public void LoadScene(string sceneName, Vector2 positionAfterLoad)
    {
        PlayerInfo.instance.nextPlayerPositionOnLoad = positionAfterLoad;
        SceneManager.LoadScene(sceneName);
        HandleMusic(sceneName);
    }
}
