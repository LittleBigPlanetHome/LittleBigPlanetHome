using System;
using System.Collections.Generic;
using Discord;
using UnityEngine;

public class Discord_Controller : MonoBehaviour
{
    const long ApplicationID = 1061821204232667187L;
    /*[Space]
    public string details = "Development Build - Private Beta";
    public string state = "In Pod";
    [Space]
    public string largeImage = "croppedlbph_icon";
    public string smallImage = "popiticon_characters";
    public string smallText = "Pod";
    public string largeText = "LittleBigPlanet Home";*/
    private long StartTime;
    public List<RPCStatus> StatusList = new();

    static bool InstanceExists;
    Discord.Discord Client;

    void Awake() 
    {
        // Transition the GameObject between scenes, destroy any duplicates
        if (!InstanceExists)
        {
            InstanceExists = true;
            DontDestroyOnLoad(gameObject);
        }
        else if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Client = new Discord.Discord(ApplicationID, (UInt64)CreateFlags.NoRequireDiscord);
        StartTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        UpdateStatus("PodIdle");
    }

    void Update()
    {
        try
        {
            Client.RunCallbacks();
        }
        catch
        {
            Client.Dispose();
            Destroy(gameObject);
        }
    }

    public void UpdateStatus(string ActivityID)
    {
        var State = StatusList.Find((S) => S.ActivityID == ActivityID);
        if (State != null)
            UpdateStatus(State);
    }

    public void UpdateStatus(RPCStatus ActivityObject)
    {
        try
        {
            var activityManager = Client.GetActivityManager();
            var activity = new Activity
            {
                Details = ActivityObject.Details,
                State = ActivityObject.State,

                Assets = 
                {
                    LargeImage = RPCStatus.LargeImage,
                    SmallImage = ActivityObject.SmallImage,
                    LargeText = RPCStatus.LargeText,
                    SmallText = ActivityObject.SmallText,
                },
                Timestamps =
                {
                    Start = StartTime
                }
            };

            activityManager.UpdateActivity(activity, (res) => { if (res != Result.Ok) Debug.LogWarning("Failed connecting to Discord!"); });
        }
        catch
        {
            // If updating the status fails, Destroy the GameObject
            Client.Dispose();
            Destroy(gameObject);
        }
    }

    private void OnApplicationQuit()
    {
        Client.Dispose();
    }
}

[CreateAssetMenu(menuName = "Discord/RPC Status", fileName = "RPC Status")]
public class RPCStatus : ScriptableObject
{
    public const string LargeImage = "croppedlbph_icon";
    public static string LargeText = $"LBP Home | r{Application.version} - Private Beta";

    public string ActivityID = "Some Activity";
    public string Details = "Details Ahaha!";
    public string State = "No way? State!";
    [Space]
    public string SmallImage = "Ah!";
    public string SmallText = "LOL!";

    public RPCStatus(string details, string state, string smallImage, string smallText)
    {
        Details = details;
        State = state;
        SmallImage = smallImage;
        SmallText = smallText;
    }
}