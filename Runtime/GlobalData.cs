using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
#endif
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public interface IGlobalData {
}

public class GlobalData<T> : ScriptableObject, IGlobalData where T : ScriptableObject {
    public static string Address => $"GlobalData_{typeof(T).Name}";
    public static string AssetName => $"{typeof(T).Name}.asset";
    public static string AssetPath => $"Assets/GlobalData/{AssetName}";

    static T s_instance;

    public static T Instance {
        get {
#if UNITY_EDITOR
            if (s_instance == null) {
                if (Application.isPlaying)
                    return Load().WaitForCompletion();

                var dirPath = $"{Application.dataPath}/GlobalData";
                if (!Directory.Exists(dirPath)) {
                    Directory.CreateDirectory(dirPath);
                }

                s_instance = AssetDatabase.LoadAssetAtPath<T>(AssetPath);
                if (s_instance == null) {
                    s_instance = CreateAsset();
                }
            }
#endif
            return s_instance;
        }
    }

    public static bool HasInstance => s_instance != null;

    public static AsyncOperationHandle<T> Load() {
        var op = Addressables.LoadAssetAsync<T>(Address);
        op.Completed += ctx => {
            s_instance = ctx.Result;
#if UNITY_EDITOR
            if (ctx.Status == AsyncOperationStatus.Failed) {
                s_instance = CreateAsset();
            }
#endif
        };
        return op;
    }

#if UNITY_EDITOR
    static T CreateAsset() {
        if (File.Exists($"{Application.dataPath}/GlobalData/{AssetName}")) {
            Debug.LogWarning("AssetDatabase.LoadAssetAtPath failed, but the file exists. Maybe rebuilding AssetDB? Maybe Addressable name was changed (don't do that!)?");
            return null;
        }

        Debug.Log($"Creating GlobalData at {AssetPath}...");

        var newInstance = ScriptableObject.CreateInstance<T>();

        AssetDatabase.CreateAsset(newInstance, AssetPath);
        var guid = AssetDatabase.AssetPathToGUID(AssetPath);

        var settings = AddressableAssetSettingsDefaultObject.Settings;
        var entry = settings.CreateOrMoveEntry(guid, settings.DefaultGroup, postEvent: false);
        entry.SetAddress(Address);

        settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, new List<AddressableAssetEntry>() { entry }, true);

        return newInstance;
    }
#endif
}
