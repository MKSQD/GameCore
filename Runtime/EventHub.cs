using System;
using System.Collections.Generic;
using UnityEngine;

public interface IEvent { }

public static class EventHub<T> where T : struct, IEvent {
    static readonly List<Action<T>> s_listeners = new();

    public static void AddListener(Action<T> listener) => s_listeners.Add(listener);
    public static void RemoveListener(Action<T> listener) => s_listeners.Remove(listener);

    public static void EmitDefault() {
        for (int i = 0; i < s_listeners.Count; ++i) {
            var listener = s_listeners[i];
            try {
                listener(default);
            } catch (Exception e) {
                Debug.LogException(e);
            }
        }
    }

    public static void Emit(T evt) {
        for (int i = 0; i < s_listeners.Count; ++i) {
            var listener = s_listeners[i];
            try {
                listener(evt);
            } catch (Exception e) {
                Debug.LogException(e);
            }
        }
    }
}