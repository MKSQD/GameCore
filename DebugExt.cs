using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DebugExt : MonoBehaviour {
    public class LineData {
        public Vector3 from, to;
        public Color color;
        public float removeTime;
    }
    public class StringData {
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

    List<LineData> lines = new List<LineData>(), lines2 = new List<LineData>();
    List<StringData> strings = new List<StringData>(), strings2 = new List<StringData>();
    List<WireSphereData> wireSpheres = new List<WireSphereData>(), wireSpheres2 = new List<WireSphereData>();

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
            Destroy(wrapper);

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

    public static void DrawRect(Vector3 from, Vector3 to, Color color, float duration = 0) {
        EnsureInstance();
        if (instance.lines.Count > 300)
            return;

        DrawLineImpl(new Vector3(from.x, from.y, from.z), new Vector3(to.x, from.y, from.z), color, duration);
        DrawLineImpl(new Vector3(to.x, from.y, from.z), new Vector3(to.x, to.y, to.z), color, duration);
        DrawLineImpl(new Vector3(to.x, to.y, to.z), new Vector3(from.x, from.y, to.z), color, duration);
        DrawLineImpl(new Vector3(from.x, from.y, to.z), new Vector3(from.x, from.y, from.z), color, duration);
    }



    public static void DrawLine(Vector3 from, Vector3 to) {
        EnsureInstance();
        if (instance.lines.Count > 300)
            return;

        DrawLineImpl(from, to, Color.green, 0);
    }

    public static void DrawLine(Vector3 from, Vector3 to, Color color) {
        EnsureInstance();
        if (instance.lines.Count > 300)
            return;

        DrawLineImpl(from, to, color, 0);
    }

    public static void DrawLine(Vector3 from, Vector3 to, Color color, float duration) {
        EnsureInstance();
        if (instance.lines.Count > 300)
            return;

        DrawLineImpl(from, to, color, duration);
    }

    static void DrawLineImpl(Vector3 from, Vector3 to, Color color, float duration) {
        instance.lines.Add(new LineData() {
            from = from,
            to = to,
            color = color,
            removeTime = Time.time + duration
        });
    }



    public static void DrawText(Vector3 pos, string text) {
        EnsureInstance();
        if (instance.strings.Count > 100)
            return;

        DrawTextImpl(pos, text, Color.green, 0);
    }

    public static void DrawText(Vector3 pos, string text, Color color) {
        EnsureInstance();
        if (instance.strings.Count > 100)
            return;

        DrawTextImpl(pos, text, color, 0);
    }

    public static void DrawText(Vector3 pos, string text, Color color, float duration) {
        EnsureInstance();
        if (instance.strings.Count > 100)
            return;

        DrawTextImpl(pos, text, color, duration);
    }

    public static void DrawTextImpl(Vector3 pos, string text, Color color, float duration) {
        instance.strings.Add(new StringData() {
            text = text,
            color = color,
            pos = pos,
            removeTime = Time.time + duration
        });
    }



    public static void DrawWireSphere(Vector3 pos, float radius, Color? color = null, float duration = 0) {
        EnsureInstance();
        if (instance.wireSpheres.Count > 200)
            return;

        instance.wireSpheres.Add(new WireSphereData() {
            color = color ?? Color.green,
            radius = radius,
            pos = pos,
            removeTime = Time.time + duration
        });
    }



    public static void DrawWireCapsule(Vector3 pos0, Vector3 pos1, float radius, Color? color = null, float duration = 0) {
        var dist = 1 / (0.1f + (pos0 - pos1).magnitude * 2);
        for (float f = 0; f < 1; f += dist) {
            var p = Vector3.Lerp(pos0, pos1, f);
            DrawWireSphere(p, radius, color, duration);
        }
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
        lines2.Clear();
        strings2.Clear();
        wireSpheres2.Clear();

        foreach (var data in lines) {
            Gizmos.color = data.color;
            Gizmos.DrawLine(data.from, data.to);
            if (Time.time < data.removeTime) {
                lines2.Add(data);
            }
        }

        GUIStyle style = new GUIStyle();
        foreach (var data in strings) {
            style.normal.textColor = data.color;
            Handles.Label(data.pos, data.text, style);
            if (Time.time < data.removeTime) {
                strings2.Add(data);
            }
        }

        foreach (var data in wireSpheres) {
            Gizmos.color = data.color;
            Gizmos.DrawWireSphere(data.pos, data.radius);
            if (Time.time < data.removeTime) {
                wireSpheres2.Add(data);
            }
        }

        if (!paused) {
            (lines, lines2) = (lines2, lines);
            (strings, strings2) = (strings2, strings);
            (wireSpheres, wireSpheres2) = (wireSpheres2, wireSpheres);
        }
    }
#endif
}