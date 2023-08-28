using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Interactable : MonoBehaviour
{
    public float maxDistance = 3f;
    public UnityEvent onInteract;
    public string interactionStringId = "GenericInteract";
    public KeyCode interactKey = KeyCode.E;
    public GameObject interactionPopup;
    PlayerManager player;

    private void Start() {
        player = FindObjectOfType<PlayerManager>();
    }

    private void Update() {
        if (player.canControl) {
            float distance = Vector3.Distance(player.transform.position, transform.position);
            if (distance <= maxDistance) {
                if (interactionPopup) {
                    if (interactionPopup.activeInHierarchy == false)
                        interactionPopup.SetActive(true);
                }

                if (player.controls.Gameplay.Interaction.triggered) {
                    print("Player interacted!");
                    player.EnterPodStateAnim();
                    onInteract?.Invoke();
                }
            } else {
                if (interactionPopup) {
                    if (interactionPopup.activeInHierarchy == true)
                        interactionPopup.SetActive(false);
                }
            }
        }
        else {
            if (interactionPopup) {
                if (interactionPopup.activeInHierarchy == true)
                    interactionPopup.SetActive(false);
            }
        }
    }
}
