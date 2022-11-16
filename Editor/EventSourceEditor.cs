using System;
using System.Collections.Generic;
using UnityEditor;

namespace GameCore {
    [CustomEditor(typeof(EventSource))]
    public class EventSourceEditor : Editor {
        public override void OnInspectorGUI() {
            var src = (EventSource)target;

            DrawDefaultInspector();

            // EventType Popup
            var types = TypeCache.GetTypesDerivedFrom<IEvent>();
            var typesStr = new List<string>(types.Count + 1);
            var typesIndices = new List<int>(types.Count + 1);
            typesStr.Add("None");
            typesIndices.Add(-1);

            var currentPopupIdx = 0;
            var currentType = src.evt?.GetType();
            for (int i = 0; i < types.Count; ++i) {
                if (types[i] == currentType) {
                    currentPopupIdx = typesIndices.Count;
                }

                typesStr.Add(types[i].ToString());
                typesIndices.Add(i);
            }

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Event Type");
            var popupIdx = EditorGUILayout.Popup(currentPopupIdx, typesStr.ToArray());
            if (popupIdx < 0 || popupIdx >= typesIndices.Count) {
                popupIdx = 0;
            }

            if (popupIdx != currentPopupIdx) {
                var newTypeIdx = typesIndices[popupIdx];

                IEvent newEvent = null;
                if (newTypeIdx != -1) {
                    newEvent = (IEvent)Activator.CreateInstance(types[newTypeIdx]);
                }
                src.evt = newEvent;

                EditorUtility.SetDirty(src);
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}