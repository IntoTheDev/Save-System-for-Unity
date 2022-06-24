# Save System for Unity

### TODO
- [x] Assets References
- [x] AOT support


## Features
- Can save pretty much everything (Vector, Quaternion, Array, List, Class, Struct, etc)
- Can save assets references
- As easy to use as PlayerPrefs
- Fast in terms of performance. Even simple int saving way more faster than ```PlayerPrefs.SetInt()```. Performance test at the end of README
- Save files are encrypted
- Extensible

## How to Install

### Requirements
[MessagePack](https://github.com/neuecc/MessagePack-CSharp)


### Git Installation (Best way to get latest version)

If you have Git on your computer, you can open Package Manager indside Unity, select "Add package from Git url...", and paste link ```https://github.com/IntoTheDev/Save-System-for-Unity.git```

or

Open the manifest.json file of your Unity project.
Add ```"com.intothedev.savesystem": "https://github.com/IntoTheDev/Save-System-for-Unity.git"```

### Manual Installation (Version can be outdated)
Download latest package from the Release section.
Import SaveSystem.unitypackage into your Unity Project


## API Example
```csharp
[CreateAssetMenu, MessagePackFormatter(typeof(AssetFormatter<Item>))]
public class Item : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private int _cost;
}

[MessagePackObject, Serializable]
public class PlayerData
{
    [Key(0), SerializeField] private string _name;
    [Key(1), SerializeField] private string _level;
    [Key(2), SerializeField] private List<Item> _inventory;
    [Key(3), SerializeField, MessagePackFormatter(typeof(AssetFormatter<Sprite>))] private Sprite _icon;
}

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerData _heroData;

    private void Awake()
    {
        // There's also async version
        DataSerializer.LoadFile(fileName: "Save");
    }

    private void OnApplicationQuit()
    {
        // There's also async version
        DataSerializer.SaveFile(fileName: "Save");
    }

    [Button]
    private void Save()
    {
        DataSerializer.Save(key: "PlayerData", _heroData);
    }

    [Button]
    private void Load()
    {
        _heroData = DataSerializer.Load<PlayerData>(key: "PlayerData");
    }
}
```

### How to make asset saveable

1. Open ```Assets Container``` window. Window/Assets Container. Window looks like this:

![image](https://user-images.githubusercontent.com/53948684/117006513-f7dd9a80-ad01-11eb-8c14-bd665a88dfe2.png)

2. Select path where your assets stored. If you already have path field then press the ```Select Path``` button or ```Add Path``` if not. In my case path is ```Assets/ScriptableObjects/Items```.

3. Press the ```Load assets at paths``` button.

![image](https://user-images.githubusercontent.com/53948684/117006947-776b6980-ad02-11eb-997c-e9108e5c3f97.png)

4. Specify formatter for a field or a class itself like in the API example

### AOT platforms

[See](https://github.com/neuecc/MessagePack-CSharp#aot-code-generation-support-for-unityxamarin)

## Performance test

### PlayerPrefs result: 00:00:00.0429685
### Code:

```csharp
using Sirenix.OdinInspector;
using System.Diagnostics;
using UnityEngine;

public class Tester : MonoBehaviour
{
    [SerializeField] private int _number;

    [Button]
    private void Test()
    {
        var stopwatch = Stopwatch.StartNew();
        stopwatch.Start();

        for (var i = 0; i < 10000; i++)
        {
            PlayerPrefs.SetInt("SAVE", _number);
            _number = PlayerPrefs.GetInt("SAVE");
        }

        stopwatch.Stop();
        print(stopwatch.Elapsed.ToString());
    }
}
```

### DataSerializer result: 00:00:00.0035277
### Code:

```csharp
using Sirenix.OdinInspector;
using System.Diagnostics;
using MessagePack;
using ToolBox.Serialization;
using UnityEngine;

public class Tester : MonoBehaviour
{
    [SerializeField, Key(0)] private int _number;

    [Button]
    private void Test()
    {
        var stopwatch = Stopwatch.StartNew();
        stopwatch.Start();

        for (var i = 0; i < 10000; i++)
        {
            DataSerializer.Save("SAVE", _number);
            _number = DataSerializer.Load<int>("SAVE");
        }

        stopwatch.Stop();
        print(stopwatch.Elapsed.ToString());
    }
}
```
