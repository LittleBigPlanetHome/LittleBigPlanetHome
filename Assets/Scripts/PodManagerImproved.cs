using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;
using System;
using PopitStuff.Enums;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PodManagerImproved : MonoBehaviour
{
    public PlayerManager Player;
    public Camera PodCamera, PlanetCamera;
    public bool ControlEnabled = false, RotatingAroundPlanet = false;

    [Space(10f)]

    public List<PodCameraDestination> Destinations = new List<PodCameraDestination>();
    public List<PodWorldUIElement> UI = new List<PodWorldUIElement>();
    public List<PodAudioPlayer> Audios = new List<PodAudioPlayer>();
    List<PodAudioPlayer> CurrentPlayers = new List<PodAudioPlayer>();
    List<PodCameraDestination> PreviousDestinations = new List<PodCameraDestination>();
    PodCameraDestination DestTo;
    PodAudioPlayer Back, PodEnter, PodExit;

    [Space(20f)]
    public List<Vector3> PodCameraPos = new List<Vector3>();
    [Space(20f)]
    public string GameMusicPreference = "LBP2";

    public static float TimeForUiTransition = 0.6f, TimeForCameraMove = 0.6f;

    //added pod controller to change in debug settings -vzp

    private GameObject podController;
    public GameObject PodMesh;

    private void Awake()
    {
        podController = base.gameObject.transform.Find("podcontroller").gameObject;
    }

    public void PodControllerPS3()
    {
        podController.transform.GetChild(0).gameObject.SetActive(true);
        podController.transform.GetChild(1).gameObject.SetActive(false);
    }

    public void PodControllerPS4()
    {
        podController.transform.GetChild(0).gameObject.SetActive(false);
        podController.transform.GetChild(1).gameObject.SetActive(true);
    }

    public PodCameraDestination CurrentDestination
    {
        get
        {
            return PreviousDestinations[0] ?? GetDestination("DEST_None");
        }
    }

    public PodCameraDestination GetDestination(string devId)
    {
        return Destinations.FirstOrDefault(d => d.DevId == devId);
    }

    public PodWorldUIElement GetUI(string devId)
    {
        return UI.FirstOrDefault(d => d.DevId == devId);
    }

    public PodAudioPlayer GetAudioPlayer(string devId)
    {
        return Audios.FirstOrDefault(a => a.DevId.ToLower() == devId.ToLower());
    }

    public PodAudioPlayer LoadOneShotClip(string devId)
    {
        PodAudioPlayer audio = GetAudioPlayer(devId);
        if (audio != null && audio.Source == null)
        {
            audio.Source = PlanetCamera.gameObject.AddComponent<AudioSource>();
            audio.Source.clip = audio.Clip;
            if (devId == "SND_PodExit")
                audio.Source.volume = 0.3f;
            else
                audio.Source.volume = 0.85f;
        }
        return audio;
    }

    public PodAudioPlayer PreloadAudioPlayer(string devId, bool muted = true)
    {
        PodAudioPlayer audio = GetAudioPlayer($"MU_{GameMusicPreference}_{devId}");
        if (audio != null && audio.Source == null)
        {
            audio.Source = PlanetCamera.gameObject.AddComponent<AudioSource>();
            audio.Source.clip = audio.Clip;
            if (!muted)
                audio.Source.volume = 0.7f;
            else
                audio.Source.volume = 0f;
            audio.Source.loop = true;
        }

        if (audio != null && audio.Source.isPlaying)
        {
            audio.Source.Pause();
            audio.isPaused = true;
        }
        else if (audio != null && audio.isPaused)
        {
            audio.Source.UnPause();
            audio.isPaused = false;
        }
        else if (audio != null && !audio.isPaused && !audio.Source.isPlaying)
        {
            audio.Source.Play();
        }
        else
        {
            Debug.LogWarning($"Failed to load audio clip for {devId}");
        }

        return audio;
    }

    public void FocusPlanetRotation(GameObject Planet, GameObject Target)
    {
        // TODO: Make planet rotate based on target position on planet.
        RotatingAroundPlanet = true;
        PlanetCamera.transform.DOMove(new Vector3(Planet.transform.position.x, Planet.transform.position.y, Planet.transform.position.z - 15), TimeForCameraMove);
        PlanetCamera.transform.DORotate(new Vector3(0, 0, 0), TimeForCameraMove);

        if (Target == null && Planet.name == "Earth")
        {
            Target = Planet.transform.Find("Items").Find("ProfilePolaroid").gameObject;
        }

        Planet.transform.eulerAngles = Vector3.zero;
        Planet.transform.DORotate(new Vector3(0, 180, 0), TimeForCameraMove * 1.2f, RotateMode.FastBeyond360).SetEase(Ease.OutElastic);
    }

    public void SetCameraToState(string devId)
    {
        SetCameraToState(devId, true);
    }

    public void SetCameraToState(string devId, bool countAsStep)
    {
        if (!ControlEnabled)
            EnablePodControl();

        if (!countAsStep && PreviousDestinations.Count != 0)
        {
            //play press O animation
            Player.anim.SetTrigger("CPressO");
        }
        else if (countAsStep && PreviousDestinations.Count != 0)
        {
            //play press X animation
            Player.anim.SetTrigger("CPressX");
        }

        print("Setting camera to new state.");
        var dest = GetDestination(devId);
        var ui = GetUI(devId);

        if (dest == null)
            return;

        PodAudioPlayer audio = null;
        if (dest.PlaySongID != "")
        {
            audio = GetAudioPlayer($"MU_{GameMusicPreference}_{dest.PlaySongID}");
        }

        if (dest.RequiresServerConnection && !ServerManager.Instance.connectedToServer)
        {
            Popup.Instance.OpenPopup("This feature requires being connected to the LBPH servers.", "Connect", () => {
                ServerManager.Instance.ConnectToServer(true, () => {
                    print("connected");
                    SetCameraToState(devId, countAsStep);
                }, () => {
                    print("error! Unable to connect");
                });
            });
            return;
        }

        RotatingAroundPlanet = false;

        if (ui == null)
            ui = GetUI("DummyUI");

        foreach (var uis in UI)
        {
            if (uis.DevId == "DEST_None") continue;

            if (uis.DevId != ui.DevId && uis.Group && uis.Group.alpha > 0)
            {
                uis.Group.DOFade(0f, TimeForUiTransition);
                uis.Group.interactable = false;
                uis.Group.blocksRaycasts = false;
            }
        }

        if (!dest.PlanetRotation)
        {
            PlanetCamera.transform.DOMove(dest.Position, TimeForCameraMove);
            PlanetCamera.transform.DORotate(dest.RotationEA, TimeForCameraMove);
        }
        else
        {
            FocusPlanetRotation(dest.PlanetToRotateAround, null);
        }

        if (countAsStep)
            PreviousDestinations.Add(dest);

        var bup = CurrentPlayers.ToList();
        foreach (var audios in bup)
        {
            if (!audios.DevId.EndsWith("Idle"))
            {
                audios.Source.DOFade(0, TimeForUiTransition);
                CurrentPlayers.Remove(audios);
            }
        }

        if (audio != null && audio.Source == null)
        {
            audio.Source = PlanetCamera.gameObject.AddComponent<AudioSource>();
            audio.Source.clip = audio.Clip;
            if (!audio.DevId.EndsWith("Idle"))
                audio.Source.volume = 0;
            else
                audio.Source.volume = 0.7f;
            if (audio != null && audio.Source.isPlaying)
            {
                audio.Source.Pause();
                audio.isPaused = true;
            }
            else if (audio != null && audio.isPaused)
            {
                audio.Source.UnPause();
                audio.isPaused = false;
            }
            else if (audio != null && !audio.isPaused && !audio.Source.isPlaying)
            {
                audio.Source.Play();
            }
            else
            {
                Debug.LogWarning($"Failed to load audio clip for {devId}");
            }
            audio.Source.loop = true;
        }

        if (audio != null)
        {
            audio.Source.DOFade(0.7f, TimeForUiTransition);
            CurrentPlayers.Add(audio);
        }

        if (ui.Group != null)
        {
            ui.Group.interactable = true;
            ui.Group.blocksRaycasts = true;
            if (Gamepad.current != null)
                EventSystem.current.SetSelectedGameObject(ui.Group.transform.GetChild(0).gameObject);
        }

        if (ui.Group != null)
            ui.Group.DOFade(1f, TimeForUiTransition);
    }

#if UNITY_EDITOR
    [MenuItem("Pod Creation Tools/Create Destination from Current Camera Position")]
    public static void CreateNewDestination()
    {
        var inst = FindObjectOfType<PodManagerImproved>();
        var dest = new PodCameraDestination();

        //SceneView.lastActiveSceneView
        dest.FamiliarName = $"Destination #{inst.Destinations.Count + 1}";
        dest.DevId = $"DEST_Destination{inst.Destinations.Count + 1}";
        dest.Position = SceneView.lastActiveSceneView.camera.gameObject.transform.position;
        dest.RotationEA = SceneView.lastActiveSceneView.camera.gameObject.transform.eulerAngles;

        inst.Destinations.Add(dest);
    }
#endif

    public void EnablePodControl()
    {
        //to-do this is scuff, we need da cross dissolve shader better ofc -vzp
        if (Popit.i.currentState != PopitState.Closed)
        {
            Popit.i.ClosePopit();
        }
        StartCoroutine("PodTransitionState", true);
        if (ServerManager.Instance.connectedToServer && !ServerManager.Instance.checkedAnnouncements)
            ServerManager.Instance.CheckAnnouncements(true);

        PreviousDestinations.Clear();
        if (Gamepad.current == null)
            EventSystem.current.SetSelectedGameObject(null);
        ControlEnabled = true;
        SetCameraToState("DEST_PodIdle");
        PodCamera.transform.DOMove(PodCameraPos[1], TimeForCameraMove);
        Player.canControl = false;
        PodEnter.Source.Play();
        PreloadAudioPlayer("Earth", true);
        PreloadAudioPlayer("Moon", true);
    }

    public void DisablePodControl()
    {
        //to-do this is scuff, we need da cross dissolve shader better ofc -vzp
        StartCoroutine("PodTransitionState", false);
        PreviousDestinations.Clear();
        if (Gamepad.current == null)
            EventSystem.current.SetSelectedGameObject(null);
        SetCameraToState("DEST_None", false);
        StartCoroutine("WaitForDisableControl");
        PodCamera.transform.DOMove(PodCameraPos[0], TimeForCameraMove);
        Player.canControl = true;
        PodExit.Source.Play();
        Player.ExitPodStateAnim();
        foreach (var audio in Audios)
        {
            if (audio.DevId == "SND_Back" || audio.DevId == "SND_PodEnter" || audio.DevId == "SND_PodExit") continue;
            Destroy(audio.Source);
            print($"Destroyed source of {audio.FamiliarName}.");
        }
    }

    IEnumerator PodTransitionState(bool enabled)
    {
        if (enabled)
        {
            foreach (Material mat in PodMesh.GetComponent<Renderer>().materials)
            {
                mat.DOFloat(1, "_DissolveAlpha", 1);
            }
            yield return new WaitForSeconds(0.4f);
            PodMesh.transform.GetChild(0).GetChild(0).GetComponent<Renderer>().enabled = true;
        }
        else
        {
            foreach (Material mat in PodMesh.GetComponent<Renderer>().materials)
            {
                mat.DOFloat(0, "_DissolveAlpha", 1);
            }
            yield return new WaitForSeconds(0.25f);
            PodMesh.transform.GetChild(0).GetChild(0).GetComponent<Renderer>().enabled = false;
        }
    }

    IEnumerator WaitForDisableControl()
    {
        yield return new WaitForSeconds(0.1f);
        ControlEnabled = false;
    }

    public void RepaintNewsData(Announcement announcement)
    {
        var dest = GetUI("DEST_RecentNews");
        var tr = dest.Group.transform;

        var parsedDate = DateTimeOffset.FromUnixTimeSeconds(announcement.date).ToLocalTime();

        tr.GetChild(0).GetComponent<TextMeshProUGUI>().text = announcement.title;
        tr.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = $"<#969696>Published on {(announcement.date == 0 ? "-" : parsedDate.ToString("dd.MM.yyyy"))}</color> | <sprite=1> {announcement.creator}";
        tr.GetChild(1).GetComponent<TextMeshProUGUI>().text = announcement.description.Replace("\\n", Environment.NewLine).Replace("{ps}", "\u2022");
    }

    public void GetNews()
    {
        if (!ServerManager.Instance.checkedAnnouncements)
        {
            ServerManager.Instance.CheckAnnouncements(false);
            Popup.Instance.OpenPopup("The news are still being downloaded. Please wait.");
            RepaintNewsData(new Announcement
            {
                enabled = true,
                creator = "-",
                title = "No new announcements!",
                description = "There's no announcements available right now. Please try again or check back later.",
                date = 0
            });
            return;
        }
        var anns = ServerManager.Instance.cachedAnnouncements.announcements.Where(an => an.enabled == true).ToArray();
        RepaintNewsData(anns.First());
    }

    void Start()
    {
        Player = FindObjectOfType<PlayerManager>();

        Back = LoadOneShotClip("SND_Back");
        PodEnter = LoadOneShotClip("SND_PodEnter");
        PodExit = LoadOneShotClip("SND_PodExit");
    }

    void Update()
    {
        if (Player.controls.Gameplay.Back.triggered && ControlEnabled && !Popup.Instance.IsOpen && !LoadingScreen.IsOpen)
        {
            // move back one step
            PodCameraDestination lastMove;
            try
            {
                lastMove = PreviousDestinations[PreviousDestinations.Count - 2];
            }
            catch
            {
                DisablePodControl();
                return;
            }

            SetCameraToState(lastMove.DevId, false);
            PreviousDestinations.Remove(PreviousDestinations.Last());
            print("going back");
            Back.Source.Play();
        }
    }
}

[System.Serializable]
public class PodCameraDestination
{
    public string FamiliarName = "Some Destination";
    public string DevId = "DEST_SomeDestination";
    public string PlaySongID = "";
    public bool PlanetRotation = false;
    public bool RequiresServerConnection = false;
    public GameObject PlanetToRotateAround = null;
    public Vector3 Position;
    public Vector3 RotationEA;
}

[System.Serializable]
public class PodAudioPlayer
{
    public string FamiliarName = "Some Song (LBPH)";
    public string DevId = "MU_LBPH_SomeSong";
    [HideInInspector] public AudioSource Source;
    public AudioClip Clip;
    public bool isPaused = false;
}

[System.Serializable]
public class PodWorldUIElement
{
    public string FamiliarName = "UI of Some Destination";
    public string DevId = "DEST_SomeDestination";
    public CanvasGroup Group;
}