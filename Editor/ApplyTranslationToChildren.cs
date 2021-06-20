using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// Move all selected GameObjects to (0,0,0) while their children keep their world positions.
public static class ApplyTranslationToChildren {
    [MenuItem("Edit/Apply Translation To Children For Selected #&t", priority = 142)]
    static void CreateWizard() {
        foreach (var go in Selection.gameObjects) {
            Apply(go.transform);
        }
    }

    static void Apply(Transform parent) {
        var children = new List<Transform>();
        for (int i = 0; i < parent.childCount; ++i) {
            var child = parent.GetChild(i);
            children.Add(child);
        }

        try {
            foreach (var child in children) {
                Undo.RecordObject(child, "Child");
                child.SetParent(null);
            }

            Undo.RecordObject(parent, "Parent");
            parent.position = Vector3.zero;
            parent.rotation = Quaternion.identity;
            parent.localScale = Vector3.one;
        } finally {
            foreach (var child in children) {
                child.SetParent(parent);
            }
        }
    }
}