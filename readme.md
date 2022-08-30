# GameCore
Simple things every Unity project needs. They will get you a long way on solo projects without magic, frameworks and magic.
This is the bare minimum of decoupling required to 

Requires the _Addressables_ and _Editor Coroutines_ packages!


## Tools
- Ctrl + G: _Simulate physics for selected GameObjects_
- Ctrl + R: _Replace selected GameObjects_
- Ctrl + Shift + T: _Apply world transform positions to the children of all selected GameObjects and move the selected GameObjects to (0,0,0); This is useful for folder GameObjects_


## Event
Simple global, type based event system. The system is great to decouple UI from logic. 

```cs
public struct MyEvent : IEvent {}
public struct DeathEvent : IEvent {
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


# Service
Simple global, type based service system. This basically allows 2 things over the usual global static reference: bind to an interface (as shown in the example) and have more consistent usage and lifetime.

```cs
public interface ISearchService : IService { }

public class GoogleSearchService : ISearchService {}
public class DuckDuckGoSearchService : ISearchService {}

...

ServiceHub<ISearchService>.Bind(new GoogleSearchService());
void OnDeath(DeathEvent evt) { ... }

...

ServiceHub<ISearchService>.Instance
```

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