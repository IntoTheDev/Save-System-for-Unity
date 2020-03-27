using UnityEngine;

public class Saver : Serializer
{
	protected override void Serialize()
	{
		for (int i = 0; i < serializables.Length; i++)
		{
			ISerializable serializable = serializables[i];
			serializable.Save();
		}	
	}
}
