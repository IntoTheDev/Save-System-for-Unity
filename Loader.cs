using UnityEngine;

public class Loader : Serializer
{
	protected override void Serialize()
	{
		for (int i = 0; i < serializables.Length; i++)
		{
			ISerializable serializable = serializables[i];
			serializable.Load();
		}
	}
}
