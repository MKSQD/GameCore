Very simple and convinient global, class-based event system.

```
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

The system is great to decouple UI from logic. To trigger a global event with a UI _Button_ place a _EventSource_ component on the button, link the Buttons onClick event to _EventSource.Emit_ and setup your event in _EventSource_ inspector (your need to have an IEvent-derived class first). Then add a listener in code.