using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public interface IGlobalData {
}

public class GlobalData<T> : ScriptableObject, IGlobalData where T : ScriptableObject {
    static T instance;

    static string address {
        get => $"GlobalData/{typeof(T).Name}";
    }

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

                var path = $"Assets/GlobalData/{typeof(T).Name}.asset";
                instance = AssetDatabase.LoadAssetAtPath<T>(path);

                if (instance == null) {
                    var newInstance = ScriptableObject.CreateInstance<T>();

                    AssetDatabase.CreateAsset(newInstance, path);
                    var guid = AssetDatabase.AssetPathToGUID(path);

                    var settings = AddressableAssetSettingsDefaultObject.Settings;
                    var entry = settings.CreateOrMoveEntry(guid, settings.DefaultGroup, postEvent: false);
                    entry.SetAddress(address);

                    settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, new List<AddressableAssetEntry>() { entry }, true);

                    instance = newInstance;
                }
            }
#endif
            return instance;
        }
    }

    public static AsyncOperationHandle<T> SetupInstance() {
        var op = Addressables.LoadAssetAsync<T>(address);
        op.Completed += ctx => {
            instance = ctx.Result;
        };
        return op;
    }
}
