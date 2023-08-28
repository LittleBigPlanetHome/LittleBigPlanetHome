using UnityEngine;
using System;
using System.IO;
using DG.Tweening;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager i { get; private set; }
    public SettingsData currentSettings;
    public CanvasGroup pauseMenu, settingsMenu;
    [SerializeField] AudioSource camAudiosource;
    public AudioClip pauseClip;

    float timeForUIDisappear = 0.6f;

    private void Awake() {
        if (i == null) {
            i = this;
        } else {
            Destroy(gameObject);
        }

        currentSettings = new SettingsData();
    }

    public void LoadSettings(Action<int> cb = null) {
        if (!File.Exists(Application.persistentDataPath + "/config.json")) {
            print("No config detected. Creating...");
            File.WriteAllText(Application.persistentDataPath + "/config.json", JsonUtility.ToJson(currentSettings, true));
            cb(1); // 1 = Created new file.
            return;
        }

        var file = File.ReadAllText(Application.persistentDataPath + "/config.json");
        currentSettings = JsonUtility.FromJson<SettingsData>(file);

        Screen.fullScreen = currentSettings.fullscreen;

        cb(0); // 0 = Loaded successfully.
    }

    public void SaveSettings(Action<int> cb = null) {
        if (!File.Exists(Application.persistentDataPath + "/config.json")) {
            print("No config detected. Creating...");
            File.WriteAllText(Application.persistentDataPath + "/config.json", JsonUtility.ToJson(currentSettings, true));
            cb(1); // 1 = Created new file.
            return;
        }

        var data = JsonUtility.ToJson(currentSettings, true);
        File.WriteAllText(Application.persistentDataPath + "/config.json", data);

        cb(1); // 1 = Created new file.
    }

    public void ToggleFullscreen() {
        Screen.fullScreen = !Screen.fullScreen;
        currentSettings.fullscreen = Screen.fullScreen;
    }

    public void ShowPauseMenu() {
        //settingsMenu.DOFade(1, timeForUIDisappear);
        if (camAudiosource != null) camAudiosource.PlayOneShot(pauseClip);
        FindObjectOfType<PlayerManager>().canControl = false;
        pauseMenu.DOFade(1, timeForUIDisappear);
        pauseMenu.interactable = true;
        pauseMenu.blocksRaycasts = true;
    }

    public void HidePauseMenu() {
        settingsMenu.DOFade(0, timeForUIDisappear);
        FindObjectOfType<PlayerManager>().canControl = true;
        pauseMenu.DOFade(0, timeForUIDisappear);
        pauseMenu.interactable = false;
        pauseMenu.blocksRaycasts = false;
    }

    public void ShowSettings() {
        settingsMenu.DOFade(1, timeForUIDisappear);
        settingsMenu.interactable = true;
        settingsMenu.blocksRaycasts = true;
    }

    public void HideSettings() {
        settingsMenu.DOFade(0, timeForUIDisappear);
        settingsMenu.interactable = false;
        settingsMenu.blocksRaycasts = false;
    }

    public void ChangeMusicID(string musicID)
    {
        FindObjectOfType<PodManagerImproved>().GameMusicPreference = musicID;
    }
}

[Serializable]
public class SettingsData
{
    public bool fullscreen = true;
}