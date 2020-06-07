using Sirenix.Serialization;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ToolBox.Serialization
{
	public static class DataSerializer
	{
		private static Dictionary<string, ISerializable> data = null;
		private static int currentProfileIndex = 0;

		private const string FILE_NAME = "Save";

		public static void Save<T>(string saveKey, T dataToSave) where T : ISerializable
		{
			if (data.ContainsKey(saveKey))
				data[saveKey] = dataToSave;
			else
				data.Add(saveKey, dataToSave);
		}

		public static T Load<T>(string loadKey) where T : ISerializable
		{
			if (data.TryGetValue(loadKey, out ISerializable value))
				return (T)value;

			return default;
		}

		public static void ChangeProfile(int profileIndex)
		{
			if (currentProfileIndex == profileIndex)
				return;

			SaveFile();

			currentProfileIndex = profileIndex;
			LoadFile();
		}

		private static void CreateFile(int profileIndex, bool overwrite)
		{
			string filePath = GetFilePath(profileIndex);
			bool isFileExists = File.Exists(filePath);
			FileStream fileStream = null;

			if (isFileExists && overwrite)
				fileStream = File.Create(filePath);
			else if (!isFileExists)
				fileStream = File.Create(filePath);

			fileStream?.Close();
		}

		private static void SaveFile()
		{
			string filePath = GetFilePath(currentProfileIndex);

			byte[] bytes = SerializationUtility.SerializeValue(data, DataFormat.Binary);
			File.WriteAllBytes(filePath, bytes);
		}

		private static void LoadFile()
		{
			string filePath = GetFilePath(currentProfileIndex);

			if (!File.Exists(filePath))
				CreateFile(currentProfileIndex, true);

			byte[] loadBytes = File.ReadAllBytes(filePath);
			data = SerializationUtility.DeserializeValue<Dictionary<string, ISerializable>>(loadBytes, DataFormat.Binary);

			if (data == null)
				data = new Dictionary<string, ISerializable>(10);
		}

		private static string GetFilePath(int profileIndex) =>
			Path.Combine(Application.persistentDataPath, $"{FILE_NAME}_{profileIndex}.data");

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
		private static void Initialize()
		{
			currentProfileIndex = 0;

			LoadFile();
			Application.quitting += SaveFile;
		}
	}
}

