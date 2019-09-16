# Save-System-for-Unity
Save System for Unity with Json and basic encryption also supports arrays

## Usage
We need to create a struct of data that we will save and inherit interface "ISavableData"

```csharp
[System.Serializable]
public struct PlayerData : ISavableData
{
	public string Name;
	public int Age;
}
```

To save data we need to do:

```csharp
// Somewhere in code...
PlayerData playerData = new PlayerData
{
	Name = "My name",
	Age = 20
};

GameData.Save(playerData);
```

We will get a new file in Application.persistentDataPath (If file is saved in a standalone build, file will be encrypted)

![Saved file](https://i.gyazo.com/80c8bff7d88fd315359b2721e65a25cb.png)

To load data we need to do:

```csharp
// Somewhere in code...
PlayerData newPlayerData = new PlayerData();
newPlayerData = GameData.Load(newPlayerData);

Debug.Log(newPlayerData.Name);
```

Result:

![Result](https://i.gyazo.com/188587d9c9fea10030070d1a169d265f.png)

