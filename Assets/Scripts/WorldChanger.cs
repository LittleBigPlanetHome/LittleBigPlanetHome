using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class WorldChanger : MonoBehaviour
{
    [SerializeField] GameObject PodBGController;
    public List<FogAdjuster> FogSettings = new List<FogAdjuster>();
    PodAudioPlayer bgChange;
    [FormerlySerializedAs("Audios")] public List<PodAudioPlayer> AudioList = new List<PodAudioPlayer>();
    public static WorldChanger i { get; private set; }

    private void Awake()
    {
        if (i == null)
        {
            i = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
        }

        bgChange = LoadOneShotClip("bg_change");
    }

    public PodAudioPlayer GetAudioPlayer(string devId)
    {
        return AudioList.FirstOrDefault(a => a.DevId.ToLower() == devId.ToLower());
    }

    public PodAudioPlayer LoadOneShotClip(string devId)
    {
        PodAudioPlayer audio = GetAudioPlayer(devId);
        if (audio != null && audio.Source == null)
        {
            audio.Source = Camera.main.gameObject.AddComponent<AudioSource>();
            audio.Source.clip = audio.Clip;
            if (devId == "SND_PodExit")
                audio.Source.volume = 0.3f;
            else
                audio.Source.volume = 0.85f;
        }
        return audio;
    }

    public void ApplyWorldSettings(string fogId)
    {
       FogAdjuster FogA = GetFogSettings(fogId);  

        if (FogA != null && FogA.fogId == fogId)
        {
            Color fogC = FogA.FogColor;
            float fogD = FogA.FogDensity;
            Color fogL = FogA.AmbientColor;
            DOTween.To(() => RenderSettings.fogColor, x => RenderSettings.fogColor = x, fogC, 2f);
            //RenderSettings.fogColor = FogA.FogColor;
            DOTween.To(() => RenderSettings.fogDensity, x => RenderSettings.fogDensity = x, fogD, 2f);
            //RenderSettings.fogDensity = FogA.FogDensity;
            DOTween.To(() => RenderSettings.ambientSkyColor, x => RenderSettings.ambientSkyColor = x, fogL, 2f);
            //RenderSettings.ambientLight = FogA.AmbientColor;
            if (FogA.useSkybox || FogA.SkyBoxMat != null)
            {
                RenderSettings.skybox = FogA.SkyBoxMat;
            }
            else if (!FogA.useSkybox || FogA.SkyBoxMat == null)
            {
                RenderSettings.skybox = null;
            }
            foreach (Transform transform in PodBGController.transform)
            {
                if (transform.childCount != 0)
                {
                    if (transform.GetChild(0).gameObject.name != FogA.PodBG.name + "(Clone)")
                    {
                        Destroy(transform.GetChild(0).gameObject);
                    }
                    continue;
                }

                if (transform.gameObject.name == FogA.transform_id)
                {
                    if (transform.childCount != 0)
                    {
                        if (transform.GetChild(0).gameObject.name != FogA.PodBG.name + "(Clone)" || transform.GetChild(0).gameObject.name != FogA.PodBG.name)
                        {
                            Destroy(transform.GetChild(0).gameObject);
                        }
                    }
                    if (transform.childCount == 0)
                    {
                        Instantiate(FogA.PodBG, transform);
                        StartCoroutine("PlayAudioOnInstantiate", FogA.PodBG); //this is so the audio can wait when the whole bg is loaded in so it doesnt play it early
                    }                  
                }
            }
        }
    }

    public IEnumerator PlayAudioOnInstantiate(GameObject prefab)
    {
        yield return new WaitForEndOfFrame();
        if (prefab != null && prefab.activeSelf)
        {
            if (bgChange.Source != null && bgChange.Source.clip != null)
            {
                if (!bgChange.Source.isPlaying)
                {
                    bgChange.Source.Play();
                }
            }
        }
    }

    public FogAdjuster GetFogSettings(string fogId)
    {
        return FogSettings.FirstOrDefault(d => d.fogId == fogId);
    }


    [System.Serializable]
    public class FogAdjuster
    {
        [Header("Fog Settings")]
        [Space(5f)]
        public string familiarName = "Fog Slot1";
        public string fogId = "default_0";
        public Color FogColor;
        public float FogDensity;
        public Color AmbientColor;
        [Header("Background Settings")]
        [Space(5f)]
        public string transform_id = "id_null";
        [Space(5f)]
        public bool useSkybox = false;
        public Material SkyBoxMat;
        [Space(5f)]
        public GameObject PodBG;
    }
}
