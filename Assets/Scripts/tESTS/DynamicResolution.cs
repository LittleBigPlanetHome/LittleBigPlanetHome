using UnityEngine;

public class DynamicResolution : MonoBehaviour
{
    [SerializeField] private float minScale = 0.5f;
    [SerializeField] private float maxScale = 1.0f;
    [SerializeField] private float maxFrameTimeInSeconds = 1 / 30f; // Maximum frame time allowed in seconds
    [SerializeField] private float minResolution = 240f; // Minimum resolution allowed

    private void Update()
    {
        float fps = 1.0f / Time.deltaTime;

        if (fps < 60f) // Lower resolution if FPS drops below 60
        {
            float scale = Mathf.Clamp(maxFrameTimeInSeconds / Time.deltaTime, minScale, maxScale);
            float newWidth = Mathf.RoundToInt(Screen.width * scale);
            float newHeight = Mathf.RoundToInt(Screen.height * scale);
            if (newWidth > minResolution) // Don't go below minimum resolution
            {
                Screen.SetResolution((int)newWidth, (int)newHeight, true);
            }
        }
    }
}
