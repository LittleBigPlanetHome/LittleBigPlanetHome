using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using PopitStuff.Objects;

public class PopitEditorWindow : EditorWindow
{
    [MenuItem("Tools/Popit Editor")]
    public static void ShowWindow() {
        var window = GetWindow<PopitEditorWindow>();
        window.titleContent = new GUIContent("Popit Editor");
        window.minSize = new Vector2(800, 600);
    }

    void OnEnable()
    {
        VisualTreeAsset original = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/PopitEditorWindow.uxml");
        TemplateContainer treeAsset = original.CloneTree();
        rootVisualElement.Add(treeAsset);

        StyleSheet style = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/PopitEditorStyles.uss");
        rootVisualElement.styleSheets.Add(style);

        CreatePopitElements();
    }

    void CreatePopitElements() {
        GetPopitElements(out BasePopitSelectable[] popitObjs);

        ListView popitObjsList = rootVisualElement.Query<ListView>();
        popitObjsList.makeItem = () => new Label();
        popitObjsList.bindItem = (element, i) => { (element as Label).text = popitObjs[i].displayName; (element as Label).name = popitObjs[i].name; };

        popitObjsList.itemsSource = popitObjs;
        popitObjsList.itemHeight = 16;
        popitObjsList.selectionType = SelectionType.Single;

        popitObjsList.onSelectionChange += (enumerable) => {
            foreach (Object it in enumerable) {
                Box infoBox = rootVisualElement.Query<Box>("popit-info").First();
                infoBox.Clear();

                BasePopitSelectable popitSelectable = it as BasePopitSelectable;

                SerializedObject serializedPopitSel = new SerializedObject(popitSelectable);
                SerializedProperty serPropPopit = serializedPopitSel.GetIterator();
                serPropPopit.Next(true);

                while (serPropPopit.NextVisible(false)) {
                    PropertyField prop = new PropertyField(serPropPopit);

                    prop.SetEnabled(serPropPopit.name != "m_Script");
                    prop.Bind(serializedPopitSel);
                    infoBox.Add(prop);
                    
                    if (serPropPopit.name == "displayIcon") {
                        prop.RegisterCallback<ChangeEvent<UnityEngine.Object>>((changeEvt) => LoadPopitImage(popitSelectable.displayIcon.texture));
                    }
                    else if (serPropPopit.name == "displayName") {
                        prop.RegisterCallback<ChangeEvent<UnityEngine.Object>>((changeEvt) => ChangeLabel(popitSelectable.name, popitSelectable.displayName));
                    }
                } 

                LoadPopitImage(popitSelectable.displayIcon.texture);
            }
        };

        popitObjsList.Rebuild();
    }

    void GetPopitElements(out BasePopitSelectable[] selectables) {
        var guids = AssetDatabase.FindAssets("t:BasePopitSelectable");
        selectables = new BasePopitSelectable[guids.Length];
        for (var i = 0; i < guids.Length; i++) {
            var path = AssetDatabase.GUIDToAssetPath(guids[i]);
            selectables[i] = AssetDatabase.LoadAssetAtPath<BasePopitSelectable>(path);
        }
    }

    void ChangeLabel(string objName, string text) {
        Debug.Log("a xd");
        var preview = rootVisualElement.Query<Label>(objName).First();
        preview.text = text;
    }

    void LoadPopitImage(Texture tex) {
        var preview = rootVisualElement.Query<Image>("preview").First();
        preview.image = tex;
    }
}
