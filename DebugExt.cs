using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DebugExt : MonoBehaviour {
    struct LineData {
        public Vector3 from, to;
        public Color color;
        public float removeTime;
    }
    struct StringData {
        public Vector3 pos;
        public string text;
        public Color color;
        public float removeTime;
    }
    struct WireSphereData {
        public Vector3 pos;
        public float radius;
        public Color color;
        public float removeTime;
    }
    struct WireCapsuleData {
        public Vector3 pos0, pos1;
        public float radius;
        public Color color;
        public float removeTime;
    }

    List<LineData> _lines = new(), _lines2 = new();
    List<StringData> _strings = new(), _strings2 = new();
    List<WireSphereData> _wireSpheres = new(), _wireSpheres2 = new();
    List<WireCapsuleData> _wireCapsules = new(), _wireCapsules2 = new();

    static DebugExt instance;
    public static DebugExt Instance {
        get {
            EnsureInstance();
            return instance;
        }
    }

    static void EnsureInstance() {
        if (instance != null)
            return;

        var wrapper = GameObject.Find("DebugExt Wrapper");

        instance = wrapper?.GetComponentInChildren<DebugExt>();
        if (instance == null) {
            DestroyImmediate(wrapper);

            var goOuter = new GameObject("DebugExt Wrapper") {
                hideFlags = HideFlags.HideAndDontSave
            };

            var go = new GameObject("DebugExt");
            go.transform.parent = goOuter.transform;

            instance = go.AddComponent<DebugExt>();

#if UNITY_EDITOR
            EditorApplication.pauseStateChanged += state => paused = state == PauseState.Paused;
#endif
        }
    }


#if UNITY_EDITOR
    static bool paused = false;
#endif

    public static void DrawRect(Vector3 point, Vector3 normal, Color color, float duration = 0) {
        EnsureInstance();
        if (instance._lines.Count > 300)
            return;

        var p0 = point + Vector3.Cross(normal, new Vector3(1, 0, 1));
        var p1 = point + Vector3.Cross(normal, new Vector3(1, 0, -1));
        var p2 = point + Vector3.Cross(normal, new Vector3(-1, 0, -1));
        var p3 = point + Vector3.Cross(normal, new Vector3(-1, 0, 1));

        DrawLineImpl(p0, p1, color, duration);
        DrawLineImpl(p1, p2, color, duration);
        DrawLineImpl(p2, p3, color, duration);
        DrawLineImpl(p3, p0, color, duration);
    }



    public static void DrawLine(Vector3 from, Vector3 to) {
        EnsureInstance();
        if (instance._lines.Count > 300)
            return;

        DrawLineImpl(from, to, Color.green, 0);
    }

    public static void DrawLine(Vector3 from, Vector3 to, Color color) {
        EnsureInstance();
        if (instance._lines.Count > 300)
            return;

        DrawLineImpl(from, to, color, 0);
    }

    public static void DrawLine(Vector3 from, Vector3 to, Color color, float duration) {
        EnsureInstance();
        if (instance._lines.Count > 300)
            return;

        DrawLineImpl(from, to, color, duration);
    }

    static void DrawLineImpl(Vector3 from, Vector3 to, Color color, float duration) {
        instance._lines.Add(new LineData() {
            from = from,
            to = to,
            color = color,
            removeTime = Time.time + duration
        });
    }



    public static void DrawText(Vector3 pos, string text) {
        EnsureInstance();
        if (instance._strings.Count > 100)
            return;

        DrawTextImpl(pos, text, Color.green, 0);
    }

    public static void DrawText(Vector3 pos, string text, Color color) {
        EnsureInstance();
        if (instance._strings.Count > 100)
            return;

        DrawTextImpl(pos, text, color, 0);
    }

    public static void DrawText(Vector3 pos, string text, Color color, float duration) {
        EnsureInstance();
        if (instance._strings.Count > 100)
            return;

        DrawTextImpl(pos, text, color, duration);
    }

    static void DrawTextImpl(Vector3 pos, string text, Color color, float duration) {
        instance._strings.Add(new StringData() {
            text = text,
            color = color,
            pos = pos,
            removeTime = Time.time + duration
        });
    }



    public static void DrawWireSphere(Vector3 pos, float radius, Color? color = null, float duration = 0) {
        EnsureInstance();
        if (instance._wireSpheres.Count > 200)
            return;

        instance._wireSpheres.Add(new WireSphereData() {
            color = color ?? Color.green,
            radius = radius,
            pos = pos,
            removeTime = Time.time + duration
        });
    }



    public static void DrawWireCapsule(Vector3 pos0, Vector3 pos1, float radius, Color? color = null, float duration = 0) {
        instance._wireCapsules.Add(new WireCapsuleData() {
            color = color ?? Color.green,
            radius = radius,
            pos0 = pos0,
            pos1 = pos1,
            removeTime = Time.time + duration
        });
    }



    static Material lineMaterial;
    static void CreateLineMaterial() {
        if (!lineMaterial) {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    public static void DrawViewportLine(Vector2 pos0, Vector2 pos1, Color? color = null) {
        Camera camera = Camera.main;
        if (camera == null)
            return;

        instance.StartCoroutine(DrawViewportLineImpl(pos0, pos1, color ?? Color.green));
    }

    static IEnumerator DrawViewportLineImpl(Vector2 pos0, Vector2 pos1, Color color) {
        yield return new WaitForEndOfFrame();

        CreateLineMaterial();
        GL.PushMatrix();
        lineMaterial.SetPass(0);
        GL.LoadOrtho();
        GL.Begin(GL.LINES);
        GL.Color(color);

        GL.Vertex(pos0);
        GL.Vertex(pos1);

        GL.End();
        GL.PopMatrix();
    }

#if UNITY_EDITOR
    void OnDrawGizmos() {
        _lines2.Clear();
        _strings2.Clear();
        _wireSpheres2.Clear();
        _wireCapsules2.Clear();

        foreach (var data in _lines) {
            Gizmos.color = data.color;
            Gizmos.DrawLine(data.from, data.to);
            if (!paused && Time.time < data.removeTime) {
                _lines2.Add(data);
            }
        }

        GUIStyle style = new GUIStyle();
        foreach (var data in _strings) {
            style.normal.textColor = data.color;
            Handles.Label(data.pos, data.text, style);
            if (!paused && Time.time < data.removeTime) {
                _strings2.Add(data);
            }
        }

        foreach (var data in _wireSpheres) {
            Gizmos.color = data.color;
            Gizmos.DrawWireSphere(data.pos, data.radius);
            if (!paused && Time.time < data.removeTime) {
                _wireSpheres2.Add(data);
            }
        }

        Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
        foreach (var data in _wireCapsules) {
            var diff = data.pos1 - data.pos0;

            Handles.color = data.color;

            Matrix4x4 angleMatrix = Matrix4x4.TRS(data.pos0 + diff * 0.5f, Quaternion.FromToRotation(Vector3.up, diff), Handles.matrix.lossyScale);
            using (new Handles.DrawingScope(angleMatrix)) {
                var pointOffset = diff.magnitude / 2;

                //draw sideways
                Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.left, Vector3.back, -180, data.radius);
                Handles.DrawLine(new Vector3(0, pointOffset, -data.radius), new Vector3(0, -pointOffset, -data.radius));
                Handles.DrawLine(new Vector3(0, pointOffset, data.radius), new Vector3(0, -pointOffset, data.radius));
                Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.left, Vector3.back, 180, data.radius);
                //draw frontways
                Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.back, Vector3.left, 180, data.radius);
                Handles.DrawLine(new Vector3(-data.radius, pointOffset, 0), new Vector3(-data.radius, -pointOffset, 0));
                Handles.DrawLine(new Vector3(data.radius, pointOffset, 0), new Vector3(data.radius, -pointOffset, 0));
                Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.back, Vector3.left, -180, data.radius);
                //draw center
                Handles.DrawWireDisc(Vector3.up * pointOffset, Vector3.up, data.radius);
                Handles.DrawWireDisc(Vector3.down * pointOffset, Vector3.up, data.radius);
            }

            if (!paused && Time.time < data.removeTime) {
                _wireCapsules2.Add(data);
            }
        }

        if (!paused) {
            (_lines, _lines2) = (_lines2, _lines);
            (_strings, _strings2) = (_strings2, _strings);
            (_wireSpheres, _wireSpheres2) = (_wireSpheres2, _wireSpheres);
            (_wireCapsules, _wireCapsules2) = (_wireCapsules2, _wireCapsules);
        }
    }
#endif
}