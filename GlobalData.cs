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
    static T instance;
    static string address => $"GlobalData/{typeof(T).Name}";
    static string assetPath => $"Assets/GlobalData/{typeof(T).Name}.asset";

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

                instance = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (instance == null) {
                    instance = CreateAsset();
                }
            }
#endif
            return instance;
        }
    }

#if UNITY_EDITOR
    static T CreateAsset() {
        var newInstance = ScriptableObject.CreateInstance<T>();

        AssetDatabase.CreateAsset(newInstance, assetPath);
        var guid = AssetDatabase.AssetPathToGUID(assetPath);

        var settings = AddressableAssetSettingsDefaultObject.Settings;
        var entry = settings.CreateOrMoveEntry(guid, settings.DefaultGroup, postEvent: false);
        entry.SetAddress(address);

        settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, new List<AddressableAssetEntry>() { entry }, true);

        return newInstance;
    }
#endif

    public static AsyncOperationHandle<T> LoadInstance() {
        var op = Addressables.LoadAssetAsync<T>(address);
        op.Completed += ctx => {
            instance = ctx.Result;
            Debug.Log($"Loaded <i>{typeof(T).Name}</i>");
#if UNITY_EDITOR
            if (ctx.Status == AsyncOperationStatus.Failed) {
                instance = CreateAsset();
            }
#endif
        };
        return op;
    }
}
