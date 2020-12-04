# Save-System-for-Unity
Save System for Unity

## Features
- Super fast in terms of performance. Even simple int saving around 170 times faster than ```PlayerPrefs.SetInt()```. Performance test at the end of README.
- As easy to use as PlayerPrefs
- Can save pretty much everything (Vector, Quaternion, Array, List, Class, Struct, etc). Thanks to [Odin Serializer](https://github.com/TeamSirenix/odin-serializer) for that
- Support multiple profiles
- Save files are encrypted 

## Usage
You need to download this package and [Odin Serializer](https://odininspector.com/download) (Odin Serializer is free, download button at the bottom of page). 

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

## Saving complex data

```csharp
using ToolBox.Serialization;
using UnityEngine;

public class Player : MonoBehaviour
{
	[SerializeField] private float _health = 100;

	private const string SAVE_KEY = "PlayerSaveKey";

	// Loading
	private void Awake()
	{
		var hasKey = DataSerializer.HasKey(SAVE_KEY);

		if (!hasKey)
			return;

		var data = DataSerializer.Load<Data>(SAVE_KEY);
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
