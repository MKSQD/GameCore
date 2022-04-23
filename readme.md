# GameCore

Requires the _Addressables_ and _Editor Coroutines_ packages!


## Tools
- Ctrl + G: _Simulate physics for selected GameObjects_
- Ctrl + R: _Replace selected GameObjects_
- Ctrl + Shift + T: _Apply world transform positions to the children of all selected GameObjects and move the selected GameObjects to (0,0,0); This is useful for folder GameObjects_


## Event
Very simple and convinient global, class-based event system. The system is great to decouple UI from logic. 

```cs
public class MyEvent : IEvent {}
public class DeathEvent : IEvent {
    public Actor Player;

    public DeathEvent(Actor player) {
        Player = player;
    }
}

...

EventHub<DeathEvent>.AddListener(OnDeath);
void OnDeath(DeathEvent evt) { ... }

EventHub<DeathEvent>.Emit(new(...));
```

To trigger a global event with a UI _Button_ place a _EventSource_ component on the button, link the Buttons onClick event to _EventSource.Emit_ and setup your event in _EventSource_ inspector (your need to have an IEvent-derived class first). Then add a listener in code.


## RuntimeSet and RuntimeSetEntity
RuntimeSetEntity is a MonoBehaviour that adds and removes a GameObject to a RuntimeSet. RuntimeSet is a generic ScriptableObject set of Components, with TransformRuntimeSet being provided by default.


## SelectImplementation
In combination with SerializeReference this allows for lists of derived objects.

```cs
public interface IEntry {}

public class MyList : ScriptableObject {
    [SerializableReference]
    [SelectImplementation(typeof(IEntry))]
    public IEntry[] Entries;
}
```


## GlobalData
Very simple and convinient global ScriptableObject singleton system.

The system enforces uniform semantics for global data and systems. Derive from the class and open the editor at _Window/Global Data_ where all GlobalData<T> derived classes are listed with their editable instances. No need to create the ScriptableObject asset yourself.

```cs
public class SomeSystemOrData : GlobalData<SomeSystemOrData> {
    public int Data;
}

...

void Start() {
    StartCoroutine(LoadGame());
}

IEnumerator LoadGame() {
    yield return SomeSystemOrData.Load(); // Load data via Addressables
    
    ...

    Debug.Log($"Data: {SomeSystemOrData.Instance.Data}");
}
```


## DebugExt
Proper debug drawing for Unity. Call from everywhere, nothing to setup, stays on the screen when the game is paused, works in editor and game viewports and is toggable via the Gizmos button. 3d shapes, 3d text, 2d viewport lines.

```cs
DebugExt.DrawLine
DebugExt.DrawRect
DebugExt.DrawText
DebugExt.DrawWireSphere
DebugExt.DrawWireCapsule
DebugExt.DrawViewportLine
```