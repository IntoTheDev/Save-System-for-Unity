# Save-System-for-Unity
Save System for Unity with Json and basic encryption also supports arrays

## Usage
```csharp
using ToolBox.Serialization;
using UnityEngine;

public class Test : MonoBehaviour
{
	private void Start()
	{
		// File name
		string serializationKey = "UserData";

		// Saving data
		User user = new User
		{
			Name = "Name",
			Age = 20
		};

		Serializer.Save(user, serializationKey);

		// Loading data
		User newUser = default;

		newUser = Serializer.Load(newUser, serializationKey);
	}
}

[System.Serializable]
public struct User : ISerializableData
{
	public string Name;
	public int Age;
}
```
