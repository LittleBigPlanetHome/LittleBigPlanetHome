using UnityEngine;
using UnityEngine.Events;
using PopitStuff.Enums;

namespace PopitStuff.Objects {
    [CreateAssetMenu(fileName = "New Base Popit Selectable Item", menuName = "LittleBigPlanet/Popit/Base Selectable", order = 0)]


    public class BasePopitSelectable : ScriptableObject
    {
        public static BasePopitSelectable i { get; private set; }

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
        public Sprite displayIcon;
        public string displayName = "Some type of object";

        [TextArea(3, 10)]
        public string displayDesc = "This is some type of object.";
        public string author = "ImaginedStudios";

        public PopitSelectableType type = PopitSelectableType.CapturedObject;

        [Space(30f)]
        [Tooltip("Execute special functions here. (eg. OpenPopit:ObjectsBag)")] public string specialEvents = "";
        public UnityEvent onUse = new UnityEvent();

        public void Use() {
            Debug.Log($"POPIT: Selectable \"{displayName}\" ({name}) has been used.");
            onUse?.Invoke();
        }
    }
}

namespace PopitStuff.Enums {
    public enum PopitSelectableType {
        CapturedObject,
        Pod,
        Logic,
        Decoration,
        Music,
        Prop,
        Costume,
        Material,
        UsableFunction
    }
}