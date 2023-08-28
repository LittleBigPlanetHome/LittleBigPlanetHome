using TMPro;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.SceneManagement;

public class Popup : Singleton<Popup>
{
    public bool IsOpen => opened;
    bool opened;
    PlayerManager player;
    Action backedUpClick;

    private void Start() {
        player = FindObjectOfType<PlayerManager>();

        SceneManager.activeSceneChanged += (a, b) => {
            player = FindObjectOfType<PlayerManager>();
        };
    }

    public void OpenPopup(string title = "This is a popup!", string buttonText = "OK", Action onButtonClick = null) {
        transform.localScale = new Vector3(0, 0, 0);
        transform.eulerAngles = new Vector3(0, 0, -10);
        transform.Find("desc").GetComponent<TextMeshProUGUI>().text = title;
        transform.Find("button").Find("text").GetComponent<TextMeshProUGUI>().text = buttonText;

        if (onButtonClick != null) {
            backedUpClick = onButtonClick;
        }

        transform.DOScale(new Vector3(0.60f, 0.60f, 0.60f), 0.2f).OnComplete(() => {
            transform.DOScale(new Vector3(0.57f, 0.57f, 0.57f), 0.2f);
        });
        transform.DORotate(new Vector3(0, 0, 5), 0.2f).OnComplete(() => {
            transform.DORotate(new Vector3(0, 0, 1.5f), 0.2f).OnComplete(() => opened = true);
        });
    }

    private void Update() {
        if (opened) {
            if (player.controls.Gameplay.Interaction.triggered) {
                ClosePopup();
            }
        }
    }

    public void ClosePopup() {
        transform.DOScale(new Vector3(0.60f, 0.60f, 0.60f), 0.2f).OnComplete(() => {
            transform.DOScale(new Vector3(0, 0, 0), 0.2f);
        });
        transform.DORotate(new Vector3(0, 0, 5), 0.2f).OnComplete(() => {
            transform.DORotate(new Vector3(0, 0, -10), 0.2f).OnComplete(() => opened = false);
        });
        if (backedUpClick != null) { print("click doing"); backedUpClick(); backedUpClick = null; };
    }
}
