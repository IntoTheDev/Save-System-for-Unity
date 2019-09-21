using ToolBox.Attributes;
using ToolBox.Serialization;
using UnityEngine;

public class SerializationExample : MonoBehaviour
{
	private void Start()
	{
		// Saving data...
		PlayerData playerData = new PlayerData
		{
			name = "My name",
			age = 20
		};

		GameData.Save(playerData);

		// Loading data...
		PlayerData newPlayerData = new PlayerData();
		newPlayerData = GameData.Load(newPlayerData);
	}

	[Button("Save")]
	private void SaveData()
	{
		// Saving data...
		PlayerData playerData = new PlayerData
		{
			name = "My name",
			age = 20
		};

		GameData.Save(playerData);
	}
}

public struct PlayerData : ISerializedData
{
	public string name;
	public int age;
}