using System.IO;
using System.Text;
using UnityEngine;

namespace ToolBox.Serialization
{
	public static class Serializer
	{
		public static void Save<T>(T dataToSave, string saveKey) where T : ISerializableData
		{
			string dataAsJson = JsonUtility.ToJson(dataToSave, true);
			SaveData(saveKey, dataAsJson);
		}

		public static void Save<T>(T[] dataToSave, string saveKey) where T : ISerializableData
		{
			string dataAsJson = JsonHelper.ToJson(dataToSave, true);
			SaveData(saveKey, dataAsJson);
		}

		public static T Load<T>(T dataToLoad, string loadKey) where T : ISerializableData
		{
			string dataAsJson = LoadData(loadKey);

			if (dataAsJson == null)
				return default;

			return JsonUtility.FromJson<T>(dataAsJson);
		}

		public static T[] Load<T>(T[] dataToLoad, string loadKey) where T : ISerializableData
		{
			string dataAsJson = LoadData(loadKey);

			if (dataAsJson == null)
				return default;

			return JsonHelper.FromJson<T>(dataAsJson);
		}

		private static void SaveData(string fileName, string dataAsJson)
		{
			string filePath = GetFilePath(fileName);

			// Encrypt file if we are not in editor
			if (!Application.isEditor)
				dataAsJson = EncryptDecrypt(dataAsJson);

			File.WriteAllText(filePath, dataAsJson);
		}

		private static string LoadData(string fileName)
		{
			string filePath = GetFilePath(fileName);

			if (!File.Exists(filePath))
				return null;

			string dataAsJson = File.ReadAllText(filePath);

			// Decrypt file if we are not in editor
			if (!Application.isEditor)
				dataAsJson = EncryptDecrypt(dataAsJson);

			return dataAsJson;
		}

		private static string GetFilePath(string fileName)
		{
			return Path.Combine(Application.persistentDataPath, fileName + ".json");
		}

		private static string EncryptDecrypt(string data)
		{
			int length = data.Length;

			StringBuilder inSb = new StringBuilder(data);
			StringBuilder outSb = new StringBuilder(length);

			char c;
			int key = 129;

			for (int i = 0; i < length; i++)
			{
				c = inSb[i];
				c = (char)(c ^ key);
				outSb.Append(c);
			}

			return outSb.ToString();
		}
	}
}

