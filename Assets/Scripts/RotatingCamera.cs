using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;

public class RotatingCamera : MonoBehaviour
{
    public Camera cam;
    public Transform main;
    PlayerManager player;
    Vector3 lastPos;
    Vector3 originalEA;

    public bool isEnabled = false;

    private void Awake() {
        player = FindObjectOfType<PlayerManager>();
    }

    public void ResetToOriginalRotation(float time = 0.8f) {
        main.DORotate(originalEA, time);
    }

    [ContextMenu("Reset to original rotation")]
    private void ContextMenuOGRotate() {
        ResetToOriginalRotation();
    }

    public void UpdateStartingEA() {
        originalEA = main.eulerAngles;
    }

    private void Start() {
        originalEA = main.eulerAngles;
    }

    private void Update() {
        if (!isEnabled) return;

        if (Mouse.current.rightButton.wasPressedThisFrame) {
            lastPos = cam.ScreenToViewportPoint(Mouse.current.position.ReadValue());
        }

        if (Mouse.current.rightButton.isPressed) {
            Vector3 dir = lastPos - cam.ScreenToViewportPoint(Mouse.current.position.ReadValue());

            main.Rotate(new Vector3(1, 0, 0), dir.y * 180);
            main.Rotate(new Vector3(0, 1, 0), -dir.x * 180, Space.World);

            if (main.eulerAngles.x >= 120) {
                main.eulerAngles = new Vector3(120, main.eulerAngles.y, main.eulerAngles.z);
            }

            lastPos = cam.ScreenToViewportPoint(Mouse.current.position.ReadValue());
        }
    }
}
