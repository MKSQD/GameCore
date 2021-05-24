using System;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(EventSource))]
public class EventSourceEditor : Editor {
    public override void OnInspectorGUI() {
        var src = (EventSource)target;

        var types = TypeCache.GetTypesDerivedFrom<IEvent>();

        var currentIdx = 0;

        if (src.evt != null) {
            var currentType = src.evt.GetType();


            for (int i = 0; i < types.Count; ++i) {
                if (types[i] == currentType) {
                    currentIdx = i + 1;
                    break;
                }
            }

            DrawDefaultInspector();
        }

        // EventType Popup
        var typesStr = new List<string>(types.Count + 1);
        typesStr.Add("None");
        for (int i = 0; i < types.Count; ++i) {
            typesStr.Add(types[i].ToString());
        }

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Event Type");
        var newIdx = EditorGUILayout.Popup(currentIdx, typesStr.ToArray());
        if (newIdx != currentIdx) {
            IEvent newEvent = null;
            if (newIdx != 0) {
                newEvent = (IEvent)Activator.CreateInstance(types[newIdx - 1]);
            }
            src.evt = newEvent;

            EditorUtility.SetDirty(src);
        }

        EditorGUILayout.EndHorizontal();
    }
}