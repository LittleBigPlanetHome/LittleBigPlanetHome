using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class LoadingScreen : Singleton<LoadingScreen>
{
    public override bool DontDestroy()
    {
        return false;
    }

    public static bool IsOpen {
        get {
            return Instance != null ? Instance.Group.blocksRaycasts : false;
        }
    }
    public List<string> RandomMainText = new List<string>();
    public List<string> RandomTipText = new List<string>();

    TextMeshProUGUI Main, Tip;
    CanvasGroup Group;

    public override void OnAwake()
    {
        Main = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        Tip = transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        Group = GetComponent<CanvasGroup>();

        Group.alpha = 0;
        Group.interactable = false;
        Group.blocksRaycasts = false;
    }

    void Start()
    {
        if (ServerManager.Instance != null)
            ServerManager.Instance.onServerConnect.AddListener(() => {
                ServerManager.Instance.GetLoadingTips((tips) => {
                    RandomTipText = tips;
                });
            });
    }

    public static void Show(string mainText = "PREPARING FOR LIFT-OFF...", string tipText = "Hold onto your chair while we load all the information for you!") {
        if (Instance == null) return;
        var p = FindObjectOfType<PlayerManager>();
        if (p != null) p.canControl = false;
        Instance.Main.text = mainText.ToUpper();
        Instance.Tip.text = tipText;

        Instance.Group.blocksRaycasts = true;
        Instance.Group.interactable = true;
        Instance.Group.DOFade(1f, 0.1f);
    }

    public static void Show() {
        Show(Instance.RandomMainText[Random.Range(0, Instance.RandomMainText.Count)], Instance.RandomTipText[Random.Range(0, Instance.RandomTipText.Count)]);
    }

    public static void Drop() {
        if (Instance == null) return;

        var p = FindObjectOfType<PlayerManager>();
        if (p != null) p.canControl = true;

        Instance.Group.blocksRaycasts = false;
        Instance.Group.interactable = false;
        Instance.Group.DOFade(0f, 0.1f);
    }
}
