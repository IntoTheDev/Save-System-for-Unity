using Sirenix.Serialization;
using System.IO;
using UnityEngine;

namespace ToolBox.Serialization
{
	public static class DataSerialization
	{
		public static void Save<T>(T data, string fileName)
		{
			string filePath = Path.Combine(Application.persistentDataPath, fileName + ".data");

			byte[] bytes = SerializationUtility.SerializeValue(data, DataFormat.Binary);
			File.WriteAllBytes(filePath, bytes);
		}

		public static T Load<T>(string fileName)
		{
			string filePath = Path.Combine(Application.persistentDataPath, fileName + ".data");

			if (!File.Exists(filePath))
				return default;
			
			byte[] loadBytes = File.ReadAllBytes(filePath);
			return SerializationUtility.DeserializeValue<T>(loadBytes, DataFormat.Binary);
		}
	}
}

