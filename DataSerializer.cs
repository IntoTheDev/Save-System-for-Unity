using Sirenix.Serialization;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

namespace ToolBox.Serialization
{
	public static class DataSerializer
	{
		public static event UnityAction FileSaving = null;
		public static event UnityAction OnFileLoaded = null;

		private static Dictionary<string, ISerializable> _data = null;
		private static int _currentProfileIndex = 0;

		private const string FILE_NAME = "Save";

		public static void Save<T>(string saveKey, T dataToSave) where T : ISerializable
		{
			if (_data.ContainsKey(saveKey))
				_data[saveKey] = dataToSave;
			else
				_data.Add(saveKey, dataToSave);
		}

		public static T Load<T>(string loadKey) where T : ISerializable
		{
			if (_data.TryGetValue(loadKey, out ISerializable value))
				return (T)value;

			return default;
		}

		public static void ChangeProfile(int profileIndex)
		{
			if (_currentProfileIndex == profileIndex)
				return;

			SaveFile();

			_currentProfileIndex = profileIndex;
			LoadFile();
		}

		public static void SaveFile()
		{
			string filePath = GetFilePath(_currentProfileIndex);

			FileSaving?.Invoke();
			byte[] bytes = SerializationUtility.SerializeValue(_data, DataFormat.JSON);
			File.WriteAllBytes(filePath, bytes);
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

		private static void LoadFile()
		{
			string filePath = GetFilePath(_currentProfileIndex);

			if (!File.Exists(filePath))
				CreateFile(_currentProfileIndex, true);

			byte[] loadBytes = File.ReadAllBytes(filePath);
			_data = SerializationUtility.DeserializeValue<Dictionary<string, ISerializable>>(loadBytes, DataFormat.JSON);

			if (_data == null)
				_data = new Dictionary<string, ISerializable>(10);
		}

		private static string GetFilePath(int profileIndex) =>
			Path.Combine(Application.persistentDataPath, $"{FILE_NAME}_{profileIndex}.data");

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
		private static void Setup()
		{
			_currentProfileIndex = 0;

			LoadFile();
			Application.quitting += SaveFile;
#if UNITY_EDITOR
			OnFileLoaded?.Invoke();
#endif
		}
	}
}

