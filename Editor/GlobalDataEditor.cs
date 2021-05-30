using System;
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

            typesScrollPos = GUILayout.BeginScrollView(typesScrollPos, GUILayout.Width(260));
            {
                var activeButtonStyle = new GUIStyle(GUI.skin.button);
                activeButtonStyle.fontStyle = FontStyle.Bold;

                for (int i = 0; i < types.Count; ++i) {
                    var type = types[i];
                    if (!IsTypeValid(type))
                        continue;

                    var inst = GetInstanceByType(type);

                    var style = i != selectedIdx ? GUI.skin.button : activeButtonStyle;
                    var text = type.ToString();
                    if (EditorUtility.IsDirty(inst)) {
                        text += "*";
                    }
                    if (GUILayout.Button(text, style)) {
                        selectedIdx = i;
                        break;
                    }
                }
            }
            GUILayout.EndScrollView();

            //
            if (selectedIdx < types.Count && IsTypeValid(types[selectedIdx])) {
                inspectorScrollPos = GUILayout.BeginScrollView(inspectorScrollPos);
                {
                    var selectedType = types[selectedIdx];
                    var inst = GetInstanceByType(selectedType);

                    var editor = Editor.CreateEditor(inst);
                    editor.DrawDefaultInspector();
                }
                GUILayout.EndScrollView();
            }
        }
        GUILayout.EndHorizontal();
    }

    ScriptableObject GetInstanceByType(Type type) {
        var instance = type.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.FlattenHierarchy);
        return (ScriptableObject)instance.GetValue(null);
    }

    static bool IsTypeValid(Type type) => !type.IsAbstract && !type.IsGenericType;
}
