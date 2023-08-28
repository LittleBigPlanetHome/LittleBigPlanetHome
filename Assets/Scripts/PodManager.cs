using UnityEngine;
using DG.Tweening;
using System.Collections;
using UnityEngine.UI;
using UnityEditor;
using System.Linq;
using TMPro;
using System;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PodManager : MonoBehaviour
{
    public bool earthRotationDebug = false;
    public float timeForUiDisappear = 0.8f, timeForPlanetRootMove = 0.8f;
    public bool earthDataCheckingPopupEnabled = false;
    public CommunityProfile lookingAtProfile;
    bool meFirstTimeTextboxAppeared;
    [Space]
    public bool legacyMode = true;
    public AudioSource idleSource, earthSource, moonSource, podComputerEnter, podBack, podComputerLeave;
    public GameObject loadingScreen;
    public PlayerManager player;
    public Camera mainCamera;
    public AudioSource clickSource, moveSelectionSource;
    [Space]
    public CanvasGroup idleUi;
    public CanvasGroup earthCommunityUi, earthRecentNewsUi, meUi, profileDataUi, moreProfileDataUi, loginUi, moonDemoUi, storyPickerUi;
    public GameObject earthUi, moonUi, idleLegacyUi, moonCreationsUi, holdPins;
    int newsIndex = 0;
    public Outline theEarth, theMoon;
    public GameObject pod, earth, moon;
    public Material podMaterial;
    public bool theEnabled = false, planetsActive = true;
    public Vector2 planetRotationOffset = new Vector2(0.45f, 0.45f);
    [Space]
    public Transform planetRoot;
    public Vector3 camIdlePosition = new Vector3(0, 9.5f, -1.25f);
    public Vector3 camIdlePosition1 = new Vector3(0, 9.5f, -1.25f);
    public Vector3 camDefaultPos;
    public Vector3 camDefaultPos1;
    public Vector3 camDefaultEulerAngles;
    public Vector3 camDefaultEulerAngles1;
    public Vector3 planetRootOnActivate;
    Vector3 planetRootDefaultPos, planetRootEA;
    public Vector3 planetRootStoryDefaultPos, planetRootStoryDefaultEA;

    [Space]
    public Vector3 camZoomOnEarthEA = new Vector3(35, 160, -5.2f);
    public Vector3 camZoomOnEarthPos = new Vector3(-7.5f, 10.2f, 8.62f);
    public Vector3 camZoomOnMoonEA = new Vector3(0, 35, 0);
    public Vector3 camZoomOnMoonPos = new Vector3(-7.5f, 10.2f, 8.62f);
    public Vector3 camZoomOnEarthCommunityEA = new Vector3(0, 90, 0);
    public Vector3 camZoomOnEarthCommunityPos = new Vector3(-12.9f, 9.1f, 17.5f);
    public Vector3 camZoomOnEarthRecentNewsPos = new Vector3(-12.9f, 6, 17.5f);
    [Space(30f)]
    public Vector3 planetRootZoomOnMeTab;
    public Vector3 planetRootZoomOnMeTabEA;
    public Vector3 planetRootEarthZoom = new Vector3(5.1f, -1.2f, -25f), planetRootEarthZoomEA = new Vector3(55, 175, 0);
    public Vector3 planetRootMoreProfileZoom = new Vector3(1.2f, -1.2f, -29f), planetEarthMoreProfileEA = new Vector3(55f, 135f, 0f);
    public Vector3 planetRootMoonZoomDemo = new Vector3(-5.7f, -1.4f, -38.5f), planetMoonZoomDemo = new Vector3(355, 70, 325);
    public Vector3 planetRootStoryLBPPos = new Vector3(-14.75f, -9.5f, -50f), planetRootStoryLBPEulerAngles = new Vector3(0, 45, 0);

    private void Start() {
        planetRootDefaultPos = planetRoot.position;
        planetRootEA = planetRoot.eulerAngles;
        //camDefaultPos = mainCamera.transform.position;
        //camDefaultEulerAngles = mainCamera.transform.eulerAngles;
        podMaterial = pod.GetComponent<Renderer>().material;
        player = FindObjectOfType<PlayerManager>();
        if (loadingScreen != null)
            loadingScreen.GetComponent<CanvasGroup>().alpha = 1f;

        /*var tweener = mainCamera.DOShakePosition(10f, 0.2f, 2, 30);
        tweener.SetLoops(-1);*/
    }

    [ContextMenu("Enable Pod Control")]
    public void EnablePodControl() {
        return;
        /*print("Enabled pod control!");
        theEnabled = true;
        FindObjectOfType<PlayerManager>().canControl = false;
        idleSource.Play();
        earthSource.Play();
        moonSource.Play();

        if (!ServerManager.instance.checkedAnnouncements)
            ServerManager.instance.CheckAnnouncements(true);

        moonSource.volume = 0;
        idleSource.volume = 1;
        earthSource.volume = 0;

        if (idleUi && !legacyMode) {
            idleUi.transform.GetChild(0).GetComponent<LockableButton>().Select();
        }

        podComputerEnter.Play();

        if (legacyMode)
            planetsActive = true;

        GameObject.Find("Pod Camera").transform.DOMove(camIdlePosition1, timeForPlanetRootMove);
        mainCamera.transform.DOMove(camIdlePosition, timeForPlanetRootMove);
        ChangePodPosition(planetRootOnActivate, Vector3.zero, idleUi);

        podMaterial.DOFloat(6.5f, "_CutoffHeight", 0.4f);

        if (legacyMode) {
            StartCoroutine(LerpOutline(theEarth, 5, 1.2f));
            StartCoroutine(LerpOutline(theMoon, 5, 1.2f));
        }*/
    }

    [ContextMenu("Disable Pod Control")]
    public void DisablePodControl() {
        return;
        /*print("Disabled pod control!");
        theEnabled = false;
        FindObjectOfType<PlayerManager>().canControl = true;
        idleSource.Stop();
        earthSource.Stop();
        moonSource.Stop();

        moonSource.volume = 0;
        idleSource.volume = 0;
        earthSource.volume = 0;

        EventSystem.current.SetSelectedGameObject(null);

        podComputerLeave.Play();

        GameObject.Find("PodCamera").transform.DOMove(camDefaultPos1, 0.9f);
        //GameObject.Find("PodCamera").transform.DORotate(camDefaultEulerAngles1, 0.9f);
        mainCamera.transform.DOMove(camDefaultPos, 0.9f);
        mainCamera.transform.DORotate(camDefaultEulerAngles, 0.9f);

        if (earthUi)
            earthUi.SetActive(false);
        if (moonUi)
            moonUi.SetActive(false);

        ChangePodPosition(planetRootDefaultPos, planetRootEA, null, idleUi);
        podMaterial.DOFloat(20f, "_CutoffHeight", 0.4f);

        if (legacyMode) {
            StartCoroutine(LerpOutline(theEarth, 0, 1.2f));
            StartCoroutine(LerpOutline(theMoon, 0, 1.2f));
        }*/
    }

    [ContextMenu("Debug Earth Rotation")]
    private void DEBUG_UpdatePlanetRotEarthProfile() {
        UpdatePlanetRotation(GameObject.Find("ProfileObjectEarth").transform, earth.transform);
    }

    [ContextMenu("Debug Earth Rotation Other")]
    private void DEBUG_UpdatePlanetRotEarthProfile2() {
        UpdatePlanetRotation(GameObject.Find("OtherProfileEarth").transform, earth.transform);
    }

    public void UpdatePlanetRotation(Transform level, Transform planet, bool includeDelay = true) {
        StartCoroutine(DEBUG_UpdPlanetRot(level, planet, includeDelay));
    }

    IEnumerator DEBUG_UpdPlanetRot(Transform level, Transform planet, bool includeDelay) {
        if (includeDelay)
            yield return new WaitForSeconds(timeForPlanetRootMove);
        planet.DOLookAt(new Vector3(-level.localPosition.x, level.localPosition.y, level.localPosition.z) + new Vector3(planetRotationOffset.x, planetRotationOffset.y), timeForPlanetRootMove, AxisConstraint.None, new Vector3(0, 1, 0));
        yield break;
    }

    public void EnableMoonCreationsView() {
        /*
        var rotCamera = Camera.main.GetComponent<RotatingCamera>();
        moonUi.SetActive(false);
        rotCamera.target = theMoon.transform.Find("CENTER");
        rotCamera.cam.transform.DORotate(camDefaultEulerAngles, 0.5f);
        rotCamera.cam.transform.DOMove(rotCamera.target.position - new Vector3(0, 0, 10), 0.5f).OnComplete(() => {
            rotCamera.isEnabled = true;
            moonCreationsUi.SetActive(true);
        });

        Popup.mngr.OpenPopup("Please note, that the Moon is still under development, and levels might not save or save with bugs.", "Got it.");
        */
    }

    public void EnableMeView() {
        ChangePodPosition(planetRootZoomOnMeTab, planetRootZoomOnMeTabEA, meUi, idleUi);

        if (meFirstTimeTextboxAppeared)
            meUi.transform.GetChild(4).GetComponent<LockableButton>().Select();

        StartCoroutine(LerpVolume(earthSource, 0.15f, timeForPlanetRootMove));

        if (!meFirstTimeTextboxAppeared)
            Popup.Instance.OpenPopup("Welcome to your profile.\nHere, you can <#fc03e8>create and share levels with others!</color>\nBegin by selecting Your Moon.", "Let's go!", () => { meFirstTimeTextboxAppeared = true; meUi.transform.GetChild(4).GetComponent<LockableButton>().Select(); });
    }

    public void MoonDemoView() {
        ChangePodPosition(planetRootMoonZoomDemo, planetRootZoomOnMeTabEA, null, meUi, idleUi, earthCommunityUi, earthRecentNewsUi);

        if (Audios.Instance.type == MusicPodType.LBPVita) {
            StartCoroutine(LerpVolume(earthSource, 0f, timeForPlanetRootMove));
            StartCoroutine(LerpVolume(idleSource, 0f, timeForPlanetRootMove));
            StartCoroutine(LerpVolume(moonSource, 1f, timeForPlanetRootMove));
        } else if (Audios.Instance.ShouldPlayBackgroundTrack()) {
            StartCoroutine(LerpVolume(earthSource, 0f, timeForPlanetRootMove));
            StartCoroutine(LerpVolume(idleSource, 1f, timeForPlanetRootMove));
            StartCoroutine(LerpVolume(moonSource, 1f, timeForPlanetRootMove));
        } else {
            StartCoroutine(LerpVolume(earthSource, 0f, timeForPlanetRootMove));
            StartCoroutine(LerpVolume(idleSource, 0f, timeForPlanetRootMove));
            StartCoroutine(LerpVolume(moonSource, 1f, timeForPlanetRootMove));
        }

        moon.transform.DORotate(planetMoonZoomDemo, timeForPlanetRootMove);
    }

    public void StoryViewDefault() {
        ChangePodPosition(planetRootStoryDefaultPos, planetRootStoryDefaultEA, storyPickerUi, idleUi, earthCommunityUi);
    
        if (Audios.Instance.ShouldPlayBackgroundTrack()) {
            StartCoroutine(LerpVolume(earthSource, 1f, timeForPlanetRootMove));
            StartCoroutine(LerpVolume(idleSource, 1f, timeForPlanetRootMove));
            StartCoroutine(LerpVolume(moonSource, 0f, timeForPlanetRootMove));
        } else {
            StartCoroutine(LerpVolume(earthSource, 1f, timeForPlanetRootMove));
            StartCoroutine(LerpVolume(idleSource, 0f, timeForPlanetRootMove));
            StartCoroutine(LerpVolume(moonSource, 0f, timeForPlanetRootMove));
        }
    }

    public enum StoryPlanet {
        LittleBigPlanet = 0
    }

    public void StoryView(StoryPlanet planet = StoryPlanet.LittleBigPlanet) {
        if (Audios.Instance.ShouldPlayBackgroundTrack()) {
            StartCoroutine(LerpVolume(earthSource, 1f, timeForPlanetRootMove));
            StartCoroutine(LerpVolume(idleSource, 1f, timeForPlanetRootMove));
            StartCoroutine(LerpVolume(moonSource, 0f, timeForPlanetRootMove));
        } else {
            StartCoroutine(LerpVolume(earthSource, 1f, timeForPlanetRootMove));
            StartCoroutine(LerpVolume(idleSource, 0f, timeForPlanetRootMove));
            StartCoroutine(LerpVolume(moonSource, 0f, timeForPlanetRootMove));
        }

        if (planet == StoryPlanet.LittleBigPlanet) {
            ChangePodPosition(planetRootStoryLBPPos, planetRootStoryLBPEulerAngles, null);
        }
    }

    public void DisableMoonDemoView() {
        ChangePodPosition(planetRootZoomOnMeTab, planetRootZoomOnMeTabEA, meUi);
        
        StartCoroutine(LerpVolume(earthSource, 0.15f, timeForPlanetRootMove));
        StartCoroutine(LerpVolume(idleSource, 1f, timeForPlanetRootMove));
    }

    public void CommunityProfileView(CommunityProfile profile) {
        DEBUG_UpdatePlanetRotEarthProfile();
        ChangePodPosition(planetRootEarthZoom, Vector3.zero, profileDataUi, idleUi, meUi);

        profileDataUi.transform.Find("PersonName").GetComponent<TextMeshProUGUI>().text = profile.username;
        profileDataUi.transform.Find("UserDesc").GetComponent<TextMeshProUGUI>().text = profile.description;
        profileDataUi.transform.Find("Hearts").GetComponent<TextMeshProUGUI>().text = profile.likes.ToString();

        lookingAtProfile = profile;

        if (Audios.Instance.ShouldPlayBackgroundTrack()) {
            StartCoroutine(LerpVolume(idleSource, 1, timeForPlanetRootMove));
            StartCoroutine(LerpVolume(earthSource, 1, timeForPlanetRootMove));
        } else {
            StartCoroutine(LerpVolume(idleSource, 0, timeForPlanetRootMove));
            StartCoroutine(LerpVolume(earthSource, 1, timeForPlanetRootMove));
        }
    }

    public void ViewMoreProfileDetails() {
        if (lookingAtProfile == null) return;

        DEBUG_UpdatePlanetRotEarthProfile();
        ChangePodPosition(planetRootMoreProfileZoom, Vector3.zero, moreProfileDataUi, profileDataUi);

        var firstLvl = lookingAtProfile.allLevels.ElementAtOrDefault(0) == null ? new CommunityLevel() : lookingAtProfile.allLevels.ElementAtOrDefault(0);
        var secondLvl = lookingAtProfile.allLevels.ElementAtOrDefault(1) == null ? new CommunityLevel() : lookingAtProfile.allLevels.ElementAtOrDefault(1);
        var thirdLvl = lookingAtProfile.allLevels.ElementAtOrDefault(2) == null ? new CommunityLevel() : lookingAtProfile.allLevels.ElementAtOrDefault(2);

        var dataParent = moreProfileDataUi.transform;
        dataParent.Find("PersonName").GetComponent<TextMeshProUGUI>().text = lookingAtProfile.username;
        dataParent.Find("UserDescription").GetComponent<TextMeshProUGUI>().text = lookingAtProfile.description;
        dataParent.Find("Hearts").GetComponent<TextMeshProUGUI>().text = lookingAtProfile.likes.ToString();
        dataParent.Find("Newest Level").GetComponent<TextMeshProUGUI>().text = string.Concat(new [] {
            "<#fc03e8><sprite=2> Newest Levels:</color><align=center>\n",
            "<sprite=2> ",
            firstLvl.displayName,
            "\n<sprite=2> ",
            secondLvl.displayName,
            "\n<sprite=2> ",
            thirdLvl.displayName
        });
    }

    public void ChangePodPosition(Vector3 endPos, Vector3 endRot, CanvasGroup toTurnOn, params CanvasGroup[] toTurnOff) {
        planetRoot.DOMove(endPos, timeForPlanetRootMove);
        planetRoot.DORotate(endRot, timeForPlanetRootMove);

        if (toTurnOn != null)
            StartCoroutine(LerpAlpha(toTurnOn, 1f, timeForUiDisappear));
        foreach(CanvasGroup cg in toTurnOff) {
            StartCoroutine(LerpAlpha(cg, 0f, timeForUiDisappear));
        }
    }

    public void AskForAccountLogin() {
        Popup.Instance.OpenPopup("You will now be prompted to log into your account.\nIf you don't have an account, contact a developer.", "Gotcha!", () => AccountLoginFurther());
    }

    private void AccountLoginFurther() {
        StartCoroutine(LerpAlpha(loginUi, 1f, timeForUiDisappear));
    }

    public void ViewPersonalProfile() {
        CommunityProfileView(new CommunityProfile());
    }

    public void DisableMeView() {
        StartCoroutine(LerpAlpha(meUi, 0f, timeForUiDisappear));
        SelectPlanet("Idle");
    }

    public void EnableEarthCommunityView() {
        earthUi.SetActive(false);

        if (Audios.Instance.ShouldPlayBackgroundTrack()) {
            StartCoroutine(LerpVolume(moonSource, 0, timeForPlanetRootMove));
            StartCoroutine(LerpVolume(idleSource, 1, timeForPlanetRootMove));
            StartCoroutine(LerpVolume(earthSource, 1, timeForPlanetRootMove));
        } else {
            StartCoroutine(LerpVolume(moonSource, 0, timeForPlanetRootMove));
            StartCoroutine(LerpVolume(idleSource, 0, timeForPlanetRootMove));
            StartCoroutine(LerpVolume(earthSource, 1, timeForPlanetRootMove));
        }

        earthCommunityUi.transform.GetChild(2).GetComponent<LockableButton>().Select();

        ChangePodPosition(camZoomOnEarthCommunityPos, camZoomOnEarthCommunityEA, earthCommunityUi, idleUi);
    }

    public void EnableEarthRecentNewsView() {
        newsIndex = 0;
        
        var anns = ServerManager.Instance.cachedAnnouncements.announcements.Where(an => an.enabled == true).ToArray();
        if (anns.Length >= 2) {
            earthRecentNewsUi.transform.Find("Next").gameObject.SetActive(true);
        }

        if (anns.Length >= 1)
            RefreshEarthNews(0);

        ChangePodPosition(camZoomOnEarthRecentNewsPos, camZoomOnEarthCommunityEA, earthRecentNewsUi, earthCommunityUi);
    }

    public void NextEarthNews(bool next = true) {
        var anns = ServerManager.Instance.cachedAnnouncements.announcements.Where(an => an.enabled == true).ToArray();

        if (next)
            RefreshEarthNews(newsIndex + 1);
        else
            RefreshEarthNews(newsIndex - 1);

        print(anns.Length - 1 + " | idx: " + newsIndex);

        if (newsIndex + 1 >= anns.Length) {
            earthRecentNewsUi.transform.Find("Next").gameObject.SetActive(false);
        }
        else {
            earthRecentNewsUi.transform.Find("Next").gameObject.SetActive(true);
        }

        if (newsIndex >= 1) {
            earthRecentNewsUi.transform.Find("Previous").gameObject.SetActive(true);
        }
        else {
            earthRecentNewsUi.transform.Find("Previous").gameObject.SetActive(false);
        }
    }

    public void RefreshEarthNews(int index) {
        var anns = ServerManager.Instance.cachedAnnouncements.announcements.Where(an => an.enabled == true).ToArray();
        var ann = anns[index];
        
        var content = ann.description.Replace("\\n", Environment.NewLine);

        newsIndex = index;
        
        earthRecentNewsUi.transform.Find("AnnTitle").GetComponent<TextMeshProUGUI>().text = ann != null ? ann.title : "No announcements!";
        earthRecentNewsUi.transform.Find("AnnContent").GetComponent<TextMeshProUGUI>().text = ann != null ? content : "There aren't any announcements available right now.\nCheck back later.";
        earthRecentNewsUi.transform.Find("AnnTitle").Find("User").GetComponent<TextMeshProUGUI>().text = ann != null ? "<sprite=1> " + ann.creator : "<sprite=1> LittleBigPlanet Home";
    }

    public void DisableEarthRecentNewsView() {
        ChangePodPosition(camZoomOnEarthCommunityPos, camZoomOnEarthCommunityEA, earthCommunityUi, earthRecentNewsUi);
    }

    public void DisableEarthCommunityView() {
        SelectPlanet("Earth");
        StartCoroutine(LerpAlpha(earthCommunityUi, 0f, timeForUiDisappear));
    }

    public void DisableMoonCreationsView() {
        var rotCamera = Camera.main.GetComponent<RotatingCamera>();
        rotCamera.isEnabled = false;
        SelectPlanet("Moon");
        moonCreationsUi.SetActive(false);
    }

    public void SelectPlanet(string planetName) {
        switch (planetName) {
            case "Earth":
                if (Audios.Instance.ShouldPlayBackgroundTrack()) {
                    StartCoroutine(LerpVolume(moonSource, 0, timeForPlanetRootMove));
                    StartCoroutine(LerpVolume(idleSource, 1, timeForPlanetRootMove));
                    StartCoroutine(LerpVolume(earthSource, 1, timeForPlanetRootMove));
                } else {
                    StartCoroutine(LerpVolume(moonSource, 0, timeForPlanetRootMove));
                    StartCoroutine(LerpVolume(idleSource, 0, timeForPlanetRootMove));
                    StartCoroutine(LerpVolume(earthSource, 1, timeForPlanetRootMove));
                }

                if (moonUi)
                    moonUi.SetActive(false);
                if (idleLegacyUi)
                    idleLegacyUi.SetActive(false);
                if (idleUi)
                    StartCoroutine(LerpAlpha(idleUi, 0f, timeForUiDisappear));
                
                ChangePodPosition(camZoomOnEarthPos, Vector3.zero, idleUi);
                planetRoot.DORotate(camZoomOnEarthEA, timeForPlanetRootMove).OnComplete(() => {
                    if (earthUi)
                        earthUi.SetActive(true);
                });
                planetsActive = false;
                
                if (legacyMode) {
                    StartCoroutine(LerpOutline(theEarth, 0, 1.2f));
                    StartCoroutine(LerpOutline(theMoon, 0, 1.2f));
                }
                break;
            case "Moon":
                if (Audios.Instance.ShouldPlayBackgroundTrack()) {
                    StartCoroutine(LerpVolume(moonSource, 1, timeForPlanetRootMove));
                    StartCoroutine(LerpVolume(idleSource, 1, timeForPlanetRootMove));
                    StartCoroutine(LerpVolume(earthSource, 0, timeForPlanetRootMove));
                } else {
                    StartCoroutine(LerpVolume(moonSource, 1, timeForPlanetRootMove));
                    StartCoroutine(LerpVolume(idleSource, 0, timeForPlanetRootMove));
                    StartCoroutine(LerpVolume(earthSource, 0, timeForPlanetRootMove));
                }

                if (earthUi)
                    earthUi.SetActive(false);
                if (idleLegacyUi)
                    idleLegacyUi.SetActive(false);
                if (idleUi)
                    StartCoroutine(LerpAlpha(idleUi, 0f, timeForUiDisappear));

                ChangePodPosition(camZoomOnMoonPos, Vector3.zero, null);
                planetRoot.DORotate(camZoomOnMoonEA, timeForPlanetRootMove).OnComplete(() => {
                    if (moonUi)
                        moonUi.SetActive(true);
                });
                planetsActive = false;

                if (legacyMode) {
                    StartCoroutine(LerpOutline(theEarth, 0, 1.2f));
                    StartCoroutine(LerpOutline(theMoon, 0, 1.2f));
                }
                break;
            default:
                StartCoroutine(LerpVolume(moonSource, 0, timeForPlanetRootMove));
                StartCoroutine(LerpVolume(idleSource, 1, timeForPlanetRootMove));
                StartCoroutine(LerpVolume(earthSource, 0, timeForPlanetRootMove));

                if (earthUi)
                    earthUi.SetActive(false);
                if (moonUi)
                    moonUi.SetActive(false);

                if (idleUi && !legacyMode) {
                    StartCoroutine(LerpAlpha(idleUi, 1f, timeForUiDisappear));
                    idleUi.transform.GetChild(1).GetComponent<LockableButton>().Select();
                }
                if (earthCommunityUi && !legacyMode)
                    StartCoroutine(LerpAlpha(earthCommunityUi, 0f, timeForUiDisappear));

                ChangePodPosition(planetRootOnActivate, Vector3.zero, null);
                planetRoot.DORotate(camDefaultEulerAngles, timeForPlanetRootMove).OnComplete(() => {
                    if (idleLegacyUi && legacyMode)
                        idleLegacyUi.SetActive(true);
                });

                if (legacyMode)
                    planetsActive = true;

                if (legacyMode) {
                    StartCoroutine(LerpOutline(theEarth, 5, 1.2f));
                    StartCoroutine(LerpOutline(theMoon, 5, 1.2f));
                }
                break;
        }
    }

    public IEnumerator LerpOutline(Outline toLerp, float endValue, float time) {
        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            toLerp.OutlineWidth = Mathf.Lerp(toLerp.OutlineWidth, endValue, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }  

        toLerp.OutlineWidth = endValue;
    }

    public IEnumerator LerpAlpha(CanvasGroup toLerp, float endValue, float time) {
        float elapsedTime = 0;

        if (endValue <= 0.2f) {
            toLerp.interactable = false;
            toLerp.blocksRaycasts = false;
        }

        if (endValue > 0.2f) {
            toLerp.interactable = true;
            toLerp.blocksRaycasts = true;
        }

        while (elapsedTime < time)
        {
            toLerp.alpha = Mathf.Lerp(toLerp.alpha, endValue, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        toLerp.alpha = endValue;
    }

    public IEnumerator LerpVolume(AudioSource toLerp, float endValue, float time) {
        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            toLerp.volume = Mathf.Lerp(toLerp.volume, endValue, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }  

        toLerp.volume = endValue;
    }

    private void Update() {
        // debugging
        if (earthRotationDebug)
            DEBUG_UpdatePlanetRotEarthProfile();

        // return mechanics
        if (player.controls.Gameplay.Back.triggered && !player.canControl) {
            podBack.Play();

            if (meUi.interactable) {
                DisableMeView();
                return;
            }

            if (idleUi.interactable) {
                DisablePodControl();
                return;
            }

            if (moonDemoUi.interactable) {
                DisableMoonDemoView();
                return;
            }

            if (earthCommunityUi.interactable) {
                SelectPlanet("Idle");
                return;
            }

            if (earthRecentNewsUi.interactable) {
                DisableEarthRecentNewsView();
                return;
            }
        }
    }
}