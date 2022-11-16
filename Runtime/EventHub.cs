using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GameCore {
    public interface IEvent { }

    public interface IEventListener {
        void OnEvent(IEvent evt);
    }

    public static class EventHub {
        public static IEnumerable<IEventListener> Listeners => _listeners;
        static readonly List<IEventListener> _listeners = new();

        public static void AddListener(IEventListener listener) => _listeners.Add(listener);
        public static void RemoveListener(IEventListener listener) => _listeners.Remove(listener);
    }

    public static class EventHub<T> where T : class, IEvent {
        static readonly List<Action<T>> _handlers = new();

        public static void AddHandler(Action<T> listener) => _handlers.Add(listener);
        public static void RemoveHandler(Action<T> listener) => _handlers.Remove(listener);

        public static void Emit(T evt) {
            Assert.IsNotNull(evt);

            foreach (var listener in EventHub.Listeners) {
                listener.OnEvent(evt);
            }

            for (int i = 0; i < _handlers.Count; ++i) {
                var handler = _handlers[i];
                try {
                    handler(evt);
                } catch (Exception e) {
                    Debug.LogException(e);
                }
            }
        }
    }
}