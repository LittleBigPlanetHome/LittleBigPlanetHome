using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using PopitStuff.Enums;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.Events;
using PopitStuff.Objects;
using static WorldChanger;
using System;
using System.Reflection;

public class Popit : MonoBehaviour
{
    public static Popit i { get; private set; }
    public List<BasePopitFunctions> DefaultSelectables = new List<BasePopitFunctions>();
    public Camera cam1;
    public Camera cam2;
    [Space(15f)]

    public PopitState currentState = PopitState.Closed;
    public float sizeChangeTime = 0.65f;
    public RectTransform popitMask;

    [Space(15f)]
    public float popitDefaultWidth = 245f;
    public float popitDefaultHeight = 350f;
    public GameObject popitDefault;
    [Space(15f)]
    public float popitCreateToolsWidth = 375f;
    public float popitCreateToolsHeight = 570f;
    public GameObject popitCreateTools;

    private CanvasGroup cg;
    private GameObject player;
    
    [FormerlySerializedAs("Audios")] public List<PodAudioPlayer> AudioList = new List<PodAudioPlayer>();
    PodAudioPlayer PopitOpen, PopitClose, PopitSelect, PopitHover;

    public LineRenderer lineRenderer;
    private bool isShowingString = false;
    public Transform sackHand;
    public float maxOffset = 0.1f;

    GameObject uiPositionObject;
    Vector3 uiPosition;
    public Transform UIPanel;

    [Header("Sackboy Costume Settings")]
    [Space(2f)]
    public List<CostumeSackBoyItems> CostumeItems = new List<CostumeSackBoyItems>();
    [Space(5f)]
    [Header("Item Transforms")]
    [Space(2f)]
    public List<Vector3> ItemTransform = new List<Vector3>();
    public Transform HeadTransform;

    void Start()
    {
        lineRenderer.enabled = false;
        lineRenderer.startWidth = 0.025f;
        lineRenderer.endWidth = 0.025f;
        lineRenderer.widthCurve = AnimationCurve.Linear(0f, 0.025f, 1f, 0.025f);
        lineRenderer.numCornerVertices = 8;
        lineRenderer.numCapVertices = 8;

        uiPosition = Camera.main.ScreenToWorldPoint(RectTransformUtility.WorldToScreenPoint(Camera.main, cg.gameObject.transform.position));

        // Create a new GameObject and place it at the world space position of the UI panel
        uiPositionObject = new GameObject("UI Position");
        uiPositionObject.transform.position = uiPosition;
    }

    private void Update()
    {
        if (isShowingString)
        {
            UpdateStringPosition();
        }
    }

    public void ChangePopitState(PopitState state) {
        currentState = state;

        /*if (state != PopitState.Closed)
            FindObjectOfType<PlayerManager>().canControl = false;
        else 
            FindObjectOfType<PlayerManager>().canControl = true;
        */

        switch (state) {
            case PopitState.Default:
                Audios.i.onClick = Audios.i.popitBack;
                ChangeSize(popitDefaultWidth, popitDefaultHeight);
                CamZoomOut();
                foreach (Transform child in popitMask) {
                    if (child.name == "BG" || child.name == "DefaultState") child.gameObject.SetActive(true);
                    else child.gameObject.SetActive(false);
                }
                break;
            case PopitState.ScreenRightSide:
                Audios.i.onClick = Audios.i.popitClick;
                ChangeSize(popitCreateToolsWidth, popitCreateToolsHeight);
                break;
            default:
                break;
        }
    }

    public void OpenState(string state) {
        ChangePopitState((PopitState)System.Enum.Parse(typeof(PopitState), state));
    }

    public void ImportAsset(string assetPath) {
#if UNITY_EDITOR
        // wahahwahwahwahwahwa
#endif
    }

    public void ChangeSize(float width, float height) {
        popitMask.DOSizeDelta(new Vector2(width, height), sizeChangeTime);
    }

    public void DebugObjects() {
        ChangePopitState(PopitState.ScreenRightSide);
        popitMask.GetChild(1).gameObject.SetActive(false);
        popitMask.GetChild(2).gameObject.SetActive(true);
    }

    public void CustomizeSack()
    {
        ChangePopitState(PopitState.ScreenRightSide);
        popitMask.transform.GetChild(1).gameObject.SetActive(false);
        popitMask.transform.GetChild(3).gameObject.SetActive(true);
        CamZoomIN();
    }

    public void OpenPopit() {
        ChangePopitState(PopitState.Default);
        isShowingString = true;
        lineRenderer.enabled = isShowingString;        
        PopulatePopit();
        cg.DOFade(1f, sizeChangeTime).OnComplete(() => {cg.interactable = true; cg.blocksRaycasts = true;});
        PopitOpen.Source.Play();
    }

    public CostumeSackBoyItems GetSackBoyItems(string devID)
    {
        return CostumeItems.FirstOrDefault(d => d.devID == devID);
    }


    public void ChangeSackBoyCostume(string devID)
    {
        CostumeSackBoyItems SackboyItems = GetSackBoyItems(devID);
        GameObject sackMesh = (Resources.Load($"Sackboy/Costume/{SackboyItems.directoryID}") as GameObject);
        string partID = string.Empty;
        if (SackboyItems.rigID == RigPart.Head)
        {
            partID = "H";
            if (GameObject.Find("sackCostumeH") != null)
            {
                Destroy(GameObject.Find("sackCostumeH"));
                player.GetComponent<PlayerManager>().CostumeAnimationIdx(-2, 0.5f);
                if (GameObject.Find(sackMesh.name + "(Clone)") != null)
                {
                    return;
                }               
            }
            if (SackboyItems.devID.Contains("hair"))
            {
                player.GetComponent<PlayerManager>().CostumeAnimationIdx(1,1f);
            }
            else
            {
                player.GetComponent<PlayerManager>().CostumeAnimationIdx(2, 0.3f);
            }
        }
        else if (SackboyItems.rigID == RigPart.Body)
        {
            partID = "B";
            if (GameObject.Find("sackCostumeB") != null)
            {
                Destroy(GameObject.Find("sackCostumeB"));
                if (GameObject.Find(sackMesh.name + "(Clone)") != null)
                {
                    return;
                }
            }
        }
        else if (SackboyItems.rigID == RigPart.Eyes)
        {
            partID = "Eyes";
            if (GameObject.Find("sackCostumeEyes") != null)
            {
                Destroy(GameObject.Find("sackCostumeEyes"));
                if (GameObject.Find(sackMesh.name + "(Clone)") != null)
                {
                    return;
                }
            }
        }
        GameObject costumeParent = new GameObject($"sackCostume{partID}");
        costumeParent.transform.SetParent(GameObject.Find(SackboyItems.GetRigName()).transform);
        costumeParent.transform.localPosition = Vector3.zero;
        costumeParent.transform.localRotation = Quaternion.identity;
        costumeParent.transform.localScale = Vector3.one;

        GameObject InstOBJ = Instantiate(sackMesh, costumeParent.transform);
        //InstOBJ.transform.localPosition = Vector3.zero + ItemTransform[((int)SackboyItems.rigID)];
        InstOBJ.transform.localRotation = Quaternion.identity;
        if (SackboyItems.rigID == RigPart.Head)
        {
            InstOBJ.transform.localScale = new Vector3(0.01063636f, 0.01063636f, 0.01063636f);
        }
        else if (SackboyItems.rigID == RigPart.Eyes)
        {
            //InstOBJ.transform.localScale = new Vector3(0.00999999885f, 0.0100000016f, 0.00999998953f);
            //InstOBJ.transform.position = new Vector3(-0.00141587853f, -1.08929801f, 0.0553174168f);
            //InstOBJ.transform.eulerAngles = new Vector3(357.605591f, -0.00557765644f, 0.000328516209f);

            InstOBJ.transform.localScale = new Vector3(0.009890564f, 0.009890564f, 0.009890564f);
        }
        foreach(Transform t in InstOBJ.transform)
        {
            t.gameObject.layer = player.layer;
        }
        InstOBJ.transform.localPosition = Vector3.zero + ItemTransform[((int)SackboyItems.rigID)];
    }

    public void ChangeSackBoyMat(string devID)
    {
        CostumeSackBoyItems SackboyItems = GetSackBoyItems(devID);
        Material sackMat = (Resources.Load($"Sackboy/Costume/{SackboyItems.directoryID}") as Material);
        GameObject.Find("SackBoy PS4").transform.GetChild(0).GetComponent<Renderer>().material = sackMat;
        if (devID.Contains("Fuzz"))
        {
            Material sackFuzz = (Resources.Load($"Sackboy/Costume/{GetFuzzMaterial(devID)}" + "/sack_boy_fuzz") as Material);
            GameObject.Find("SackBoy PS4").transform.GetChild(0).GetComponent<Renderer>().sharedMaterials[1] = sackFuzz;

            Renderer myRenderer = GameObject.Find("SackBoy PS4").transform.GetChild(0).GetComponent<Renderer>();
            Material[] materials = myRenderer.materials;
            if (materials.Length >= 2)
            {
                materials[1] = sackFuzz; // Modify the second material
                myRenderer.materials = materials; // Assign the updated materials array back to the renderer
            }
        }
        else
        {
            Renderer myRenderer = GameObject.Find("SackBoy PS4").transform.GetChild(0).GetComponent<Renderer>();
            Material[] materials = myRenderer.materials;
            Material sackFuzz = (Resources.Load($"Sackboy/Costume/Default/sack_boy_fuzz") as Material);
            if (materials.Length >= 2)
            {
                materials[1] = sackFuzz; // Modify the second material
                myRenderer.materials = materials; // Assign the updated materials array back to the renderer
            }
        }
    }

    private string GetFuzzMaterial(string devID)
    {
        CostumeSackBoyItems SackboyItems = GetSackBoyItems(devID);
        string[] subDirectories = SackboyItems.directoryID.Split('/');
        string Output = string.Empty;
        if (subDirectories.Length > 1)
        {
            Output = string.Join("/", subDirectories.Take(subDirectories.Length - 1));
            Debug.Log(Output);
            return Output;
        }
        Debug.Log(Output);
        return string.Empty;
    }

    public void RemoveAllCostume()
    {
        ChangeSackBoyMat("default_skin");
        Renderer myRenderer = GameObject.Find("SackBoy PS4").transform.GetChild(0).GetComponent<Renderer>();
        Material[] materials = myRenderer.materials;
        Material sackFuzz = (Resources.Load($"Sackboy/Costume/Default/sack_boy_fuzz") as Material);
        if (materials.Length >= 2)
        {
            materials[1] = sackFuzz; // Modify the second material
            myRenderer.materials = materials; // Assign the updated materials array back to the renderer
        }
            if (GameObject.Find("sackCostumeH") != null)
            {
                Destroy(GameObject.Find("sackCostumeH"));
            }
            if (GameObject.Find("sackCostumeB") != null)
            {
                Destroy(GameObject.Find("sackCostumeB"));
            }
        if (GameObject.Find("sackCostumeEyes") != null)
        {
            Destroy(GameObject.Find("sackCostumeEyes"));
        }
    }

    public void CamZoomIN()
    {
        return;
        cam1.transform.DOMove(player.transform.position + new Vector3(0, 0.8f, -2f), 1f);
        cam2.transform.DOMove(player.transform.position + new Vector3(0, 0.8f, -2f), 1f);
    }

    public void CamZoomOut()
    {
        return;
        cam1.transform.DOMove(new Vector3(0, 8.36999989f, -5.61999989f), 1f);
        cam2.transform.DOMove(new Vector3(0, 8.36999989f, -5.61999989f), 1f);
    }

    public BasePopitFunctions GetSelectable()
    {
        return DefaultSelectables.FirstOrDefault();
    }

    public BasePopitSelectable GetBaseSelectable()
    {
        return GetSelectable().BaseSelectable;
    }

    public void PopulatePopit()
    {
        var holder = popitMask.GetChild(1).GetChild(0);
        foreach (Transform child in holder)
            Destroy(child.gameObject);

        var res = Resources.Load<GameObject>("prefabs/PopitItem");
        List<BasePopitFunctions> selectables = Popit.i.DefaultSelectables.ToList();
        foreach (BasePopitFunctions Selectable in selectables)
        {
            var Item = Instantiate(res, holder);
            var PopitItem = Selectable.BaseSelectable;
            Item.name = PopitItem.displayName;
            Item.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = PopitItem.displayIcon;
            var Btn = Item.GetComponent<UnityEngine.UI.Button>();

            // capture a reference to the current Selectable object
            var currentSelectable = Selectable;

            // set up the onClick listener using a lambda expression that uses the captured reference
            Btn.onClick.AddListener(() => currentSelectable.Use());
            Btn.onClick.AddListener(() => AudioSource.PlayClipAtPoint(Audios.i.onClick, Camera.main.transform.position, 1));

            if (!string.IsNullOrEmpty(PopitItem.specialEvents))
            {
                var Commands = PopitItem.specialEvents.Split(";");
                foreach (var Command in Commands)
                {
                    var Arguments = Command.ToLower().Split(":");
                    switch (Arguments[0])
                    {
                        case "openpopit":
                            var StateToOpen = Arguments[1].ToLower();
                            switch (StateToOpen)
                            {
                                case "objectsbag":
                                    break;
                            }
                            break;
                    }
                }
            }
        }
    }

    public void ClosePopit() {
        ChangePopitState(PopitState.Closed);
        cg.DOFade(0f, sizeChangeTime).OnComplete(() => {cg.interactable = false; cg.blocksRaycasts = false;});
        isShowingString = false;
        lineRenderer.enabled = isShowingString;
        CamZoomOut();
        PopitClose.Source.Play();
    }

    private void Awake() {
        if (i == null) {
            i = this;
        } else {
            Destroy(this);
        }

        PopitOpen = LoadOneShotClip("popit_open");
        PopitClose = LoadOneShotClip("popit_close");
        PopitSelect = LoadOneShotClip("popit_select");
        PopitHover = LoadOneShotClip("popit_hover");
        player = GameObject.Find("Sackboy").gameObject;
        cg = gameObject.GetComponent<CanvasGroup>();
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

    private void UpdateStringPosition()
    {
        Vector3[] points = new Vector3[2]; // increased number of points to 6
        points[0] = sackHand.position;
        points[1] = UIPanel.position;
        // Add some random offset to the middle point to make the line look more "alive"
        //Vector3 offset = Random.insideUnitSphere * maxOffset;
        //points[1] = Vector3.Lerp(points[0], points[1], 0.5f) + offset;
        lineRenderer.SetPositions(points);
    }
}

[System.Serializable]
public class BasePopitFunctions
{
    public BasePopitSelectable BaseSelectable;
    public UnityEvent onUse = new UnityEvent();

    public void Use()
    {
        Debug.Log($"POPIT: Selectable \"{BaseSelectable.displayName}\" ({BaseSelectable.name}) has been used.");
        onUse?.Invoke();
    }
}

[System.Serializable]
public class CostumeSackBoyItems
{
    public string FamiliarName = "Default Costume";
    public string devID = "Default_Costume";
    public string directoryID = "Default/default_costume";
    public RigPart rigID = RigPart.Head;

    public string GetRigName()
    {
        RigPart part = rigID;
        RigPartAttribute attribute = typeof(RigPart)
            .GetField(part.ToString())
            .GetCustomAttribute<RigPartAttribute>();

        return attribute?.Name;
    }
}

public class RigPartAttribute : Attribute
{
    public string Name { get; }

    public RigPartAttribute(string name)
    {
        Name = name;
    }
}

public enum RigPart
{
    [RigPart("Bip01 Head")]
    Head = 0,
    [RigPart("SackBoy PS4")]
    Body = 1,
    [RigPart("Bid Feet")]
    Feet = 2,
    [RigPart("Bip01 Head")]
    Eyes = 3,

    None = -1
}

namespace PopitStuff {
    namespace Enums {
        public enum PopitState {
            Closed,
            Default,
            ScreenRightSide,
            ScreenLeftSide,
            Fullscreen
        }
    }
}