using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LockableButton : Button
{
    public UnityEvent<bool> onInteractionChanged = new UnityEvent<bool>();
    public UnityEvent<BaseEventData> onSelected = new UnityEvent<BaseEventData>();

    public bool newInteractable {
        get => interactable;
        set
        {
            print("changed interactable");
            if (value != interactable)
            {
                interactable = value;
                onInteractionChanged?.Invoke(value);

                if (value) {
                    GetComponentInChildren<TextMeshProUGUI>().fontStyle = FontStyles.Normal;
                }
                else {
                    GetComponentInChildren<TextMeshProUGUI>().fontStyle = FontStyles.Strikethrough;
                }
            }
        }
    }

    [ContextMenu("Test Interactable Change")]
    public void TestInteractable() {
        newInteractable = !newInteractable;
    }



}
