using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    public readonly string playerCharacterName = "Samurai";

    public bool isGamePaused;

    public bool isPlayerControlRestricted;

    public MenuController menuController;

    public Milestones milestones = new Milestones { IsFistVisitToCatacomb = true };

    public Dictionary<HealthPotionName, bool> healthPotionAvailbility = CollectableDictionaryHelper.GetCollectableDictionaryForEnum<HealthPotionName>();

    public bool isIntroCutscenePlaying;
    public bool isEndGameCutscenePlaying;

    public List<LevelName> FirstLevelMusicTriggers;
    public List<LevelName> SecondLevelMusicTriggers;
    public List<LevelName> ThirdLevelMusicTriggers;

    public bool canSaveGame = true;

    public int score { get; private set; }

    private AudioSource audioSource;
    public MusicTracks musicTracks;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        musicTracks = GetComponent<MusicTracks>();

        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
            HandleMusic();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void HandleMusic()
    {
        audioSource.Stop();

        string sceneName = SceneManager.GetActiveScene().name;

        if(sceneName == "MainMenu")
        {
            audioSource.clip = musicTracks.MainMenuTheme;
        }
        else if(sceneName == "VillageKost")
        {
            audioSource.clip = musicTracks.VillageKostTrack;
        }
        else if(sceneName == "SurfaceStage")
        {
            audioSource.clip = musicTracks.SurfaceLayerTrack;
        }
        else if(sceneName == "Layer1")
        {
            audioSource.clip = musicTracks.FirstLayerTrack;
        }
        else if(sceneName == "Layer2")
        {
            audioSource.clip = musicTracks.SecondLayerTrack;
        }
        else if(sceneName == "Layer3")
        {
            audioSource.clip = musicTracks.ThirdLayerTrack;
        }
        else if(sceneName == "Layer4")
        {
            audioSource.clip = musicTracks.FourthLayerTrack;
        }
        else if(sceneName == "FinalStage")
        {
            audioSource.clip = musicTracks.BossFightTrack;
        }
        else
        {
            return;
        }

        audioSource.Play();
    }

    public void SwitchMusicTrack(AudioClip track)
    {
        audioSource.Stop();
        StartCoroutine(HandleSwitchMusicTrackDelay(false, track));
    }

    private IEnumerator HandleSwitchMusicTrackDelay(bool isLevelTrack, AudioClip track = null)
    {

        yield return new WaitForSeconds(1.0f);

        if(isLevelTrack)
        {
            HandleMusic();
        }
        else
        {
            audioSource.clip = track;
            audioSource.Play();
        }
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
        StartCoroutine(HandleSwitchMusicTrackDelay(true));
    }

    public void LoadScene(string sceneName, Vector2 positionAfterLoad)
    {
        PlayerInfo.instance.healthPotionCount = 0;
        PlayerInfo.instance.nextPlayerPositionOnLoad = positionAfterLoad;
        SceneManager.LoadScene(sceneName);
        StartCoroutine(HandleSwitchMusicTrackDelay(true));
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        StartCoroutine(HandleSwitchMusicTrackDelay(true));
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

    public void ResetProgress()
    {
        PlayerPrefs.DeleteAll();
    }

    public void SaveProgress(Vector2 position)
    {
        Debug.Log("Saving!");

        PlayerPrefs.SetString("LastScene", SceneManager.GetActiveScene().name);
        PlayerPrefs.SetFloat("LastPositionX", position.x);
        PlayerPrefs.SetFloat("LastPositionY", position.y);
        PlayerPrefs.Save();

        Debug.Log($"X Pos Save: {PlayerInfo.instance.playerPosition.x}. Y Pos Save: {PlayerInfo.instance.playerPosition.y}");

        bool isUIPresent = GameObject.Find("PlayerUICanvas").TryGetComponent<PlayerUI>(out PlayerUI playerUI);

        if(isUIPresent)
        {
            playerUI.DisplayProgressSavedMessage();
        }
    }

    public void LoadProgress()
    {
        string lastSceneName = PlayerPrefs.GetString("LastScene", SceneManager.GetActiveScene().name);
        float lastPositionX = PlayerPrefs.GetFloat("LastPositionX", PlayerInfo.instance.playerPosition.x);
        float lastPositionY = PlayerPrefs.GetFloat("LastPositionY", PlayerInfo.instance.playerPosition.y);

        Debug.Log($"X Pos Load: {lastPositionX}. Y Pos Load: {lastPositionY}");

        PlayerInfo.instance.nextPlayerPositionOnLoad = new Vector2(lastPositionX, lastPositionY);
        PlayerInfo.instance.healthPotionCount = 0;
        PlayerInfo.instance.health = PlayerInfo.instance.fullHealth;

        Debug.Log($"Loading: {PlayerInfo.instance.nextPlayerPositionOnLoad.y}");

        Debug.Log("Loading progrss");
        
        SceneManager.LoadScene(lastSceneName);
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
