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

    public Dictionary<HealthPotionName, bool> healthPotionAvailbility = new Dictionary<HealthPotionName, bool>
    {
        { HealthPotionName.FirstLevelTreasureRoom1, true },
        { HealthPotionName.FirstLevelTrapRoom1, true },
    };

    public Dictionary<RelicName, bool> relicAvailbility = new Dictionary<RelicName, bool>
    {
        { RelicName.FirstLevelTreasureRoom1, true },
        { RelicName.FirstLevelRelicRoom1, true },
        { RelicName.FirstLevelRelicRoom2, true },
    };

    public Dictionary<PrayerName, bool> prayerAvailbility = new Dictionary<PrayerName, bool>
    {

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
