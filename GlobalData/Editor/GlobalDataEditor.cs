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

            typesScrollPos = GUILayout.BeginScrollView(typesScrollPos, false, true, GUILayout.Width(200));
            {
                var activeButtonStyle = new GUIStyle(GUI.skin.button);
                activeButtonStyle.fontStyle = FontStyle.Bold;

                for (int i = 0; i < types.Count; ++i) {
                    var type = types[i];
                    if (!IsTypeValid(type))
                        continue;

                    var style = i != selectedIdx ? GUI.skin.button : activeButtonStyle;
                    if (GUILayout.Button(type.ToString(), style)) {
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
                    var instance = selectedType.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.FlattenHierarchy);
                    var inst = (ScriptableObject)instance.GetValue(null);

                    var editor = Editor.CreateEditor(inst);
                    editor.DrawDefaultInspector();
                }
                GUILayout.EndScrollView();
            }
        }
        GUILayout.EndHorizontal();
    }

    static bool IsTypeValid(Type type) => !type.IsAbstract && !type.IsGenericType;
}
