# Save-System-for-Unity
Save System for Unity

## Features
- Super fast in terms of performance
- Can save pretty much everything (Thanks to [Odin Serializer](https://github.com/TeamSirenix/odin-serializer) for that)
- Saving and loading file is automatically
- Support multiple profiles
- Save files are encrypted 

## Usage
You need to download this package and [Odin Serializer](https://odininspector.com/download) (Odin Serializer is free, download button at the bottom of the page). For saving/loading you need to create class/struct and inherit ISerializable interface.

```csharp
using ToolBox.Serialization;
using UnityEngine;

public class Player : MonoBehaviour
{
	private float _health = 100f;

	private const string SAVE_KEY = "PlayerData";

	// Loading
	private void Awake()
	{
		var hasData = DataSerializer.TryLoad<PlayerData>(SAVE_KEY, out var data);

		if (!hasData)
			return;

		transform.position = data.Position;
		_health = data.Health;
	}

	// Saving
	private void OnApplicationQuit()
	{
		DataSerializer.Save(SAVE_KEY, new PlayerData(transform.position, _health));
	}
}

public struct PlayerData : ISerializable
{
	[SerializeField, HideInInspector] private Vector3 _position;
	[SerializeField, HideInInspector] private float _health;

	public Vector3 Position => _position;
	public float Health => _health;

	public PlayerData(Vector3 position, float health)
	{
		_position = position;
		_health = health;
	}
}
```
