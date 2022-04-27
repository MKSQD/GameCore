using UnityEngine;

[AddComponentMenu("RuntimeSetEntity")]
public class RuntimeSetEntity : MonoBehaviour {
    public RuntimeSet Set;

    protected void OnEnable() => Set.Add(gameObject);
    protected void OnDisable() => Set.Remove(gameObject);

#if UNITY_EDITOR
    protected void OnDrawGizmos() {
        if (Set != null) {
            Set.DrawGizmos(gameObject);
        }
    }
#endif
}
