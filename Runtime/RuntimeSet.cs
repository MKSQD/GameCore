using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class RuntimeSet : ScriptableObject {
#if UNITY_EDITOR
    [Tooltip("Texture name in the Gizmos folder. Optional for the editor only.")]
    public string GizmoIconName;
#endif

    public abstract void Add(GameObject gameObject);
    public abstract void Remove(GameObject gameObject);

#if UNITY_EDITOR
    public void DrawGizmos(GameObject gameObject) {
        if (GizmoIconName != null) {
            Gizmos.DrawIcon(gameObject.transform.position, GizmoIconName);
        }
    }
#endif
}


public class RuntimeSet<T> : RuntimeSet where T : Component {
    public List<T> Items = new();

    public T Random {
        get {
            if (Items.Count == 0)
                return null;

            return Items[UnityEngine.Random.Range(0, Items.Count)];
        }
    }

    public bool TryGetRandom(out T value) {
        if (Items.Count == 0) {
            value = null;
            return false;
        }
        value = Items[UnityEngine.Random.Range(0, Items.Count)];
        return true;
    }

    public override void Add(GameObject gameObject) {
        var component = gameObject.GetComponent<T>();
        if (component == null)
            throw new Exception($"{gameObject} missing component {typeof(T).FullName}");

        Items.Add(component);
    }
    public override void Remove(GameObject gameObject) => Items.Remove(gameObject.GetComponent<T>());

    void OnEnable() {
        Items.Clear();
    }
}
