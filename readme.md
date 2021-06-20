# GameCore

Requires the Addressables package!

## Tools
- Ctrl + G: _Simulate physics for selected GameObjects_
- Ctrl + R: _Replace selected GameObjects_
- Ctrl + Shift + T: _Apply world transform positions to the children of all selected GameObjects and move the selected GameObjects to (0,0,0); This is useful for folder GameObjects_

## Event
Very simple and convinient global, class-based event system.

The system is great to decouple UI from logic. To trigger a global event with a UI _Button_ place a _EventSource_ component on the button, link the Buttons onClick event to _EventSource.Emit_ and setup your event in _EventSource_ inspector (your need to have an IEvent-derived class first). Then add a listener in code.

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

EventHub<DeathEvent>.Emit(new DeathEvent(...));
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
    yield return SomeSystemOrData.LoadInstance(); // Load data via Addressables
    
    ...

    Debug.Log($"Data: {SomeSystemOrData.Instance.Data}");
}
```