using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ReplaceGameObjectsWizard : ScriptableWizard {
    public GameObject Prefab;
    public bool KeepOriginalNames = false;

    [MenuItem("Edit/Replace Selected #R", priority = 142)]
    static void CreateWizard() {
        DisplayWizard<ReplaceGameObjectsWizard>("Replace Selected", "Replace");
    }

    protected void OnWizardCreate() {
        var newSelection = new List<Object>();
        foreach (var go in Selection.gameObjects) {
            if (PrefabUtility.IsPartOfPrefabAsset(go))
                continue;

            var newObject = (GameObject)PrefabUtility.InstantiatePrefab(Prefab);
            if (newObject == null) { // Maybe not a prefab?
                newObject = Instantiate(Prefab);
            }
            Undo.RegisterCreatedObjectUndo(newObject, "Replaced GameObject");

            newObject.transform.SetParent(go.transform.parent, true);
            newObject.transform.localPosition = go.transform.localPosition;
            newObject.transform.localRotation = go.transform.localRotation;
            newObject.transform.localScale = go.transform.localScale;
            newObject.isStatic = go.isStatic;
            if (KeepOriginalNames) {
                newObject.transform.name = go.transform.name;
            }

            newSelection.Add(newObject);

            Undo.DestroyObjectImmediate(go);
        }
        Selection.objects = newSelection.ToArray();
    }
}