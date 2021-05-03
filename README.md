# Save System for Unity

### TODO
- [x] Assets saving/loading
- [ ] Scene References saving/loading


## Features
- Super fast in terms of performance. Even simple int saving way more faster than ```PlayerPrefs.SetInt()```. Performance test at the end of README
- As easy to use as PlayerPrefs
- Can save pretty much everything (Vector, Quaternion, Array, List, Class, Struct, etc)
- Can save assets references
- Multiple profiles
- Save files are encrypted

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

## Saving and loading complex data

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

// Use struct or if you're using class for some reason then make a new instance each time you're saving
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

## Performance test

### PlayerPrefs result: 225.8494 milliseconds
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

### DataSerializer result: 1.3 milliseconds
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
