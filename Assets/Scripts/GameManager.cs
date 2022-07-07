using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    public bool isGamePaused;

    public bool isPlayerControlRestricted;

    public MenuController menuController;

    public Milestones milestones = new Milestones { IsFistVisitToCatacomb = true };

    public Dictionary<HealthPotionName, bool> healthPotionAvailbility = CollectableDictionaryHelper.GetCollectableDictionaryForEnum<HealthPotionName>();

    public Dictionary<RelicName, bool> relicAvailbility = CollectableDictionaryHelper.GetCollectableDictionaryForEnum<RelicName>();

    public Dictionary<PrayerName, bool> prayerAvailbility = CollectableDictionaryHelper.GetCollectableDictionaryForEnum<PrayerName>();

    public bool isIntroCutscenePlaying;
    public bool isEndGameCutscenePlaying;

    public List<LevelName> FirstLevelMusicTriggers;
    public List<LevelName> SecondLevelMusicTriggers;
    public List<LevelName> ThirdLevelMusicTriggers;

    public int score { get; private set; }

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

        if (FirstLevelMusicTriggers.Any(x => x.ToString() == sceneName) && audioSource.clip != musicTracks.FirstLayerTrack)
        {
            audioSource.clip = musicTracks.FirstLayerTrack;
        }
        else if (sceneName == "MainMenu" || LevelName.CatacombEntrance.ToString() == sceneName)
        {
            audioSource.clip = musicTracks.MainMenuTrack;
        }
        else if (SecondLevelMusicTriggers.Any(x => x.ToString() == sceneName) && audioSource.clip != musicTracks.SecondLayerTrack)
        {
            audioSource.clip = musicTracks.SecondLayerTrack;
        }
        else if (ThirdLevelMusicTriggers.Any(x => x.ToString() == sceneName) && audioSource.clip != musicTracks.ThirdLayerTrack)
        {
            audioSource.clip = musicTracks.ThirdLayerTrack;
        }
        else if (LevelName.ThirdLevelBossRoom.ToString() == sceneName)
        {
            audioSource.clip = musicTracks.BossFightTrack;
        }
        else if (sceneName == "EndGameCutscene")
        {
            audioSource.clip = musicTracks.FinalTrack;
        }
        else
        {
            return;
        }

        audioSource.Stop();
        audioSource.Play();
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

    public void PlayEndGameCutscene()
    {
        if (!isEndGameCutscenePlaying)
        {
            isEndGameCutscenePlaying = true;
            StartCoroutine(WaitForEndGameCutscene(6));
        }
    }

    private IEnumerator WaitForEndGameCutscene(float secondsToWait)
    {
        yield return new WaitForSeconds(secondsToWait);
        SceneManager.LoadScene("EndGameCutscene");
    }

    public void UpdateScore(int scoreValue)
    {
        score += scoreValue;
    }

    public void GameOver()
    {
        menuController.IsPauseMode(false);
        menuController.PauseGame();
    }
}
