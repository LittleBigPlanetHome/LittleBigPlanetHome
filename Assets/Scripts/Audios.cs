#if UNITY_EDITOR
using PopitStuff.Enums;
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

public class Audios : Singleton<Audios>
{
    public static Audios i { get; private set; }
    public AudioSource onHoverButton;
    public AudioClip onClick;
    public AudioClip DefaultClick;
    public AudioClip popitHover;
    public AudioClip popitClick;
    public AudioClip popitCostume;
    public AudioClip popitBack;

    private MusicPodType _type = MusicPodType.LBP2;
    public MusicPodType type;

    [ContextMenu("Toggle Karting Pod")]
    public void ToggleKartingOverride() {
    }

    [ContextMenu("Toggle LBP2 Pod")]
    public void ToggleLBP2Override() {
    }

    [ContextMenu("Toggle LBP1 Pod")]
    public void ToggleLBP1Override() {
    }

    [ContextMenu("Toggle LBP3 Pod")]
    public void ToggleLBP3Override() {
    }

    [ContextMenu("Toggle LBP3 Alpha Pod")]
    public void ToggleLBP3AlphaOverride() {
    }

    [ContextMenu("Toggle LBP PS Vita Pod")]
    public void ToggleLBPPSVitaOverride() {
    }

    public bool ShouldPlayBackgroundTrack() {
        return false;
    }

    private void Awake()
    {
        if (i == null)
        {
            i = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start() {
        foreach (Button btn in Resources.FindObjectsOfTypeAll<Button>()) {
#if UNITY_EDITOR
            if (PrefabUtility.GetPrefabAssetType(btn) != PrefabAssetType.NotAPrefab)
                 continue;
#endif
            if (btn.gameObject.GetComponent<ButtonOverrides>() == null)
                btn.gameObject.AddComponent<ButtonOverrides>();         

                btn.onClick.AddListener(() => AudioSource.PlayClipAtPoint(onClick, Camera.main.transform.position, 1));                     
        }
    }


}

public enum MusicPodType {
    Karting,
    LBP1,
    LBP2,
    LBP3,
    LBP3Alpha,
    LBPVita
}