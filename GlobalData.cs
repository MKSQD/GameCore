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
    public static string Address => $"GlobalData/{typeof(T).Name}";
    public static string AssetName => $"{typeof(T).Name}.asset";
    public static string AssetPath => $"Assets/GlobalData/{AssetName}";

    static T instance;

    public static T Instance {
        get {
#if UNITY_EDITOR
            if (instance == null) {
                if (Application.isPlaying) {
                    Debug.LogError($"Call {typeof(T).Name}.SetupInstance() in code to setup this GlobalData type for runtime access");
                    return null;
                }

                var dirPath = $"{Application.dataPath}/GlobalData";
                if (!Directory.Exists(dirPath)) {
                    Directory.CreateDirectory(dirPath);
                }

                instance = AssetDatabase.LoadAssetAtPath<T>(AssetPath);
                if (instance == null) {
                    instance = CreateAsset();
                }
            }
#endif
            return instance;
        }
    }

    public static bool HasInstance => instance != null;

#if UNITY_EDITOR
    static T CreateAsset() {
        if (File.Exists($"{Application.dataPath}/GlobalData/{AssetName}")) {
            Debug.LogWarning("AssetDatabase.LoadAssetAtPath failed, but file exists. Maybe rebuilding AssetDB?");
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

    public static AsyncOperationHandle<T> LoadInstance() {
        var op = Addressables.LoadAssetAsync<T>(Address);
        op.Completed += ctx => {
            instance = ctx.Result;
#if UNITY_EDITOR
            if (ctx.Status == AsyncOperationStatus.Failed) {
                instance = CreateAsset();
            }
#endif
        };
        return op;
    }
}
