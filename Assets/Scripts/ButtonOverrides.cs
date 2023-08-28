using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using DG.Tweening;

public class ButtonOverrides : MonoBehaviour, IPointerEnterHandler
{
    LockableButton savedBtn;

    public void Start() {
        if (GetComponent<LockableButton>() != null) {
            var btnA = GetComponent<LockableButton>();

            /*if (gameObject.name == "Community" || gameObject.name == "Store") {
                btnA.newInteractable = false;
                ServerManager.instance.onServerConnect.AddListener(() => {
                    btnA.newInteractable = true;
                });
            }*/

            if (btnA.tag != "freeBtn")
            {
                ColorBlock cbA = btnA.colors;
                cbA.normalColor = ServerManager.Instance.buttonsIdleColor;
                cbA.disabledColor = ServerManager.Instance.buttonsDisabledColor;
                cbA.pressedColor = ServerManager.Instance.buttonsClickedColor;
                cbA.selectedColor = ServerManager.Instance.buttonsSelectedColor;
                cbA.highlightedColor = ServerManager.Instance.buttonsHoveredColor;
                cbA.fadeDuration = 0.05f;

                btnA.colors = cbA;             
            }
            return;

        }

        Button.ButtonClickedEvent onClickThing = null;
        Navigation n = new Navigation();
        bool wasInteractable = true;
        LockableButton btn = null;
        if (GetComponent<Button>() != null) {
            wasInteractable = GetComponent<Button>().interactable;
            onClickThing = GetComponent<Button>().onClick;
            n = GetComponent<Button>().navigation;
            DestroyImmediate(GetComponent<Button>());
            btn = gameObject.AddComponent<LockableButton>();
        }

        /*if (gameObject.name == "Community" || gameObject.name == "Store") {
            btn.newInteractable = false;
            ServerManager.instance.onServerConnect.AddListener(() => {
                btn.newInteractable = true;
            });
        }*/

        if (btn == null)
            btn = GetComponent<LockableButton>();
        ColorBlock cb = btn.colors;

        if (ServerManager.Instance == null)
        {
            cb.normalColor = Color.green;
            cb.disabledColor = Color.gray;
            cb.pressedColor = Color.green - new Color(0.5f, 0.5f, 0.5f);
            cb.selectedColor = Color.green - new Color(0.5f, 0.5f, 0.5f);
            cb.highlightedColor = Color.green - new Color(0.2f, 0.2f, 0.2f);
        }
        else
        {
            cb.normalColor = ServerManager.Instance.buttonsIdleColor;
            cb.disabledColor = ServerManager.Instance.buttonsDisabledColor;
            cb.pressedColor = ServerManager.Instance.buttonsClickedColor;
            cb.selectedColor = ServerManager.Instance.buttonsSelectedColor;
            cb.highlightedColor = ServerManager.Instance.buttonsHoveredColor;
        }
        
        cb.fadeDuration = 0.05f;
        btn.interactable = wasInteractable;

        if (wasInteractable) {
            //GetComponentInChildren<TextMeshProUGUI>().fontStyle = FontStyles.Normal;
        }
        else {
            //GetComponentInChildren<TextMeshProUGUI>().fontStyle = FontStyles.Strikethrough;
        }

        btn.navigation = n;
        btn.colors = cb;
        if (onClickThing != null) {
            btn.onClick = onClickThing;
        }
        if (btn.onInteractionChanged == null) {
            print("oh no our interaction is broken");
        }
        
        btn.onSelected.AddListener(_ => {
            btn.transform.DOScale(1.2f, 0.4f);
        });

        savedBtn = btn;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!Audios.Instance)
            return;
        
        if (Audios.Instance.onHoverButton && savedBtn && savedBtn.interactable)
        {
            if (FindObjectOfType<Popit>().currentState == PopitStuff.Enums.PopitState.Closed)
            {
                AudioSource.PlayClipAtPoint(Audios.Instance.onHoverButton.clip, Camera.main.transform.position, 1f);
            }
            else if (FindObjectOfType<Popit>().currentState != PopitStuff.Enums.PopitState.Closed)
            {
                AudioSource.PlayClipAtPoint(Audios.Instance.popitHover, Camera.main.transform.position, 1f);
            }
        }
    }
}
