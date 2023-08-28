using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ChangeTexture : MonoBehaviour
{
    public Material material;
    public string url;

    void Start()
    {
        StartCoroutine(GetTexture());
    }

    IEnumerator GetTexture()
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Texture texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            material.mainTexture = texture;
        }
    }
}
