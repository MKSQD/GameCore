using UnityEngine;

[AddComponentMenu("RuntimeSetEntity")]
public class RuntimeSetEntity : MonoBehaviour {
    public RuntimeSet Set;

    void OnEnable() => Set.Add(gameObject);
    void OnDisable() => Set.Remove(gameObject);

#if UNITY_EDITOR
    void OnDrawGizmos() {
        if (Set != null) {
            Set.DrawGizmos(gameObject);
        }
    }
#endif
}
