using System.Reflection;
using UnityEditor;
using UnityEngine;

public class GlobalDataEditor : EditorWindow {
    Vector2 typesScrollPos, inspectorScrollPos;
    int selectedIdx;

    [MenuItem("Window/Global Data")]
    static void Init() {
        var window = EditorWindow.GetWindow<GlobalDataEditor>("Global Data");
        window.Show();
    }

    void OnGUI() {
        GUILayout.BeginHorizontal();
        {
            var types = TypeCache.GetTypesDerivedFrom<IGlobalData>();

            typesScrollPos = GUILayout.BeginScrollView(typesScrollPos, false, true, GUILayout.Width(200));
            {
                for (int i = 0; i < types.Count; ++i) {
                    var type = types[i];
                    if (type.IsAbstract || type.IsGenericType)
                        continue;

                    if (GUILayout.Button(type.ToString())) {
                        selectedIdx = i;
                        break;
                    }
                }
            }
            GUILayout.EndScrollView();

            //
            var selectedType = types[selectedIdx];
            var instance = selectedType.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.FlattenHierarchy);
            var inst = (ScriptableObject)instance.GetValue(null);

            inspectorScrollPos = GUILayout.BeginScrollView(inspectorScrollPos);
            {
                var editor = Editor.CreateEditor(inst);
                editor.DrawDefaultInspector();
            }
            GUILayout.EndScrollView();
        }
        GUILayout.EndHorizontal();
    }
}
