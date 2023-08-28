using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using static System.Net.Mime.MediaTypeNames;

public class ScreenFader : MonoBehaviour
{
    public float fadeTime = 1.0f;
    public AudioClip soundEffect;
    public string sceneName;
    public UnityEngine.UI.Image fadeImage;

    private bool isFading = false;

    public void StartFade()
    {
        if (!isFading)
        {
            StartCoroutine(FadeToWhite());
        }
    }

    IEnumerator FadeToWhite()
    {
        isFading = true;

        AudioSource audioSource = GetComponent<AudioSource>();
        if (soundEffect != null && audioSource != null)
        {
            audioSource.PlayOneShot(soundEffect);
        }

        float timeElapsed = 0;
        Color currentColor = fadeImage.color;
        Color targetColor = Color.white;

        while (timeElapsed < fadeTime)
        {
            currentColor = Color.Lerp(currentColor, targetColor, timeElapsed / fadeTime);
            currentColor.a = (timeElapsed / fadeTime);
            fadeImage.color = currentColor;
            yield return null;
            timeElapsed += Time.deltaTime;
        }

        SceneManager.LoadScene(sceneName);
    }
}
