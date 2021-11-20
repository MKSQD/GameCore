using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventHub<T> where T : IEvent {
    static List<Action<T>> listeners = new List<Action<T>>();

    public static void AddListener(Action<T> listener) {
        listeners.Add(listener);
    }

    public static void RemoveListener(Action<T> listener) {
        listeners.Remove(listener);
    }

    public static void EmitEmpty() {
        for (int i = 0; i < listeners.Count; ++i) {
            var listener = listeners[i];
            try {
                listener(default);
            } catch (Exception e) {
                Debug.LogException(e);
            }
        }
    }

    public static void Emit(T evt) {
        for (int i = 0; i < listeners.Count; ++i) {
            var listener = listeners[i];
            try {
                listener(evt);
            } catch (Exception e) {
                Debug.LogException(e);
            }
        }
    }
}