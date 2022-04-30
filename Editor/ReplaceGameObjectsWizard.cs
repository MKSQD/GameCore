using UnityEditor;
using UnityEngine;

public class ReplaceGameObjectsWizard : ScriptableWizard {
    public GameObject Prefab;
    public bool KeepOriginalNames = false;

    [MenuItem("Edit/Replace Selected #R", priority = 142)]
    static void CreateWizard() {
        var replaceGameObjects = DisplayWizard<ReplaceGameObjectsWizard>("Replace Selected", "Replace");
    }

    void OnWizardCreate() {
        foreach (var go in Selection.gameObjects) {
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

            Undo.DestroyObjectImmediate(go);
        }
    }
}