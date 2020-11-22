# Save-System-for-Unity
Save System for Unity

## Features
- Super fast in terms of performance
- Can save pretty much everything (Thanks to [Odin Serializer](https://github.com/TeamSirenix/odin-serializer) for that)

## Usage
You need to download this package and [Odin Serializer](https://github.com/TeamSirenix/odin-serializer). That's all.

```csharp
using ToolBox.Serialization;
using UnityEngine;

public class Player : MonoBehaviour
{
	[SerializeField] private float _health = 100f;

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
