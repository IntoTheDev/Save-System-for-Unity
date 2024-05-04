# Save System for Unity

### TODO
- [x] Assets References
- [x] AOT support
- [x] Upload MessagePack version to separate branch? MessagePack version has better performance and less file size (WIP)


## Features
- Can save pretty much everything (Vector, Quaternion, Array, List, Class, Struct, etc)
- Can save assets references
- As easy to use as PlayerPrefs
- Fast in terms of performance. Even simple int saving way more faster than ```PlayerPrefs.SetInt()```. Performance test at the end of README
- Save files are encrypted
- Multiple profiles

## How to Install

### Git Installation (Best way to get latest version)

If you have Git on your computer, you can open Package Manager indside Unity, select "Add package from Git url...", and paste link ```https://github.com/IntoTheDev/Save-System-for-Unity.git```

or

Open the manifest.json file of your Unity project.
Add ```"com.intothedev.savesystem": "https://github.com/IntoTheDev/Save-System-for-Unity.git"```

### Manual Installation (Version can be outdated)
Download latest package from the Release section.
Import SaveSystem.unitypackage to your Unity Project

## Usage

### Saving

```csharp
float health = 100f;
DataSerializer.Save("SaveKeyHere", health);
```

### Loading

```csharp
float health = DataSerializer.Load<float>("SaveKeyHere");
```

### Check for key

```csharp
if (DataSerializer.HasKey("SaveKeyHere"))
	float health = DataSerializer.Load<float>("SaveKeyHere");
	
// OR

float health;

if (DataSerializer.TryLoad("SaveKeyHere", out float value))
	health = value;
```

### Delete key

```csharp
DataSerializer.DeleteKey("SaveKeyHere");
```

### Delete all save file data

```csharp
DataSerializer.DeleteAll();
```

### Change profile. Old profile will be saved and new one is loaded.

```csharp
DataSerializer.ChangeProfile(profileIndex: 1);
```

### Complex example

```csharp
using System.Collections.Generic;
using ToolBox.Serialization;
using UnityEngine;

public class Player : MonoBehaviour
{
	[SerializeField] private float _health = 0f;
	[SerializeField] private List<Item> _inventory = new List<Item>();

	private const string SAVE_KEY = "PlayerSaveData";

	private void Awake()
	{
		DataSerializer.FileSaving += FileSaving;

		if (DataSerializer.TryLoad<SaveData>(SAVE_KEY, out var loadedData))
		{
			_health = loadedData.Health;
			transform.position = loadedData.Position;
			_inventory = loadedData.Inventory;
		}
	}

	// This method will be called before application quits
	private void FileSaving()
	{
		DataSerializer.Save(SAVE_KEY, new SaveData(_health, transform.position, _inventory));
	}
}

// If you want to make scriptable object or any other asset saveable then you need to add that asset to Assets Container. 
// See guide below
[CreateAssetMenu]
public class Item : ScriptableObject
{
	[SerializeField] private string _name = string.Empty;
	[SerializeField] private int _cost = 100;
}

public struct SaveData
{
    [SerializeField] private float _health;
    [SerializeField] private Vector3 _position;
    [SerializeField] private List<Item> _inventory;

    public float Health => _health;
    public Vector3 Position => _position;
    public List<Item> Inventory => _inventory;
    
    public SaveData(float health, Vector3 position, List<Item> inventory)
    {
        _health = health;
        _position = position;
        _inventory = inventory;
    }
}
```

### How to make asset saveable

1. Open ```Assets Container``` window. Window/Assets Container. Window looks like this:

![image](https://user-images.githubusercontent.com/53948684/117006513-f7dd9a80-ad01-11eb-8c14-bd665a88dfe2.png)

2. Select path where your assets stored. If you already have path field then press the ```Select Path``` button or ```Add Path``` if not. In my case path is ```Assets/ScriptableObjects/Items```.

3. Press the ```Load assets at paths``` button.

4. Repeat step 3 every time you create a new asset.

![image](https://github.com/IntoTheDev/Save-System-for-Unity/assets/53948684/10e575a2-a4f6-4693-98c3-1e04dca618ec)

### AOT platforms

AOT (IL2CPP) works without any additional work, but IF some types do NOT serialize, please follow the steps below.

You need to create a simple C# class that implements the ```ITypeProvider``` interface. Then, you need to define the types that fail to save for some reason.

Although it should work without an ITypeProvider, for the sake of simplicity, I'll use the case above as an example.

```csharp
using System;
using System.Collections.Generic;
using ToolBox.Serialization;
using UnityEngine;

public sealed class TestProvider : ITypeProvider
{
    public Type[] GetTypes()
    {
        return new Type[]
        {
            typeof(SaveData),
            typeof(Vector3),
            typeof(List<Item>)
        };
    }
}

```

## Performance test

### PlayerPrefs result: 329 milliseconds
### Code:

```csharp
using Sirenix.OdinInspector;
using System.Diagnostics;
using UnityEngine;

public class Tester : MonoBehaviour
{
	[SerializeField] private int _number = 0;

	[Button]
	private void Test()
	{
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();

		for (int i = 0; i < 10000; i++)
		{
			PlayerPrefs.SetInt("SAVE", _number);
			_number = PlayerPrefs.GetInt("SAVE");
		}

		stopwatch.Stop();
		print(stopwatch.Elapsed.TotalMilliseconds);
	}
}

```

### DataSerializer result: 18 milliseconds
### Code:

```csharp
using Sirenix.OdinInspector;
using System.Diagnostics;
using ToolBox.Serialization;
using UnityEngine;

public class Tester : MonoBehaviour
{
	[SerializeField] private int _number = 0;

	[Button]
	private void Test()
	{
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();

		for (int i = 0; i < 10000; i++)
		{
			DataSerializer.Save("SAVE", _number);
			_number = DataSerializer.Load<int>("SAVE");
		}

		stopwatch.Stop();
		print(stopwatch.Elapsed.TotalMilliseconds);
	}
}
```

## License
[DataSerializer](https://github.com/IntoTheDev/Save-System-for-Unity/tree/master/DataSerializer) folder is licensed under the MIT License, see [LICENSE](https://github.com/IntoTheDev/Save-System-for-Unity/blob/master/DataSerializer/LICENSE) for more information.

[OdinSerializer](https://github.com/IntoTheDev/Save-System-for-Unity/tree/master/OdinSerializer) folder is licensed under the Apache-2.0 License, see [LICENSE](https://github.com/IntoTheDev/Save-System-for-Unity/blob/master/OdinSerializer/LICENSE) for more information. Odin Serializer belongs to [Team Sirenix](https://github.com/TeamSirenix)
