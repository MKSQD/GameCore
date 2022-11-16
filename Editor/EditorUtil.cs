using UnityEngine;

public static class EditorUtil {
    public static Texture2D CreateSingleColorTexture(Color col) {
        var pix = new Color[1];

        for (int i = 0; i < pix.Length; i++) {
            pix[i] = col;
        }

        var result = new Texture2D(1, 1);
        result.SetPixels(pix);
        result.Apply();

        return result;
    }
}
