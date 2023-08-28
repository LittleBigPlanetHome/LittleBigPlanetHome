using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using System.Linq;
using System.Collections.Generic;
using TMPro;

public class ServerManager : Singleton<ServerManager>
{
    public static string BaseURL = "https://lbpr2d-dev-global-repl-001.mcmistrzyt.repl.co";
#if UNITY_EDITOR
    [Tooltip("This override only works in the Editor. It does NOT carry over to built releases.")]
    public string OverrideBaseURL = ""; // only override in UNITY. built platforms use normal url.
#endif
    public CommunityProfile cachedCommunityProfile;
    public bool connectedToServer;
    public Server connectedServer;
    public UnityEvent onServerConnect, onServerDisconnect;
    public Announcements cachedAnnouncements;
    public bool checkedAnnouncements;
    public Color buttonsIdleColor, buttonsHoveredColor, buttonsClickedColor, buttonsDisabledColor, buttonsSelectedColor;

    public override void OnAwake() {
#if UNITY_EDITOR
        if (OverrideBaseURL != "")
            ServerManager.BaseURL = OverrideBaseURL;
#endif
    }

    public void PingServer() {
        StartCoroutine(GET("/ping", (uwr) => {
            if (uwr.result != UnityWebRequest.Result.Success) {
                onServerDisconnect?.Invoke();
            }
        }));
    }

    private void Start() {
        ConnectToServer(false, () => RefreshHotfixes(() => {
            LoadingScreen.Drop();
        }));

        onServerConnect.AddListener(() => {
            connectedToServer = true;
            InvokeRepeating("PingServer", 20, 20);
        });

        onServerDisconnect.AddListener(() => {
            var p = FindObjectOfType<PodManagerImproved>();
            if (p != null) {
                p.DisablePodControl();
            }

            CancelInvoke("PingServer");
            connectedToServer = false;
            connectedServer = null;
            Popup.Instance.OpenPopup("Failed to ping server!\nThis usually means you disconnected from the internet or the server has gone under maintenance.", "Retry", () => ServerManager.Instance.ConnectToServer(true));
        });
    }

    public void ConnectToServer(bool dropLoadingScreen, Action afterSuccessfulConnection = null, Action afterUnsuccessfulConnection = null) {
        LoadingScreen.Show("Connecting to LBPH Online...", "Hold onto your chair while we locate the nearest LBP Home game servers for you!");
        StartCoroutine(GET("/api/server", (uwr) => {
            Server response = null;
            try {
                print("Connecting!!");
                if (uwr.result != UnityWebRequest.Result.Success) throw new Exception("Web error");
                response = JsonUtility.FromJson<Server>(uwr.downloadHandler.text);
            } catch (Exception) {
                Popup.Instance.OpenPopup("The LittleBigPlanet: Home game servers are currently down.\nYou will enter Offline Mode, which limits certain functionality like the Community tab.", "OK", () => {
                    LoadingScreen.Drop();
                    if (afterUnsuccessfulConnection != null)
                        afterUnsuccessfulConnection();
                });
                return;
            }
            connectedServer = response;
            onServerConnect?.Invoke();
            if (afterSuccessfulConnection != null)
                afterSuccessfulConnection();

            LoadingScreen.Drop();
        }));
    }

    public void Login(string username, string password, Action<CommunityProfile> callback) {
        CommunityProfile profile = null;

        var data = new Dictionary<string, string>();
        data.Add("username", username);
        data.Add("password", password);

        StartCoroutine(POST("/api/user/login", data, (uwr) => {
            if (uwr.result != UnityWebRequest.Result.Success) return;
            profile = JsonUtility.FromJson<CommunityProfile>(uwr.downloadHandler.text);
        }));

        callback(profile);
    }

    public void GetLoadingTips(Action<List<string>> callback) {
        StartCoroutine(GET("/api/loading-tips", (uwr) => {
            if (uwr.result != UnityWebRequest.Result.Success) return;
            var response = JsonUtility.FromJson<LoadingTipsClass>(uwr.downloadHandler.text);
            callback(response.tips);
        }));
    }

    public void CheckAnnouncements(bool openPopup) {
        StartCoroutine(GET("/api/announcements", (uwb) => {
            var ann = JsonUtility.FromJson<Announcements>(uwb.downloadHandler.text);
            var allAncs = ann.announcements.Where(an => an.enabled == true).ToArray();
            if (allAncs.Length > 0 && openPopup) {
                Popup.Instance.OpenPopup($"There are <#fc03e8>{allAncs.Length}</color> news available.\nIf you want to read them, go to Earth -> Community -> Recent News.");
            }
            checkedAnnouncements = true;
            cachedAnnouncements = ann;
        }));
    }

    public void RefreshHotfixes(Action callback = null) {
        StartCoroutine(GET("/hotfix/Hotfix.ini", (uwr) => {
            if (uwr.result != UnityWebRequest.Result.Success) return;

            INIParser parser = new INIParser();
            parser.OpenFromString(uwr.downloadHandler.text);
            print("Hotfixing!");

            if (parser.IsSectionExists("HotfixPlayer")) {
                print("Player hotfix available!");
                if (parser.IsKeyExists("HotfixPlayer", "Speed"))
                    FindObjectOfType<PlayerManager>().speed = parser.ReadValue("HotfixPlayer", "Speed", 5f);
            }

            if (parser.IsSectionExists("HotfixStrings")) {
                print("String hotfix available!");
                foreach(GameString gs in StringManager.instance.strings) {
                    var index = StringManager.instance.GetGameStringIndex(gs);
                    if (parser.IsKeyExists("HotfixStrings", gs.name)) {
                        var hotfix = parser.ReadValue("HotfixStrings", gs.name, gs.value).Replace("{'", "").Replace("'}", "");
                        StringManager.instance.strings[index].value = hotfix;
                        print("Hotfixed " + gs.name + " to " + hotfix);

                        foreach(var dt in FindObjectsOfType<DynamicText>()) {
                            if (dt.ogValue == gs.name) {
                                dt.tmp.text = hotfix;
                            }
                        }
                    }
                }
            }
            parser.Close();
            callback();
        }));
    }

    public IEnumerator GET(string uri, Action<UnityWebRequest> callback) {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(BaseURL + uri))
        {
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError("Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError("HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log("Received: " + webRequest.downloadHandler.text);
                    break;
            }

            callback(webRequest);
        }
    }

    public IEnumerator POST(string uri, Dictionary<string, string> data, Action<UnityWebRequest> callback) {
        using (UnityWebRequest webRequest = UnityWebRequest.Post(BaseURL + uri, data))
        {
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError("Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError("HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log("Received: " + webRequest.downloadHandler.text);
                    break;
            }

            callback(webRequest);
        }
    }
}

[System.Serializable]
public class Server {
    public bool ServerOnline = false;
    public string ServerId;
    public string ServerLastStarted;
}

[System.Serializable]
public class Announcements {
    public List<Announcement> announcements = new List<Announcement>();
}

[System.Serializable]
public class LoadingTipsClass {
    public List<string> tips = new List<string>();
}

[System.Serializable]
public class Announcement {
    public bool enabled = true;
    public string creator, title, description;
    public int date;
}

[System.Serializable]
public class CommunityProfile {
    public string username = "You";
    [TextArea(3, 10)]
    public string description = "You didn't log in yet.\nRegister your account in the Community tab.";
    public string password = "";
    public string imageURL;
    public CommunityPin[] pins = new CommunityPin[3] {
        null,
        null,
        null
    };
    public List<CommunityLevel> allLevels = new List<CommunityLevel>();
    public int likes;

    public CommunityProfile() {
        if (ServerManager.Instance != null) {
            if (ServerManager.Instance.cachedCommunityProfile == null) return;
            var prfl = ServerManager.Instance.cachedCommunityProfile;
            this.username = prfl.username;
            this.description = prfl.description;
            this.password = prfl.password;
            this.imageURL = prfl.imageURL;
            this.pins = prfl.pins;
            this.likes = prfl.likes;
            this.allLevels = prfl.allLevels;
        }
    }
}

[System.Serializable]
public class CommunityPin {
    public string displayNameId;
    public string displayDescriptionId;
    public Sprite displayPicture;
}

[System.Serializable]
public class CommunityPolaroid {
    public int id = -1;
    public string displayName = "Polaroid";
    public string displayDescription = "debugging purposes only";
    public Sprite displayPicture;
    public string rawLvlData;
    public string dateOfUpload = "-";
}

[System.Serializable]
public class CommunityLevel {
    public int id = -1;
    public string displayName = "-----";
    public string displayDescription = "debugging purposes only";
    public Sprite displayPicture;
    public string rawLvlData;
    public string dateOfUpload = "-";
}