using UnityEngine;

public class DebugFreeCamera : MonoBehaviour
{
    public Camera mainCamera;
    public Camera debugCamera;
    public GUISkin guiSkin;
    public float moveSpeed = 10.0f;
    public float lookSpeed = 5.0f;

    private bool debugCameraEnabled = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.B))
        {
            debugCameraEnabled = !debugCameraEnabled;
            mainCamera.enabled = !debugCameraEnabled;
            debugCamera.enabled = debugCameraEnabled;
            Cursor.visible = debugCameraEnabled;

            if (debugCameraEnabled)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
            }
        }

        if (debugCameraEnabled)
        {
            float horizontal = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
            float vertical = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
            float lookHorizontal = Input.GetAxis("Mouse X") * lookSpeed;
            float lookVertical = Input.GetAxis("Mouse Y") * lookSpeed;

            transform.Translate(new Vector3(horizontal, 0, vertical));
            transform.Rotate(new Vector3(0, lookHorizontal, 0));
            debugCamera.transform.Rotate(new Vector3(-lookVertical, 0, 0));
        }
    }

    private void OnGUI()
    {
        if (debugCameraEnabled)
        {
            Vector3 cameraPos = debugCamera.transform.position;
            GUI.skin = guiSkin;
            GUI.Label(new Rect(10, 10, 200, 50), "Debug Camera Location: " + cameraPos.ToString("F2"));
        }
    }
}
