using System.Reflection;
using UnityEngine;

namespace GameCore {
    [AddComponentMenu("GameCore/EventSource")]
    public class EventSource : MonoBehaviour {
        [SerializeReference]
        public IEvent evt;

        public void Emit() {
            if (evt == null) {
                Debug.LogWarning("Event Type not set, can't emit", this);
                return;
            }

            var emit = typeof(EventHub<>)
                .MakeGenericType(evt.GetType())
                .GetMethod("Emit", BindingFlags.Static | BindingFlags.Public);
            emit.Invoke(null, new object[] { evt });
        }
    }
}