using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    public bool isGamePaused;

    public bool isPlayerControlRestricted;

    public Milestones milestones = new Milestones { IsFistVisitToCatacomb = true };

    public Dictionary<LevelName, bool> healthPotionAvailbility = new Dictionary<LevelName, bool>
    {
        { LevelName.FirstLevelTreasureRoom, true },
        { LevelName.FirstLevelTrapRoom, true },
    };

    public Dictionary<LevelName, bool> relicAvailbility = new Dictionary<LevelName, bool>
    {
        { LevelName.FirstLevelTreasureRoom, true },
    };

    public Dictionary<LevelName, bool> prayerAvailbility = new Dictionary<LevelName, bool>
    {
        { LevelName.FirstLevelTreasureRoom, true },
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
