# Save-System-for-Unity
Save System for Unity with Json and basic encryption also supports arrays

## Usage
```csharp
using UnityEngine;
using ToolBox.Serialization;

public class Test : MonoBehaviour
{
	private void Start()
	{
		// File name
		string serializationKey = "UserDataSecond";

		// Saving data
		User user = new User
		{
			Name = "NAME",
			Age = 20
		};

		Serializer.Save(user, serializationKey);

		// Loading data
		User newUser = Serializer.Load(user, serializationKey);
		Debug.Log(newUser.Name + " is " + newUser.Age);
	}
}

[System.Serializable]
public struct User : ISerializableData
{
	public string Name;
	public int Age;
}

```
