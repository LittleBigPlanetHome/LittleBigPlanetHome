using UnityEngine;
using static System.Net.Mime.MediaTypeNames;

public class FPSController : MonoBehaviour
{
    public int targetFrameRate = 30;

    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        UnityEngine.Application.targetFrameRate = targetFrameRate;
    }

    private void Update()
    {
        Time.captureFramerate = targetFrameRate;
    }
}
