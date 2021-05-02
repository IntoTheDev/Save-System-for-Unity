using System.Collections.Generic;
using System.IO;
using ToolBox.Serialization.OdinSerializer;
using UnityEngine;

namespace ToolBox.Serialization
{
	public static class DataSerializer
	{
		private static Dictionary<string, ISerializable> _data = null;
		private static int _currentProfileIndex = 0;
		private static string _savePath = "";

		private const string FILE_NAME = "Save";
		private const DataFormat DATA_FORMAT = DataFormat.Binary;
		private const int INITIAL_SIZE = 64;

		public static void Save<T>(string key, T dataToSave)
		{
			if (_data.TryGetValue(key, out var data))
			{
				var item = (Item<T>)data;
				item.Value = dataToSave;
			}
			else
			{
				var saveItem = new Item<T> { Value = dataToSave };
				_data.Add(key, saveItem);
			}
		}

		public static T Load<T>(string key)
		{
			_data.TryGetValue(key, out var value);
			var loadItem = (Item<T>)value;

			return loadItem.Value;
		}

		public static bool TryLoad<T>(string key, out T data)
		{
			bool hasKey;

			if (_data.TryGetValue(key, out var value))
			{
				var loadItem = (Item<T>)value;
				data = loadItem.Value;
				hasKey = true;
			}
			else
			{
				hasKey = false;
				data = default;
			}

			return hasKey;
		}

		public static bool HasKey(string key) =>
			_data.ContainsKey(key);

		public static void DeleteKey(string key) =>
			_data.Remove(key);

		public static void DeleteAll() =>
			_data.Clear();

		public static void ChangeProfile(int profileIndex)
		{
			if (_currentProfileIndex == profileIndex)
				return;

			SaveFile();

			_currentProfileIndex = profileIndex;
			GeneratePath();
			LoadFile();
		}

		private static void SaveFile()
		{
			var bytes = SerializationUtility.SerializeValue(_data, DATA_FORMAT);
			File.WriteAllBytes(_savePath, bytes);
		}

		private static void LoadFile()
		{
			if (!File.Exists(_savePath))
			{
				var fileStream = File.Create(_savePath);
				fileStream?.Close();
			}

			var loadBytes = File.ReadAllBytes(_savePath);
			_data = SerializationUtility.DeserializeValue<Dictionary<string, ISerializable>>(loadBytes, DATA_FORMAT);

			if (_data == null)
				_data = new Dictionary<string, ISerializable>(INITIAL_SIZE);
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void Setup()
		{
			_currentProfileIndex = 0;
			GeneratePath();

			LoadFile();
			Application.quitting += SaveFile;
		}

		private static void GeneratePath() =>
			_savePath = Path.Combine(Application.persistentDataPath, $"{FILE_NAME}_{_currentProfileIndex}.data");
	}
}

