# Save System for Unity

### TODO
- [x] Assets saving/loading
- [ ] Scene References saving/loading


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

### Saving and loading complex data

```csharp
using ToolBox.Serialization;
using UnityEngine;

public class Player : MonoBehaviour
{
	[SerializeField] private float _health = 100f;

	private const string SAVE_KEY = "PlayerSaveKey";

	// Loading
	private void Awake()
	{
		if (!DataSerializer.TryLoad(SAVE_KEY, out Data data))
			return;

		transform.position = data.Position;
		_health = data.Health;
	}

	// Saving
	private void OnApplicationQuit()
	{
		DataSerializer.Save(SAVE_KEY, new Data(transform.position, _health));
	}
}

public struct Data
{
	[SerializeField, HideInInspector] private Vector3 _position;
	[SerializeField, HideInInspector] private float _health;

	public Vector3 Position => _position;
	public float Health => _health;

	public Data(Vector3 position, float health)
	{
		_position = position;
		_health = health;
	}
}
```

### Assets saving

1. Open ```Assets Container``` window. To do that: Window/Assets References

![image](https://user-images.githubusercontent.com/53948684/116908291-26089f00-ac5c-11eb-8bcc-a76489be7aa5.png)

2. Add project folders where lies your assets you want to save. If you want to all assets be able to be saved then just add ```Assets``` folder via ```Select Path``` button.

3. Press ```Load assets from paths``` button.

4. Result:

![image](https://user-images.githubusercontent.com/53948684/116908619-8f88ad80-ac5c-11eb-83ac-f927afc49300.png)

- Everytime you added new asset to your project and you want that asset be able to be saved then you need to repeat process above

- To add more paths just press the ```Add Path``` button then select folder via ```Select Path``` button

- To delete all references you need to press ```Remove assets from container``` button

## Quick example

```csharp
using System.Collections.Generic;
using ToolBox.Serialization;
using UnityEngine;

public class Player : MonoBehaviour
{
	[SerializeField] private List<Item> _inventory = null;

	private const string SAVE_KEY = "PlayerItems";

	private void Awake()
	{
		DataSerializer.FileSaving += FileSaving;
	
		if (DataSerializer.TryLoad<List<Item>>(SAVE_KEY, out var items))
		{
			_inventory = items;
			return;
		}

		_inventory = new List<Item>();
	}

	// Will be called before game quits
	private void FileSaving()
	{
		DataSerializer.Save(SAVE_KEY, _inventory);
	}
}

[CreateAssetMenu]
public class Item : ScriptableObject
{
	[SerializeField] private string _name = string.Empty;
	[SerializeField] private int _cost = 100;
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
