using ToolBox.Serialization;
using UnityEngine;

public class ArraySerializationExample : MonoBehaviour
{
	private void Start()
	{
		// Saving data...
		PlayerData[] playersData = new PlayerData[2];

		playersData[0] = new PlayerData
		{
			name = "First Player",
			age = 20
		};

		playersData[1] = new PlayerData
		{
			name = "Second Player",
			age = 25
		};

		GameData.Save(playersData);

		// Loading data...
		PlayerData[] newPlayersData = new PlayerData[2];
		newPlayersData = GameData.Load(newPlayersData);
	}
}

[System.Serializable]
public struct PlayerData : ISerializedData
{
	public string name;
	public int age;
}