Very simple and convinient global ScriptableObject singleton system.

```
public class SomeSystemOrData : GlobalData<SomeSystemOrData> {
    public int Data;
}

...

void Start() {
    StartCoroutine(LoadGame());
}

IEnumerator LoadGame() {
    yield return SomeSystemOrData.LoadInstance(); // Load data via Addressibles
    
    ...

    Debug.Log($"Data: {SomeSystemOrData.Instance.Data}");
}
```

The system enforces uniform semantic for global data and systems. Derive from the class and open the editor at _Window/Global Data_ where all GlobalData<T> derived classes are listed with their editable instances. No need to create the ScriptableObject asset yourself.
