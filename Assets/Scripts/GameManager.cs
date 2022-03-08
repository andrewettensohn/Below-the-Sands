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

    public Milestones milestones = new Milestones { IsFistVisitToCatacomb = true };

    public Dictionary<HealthPotionName, bool> healthPotionAvailbility = CollectableDictionaryHelper.GetCollectableDictionaryForEnum<HealthPotionName>();

    public Dictionary<RelicName, bool> relicAvailbility = CollectableDictionaryHelper.GetCollectableDictionaryForEnum<RelicName>();

    public Dictionary<PrayerName, bool> prayerAvailbility = CollectableDictionaryHelper.GetCollectableDictionaryForEnum<PrayerName>();

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
