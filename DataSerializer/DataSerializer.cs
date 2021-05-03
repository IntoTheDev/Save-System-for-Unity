using System;
using System.Collections.Generic;
using System.IO;
using ToolBox.Serialization.OdinSerializer;
using UnityEngine;

namespace ToolBox.Serialization
{
	public static class DataSerializer
	{
		private static Dictionary<string, Item> _data = null;
		private static int _currentProfileIndex = 0;
		private static string _savePath = "";
		private static SerializationContext _serializationContext = null;
		private static DeserializationContext _deserializationContext = null;

		private const string FILE_NAME = "Save";
		private const DataFormat DATA_FORMAT = DataFormat.Binary;
		private const int INITIAL_SIZE = 64;

		public static event Action FileSaving = null;

		public static void Save<T>(string key, T dataToSave)
		{
			if (_data.TryGetValue(key, out var data))
			{
				var item = data;
				item.Value = SerializationUtility.SerializeValue(dataToSave, DATA_FORMAT, _serializationContext);
			}
			else
			{
				var saveItem = new Item { Value = SerializationUtility.SerializeValue(dataToSave, DATA_FORMAT, _serializationContext) };
				_data.Add(key, saveItem);
			}
		}

		public static T Load<T>(string key)
		{
			_data.TryGetValue(key, out var value);
			var loadItem = value;

			return SerializationUtility.DeserializeValue<T>(loadItem.Value, DATA_FORMAT, _deserializationContext);
		}

		public static bool TryLoad<T>(string key, out T data)
		{
			bool hasKey;

			if (_data.TryGetValue(key, out var value))
			{
				var loadItem = value;
				data = SerializationUtility.DeserializeValue<T>(loadItem.Value, DATA_FORMAT, _deserializationContext);
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

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void Setup()
		{
			var container = Resources.Load<AssetsContainer>("ToolBoxAssetsContainer");

			_serializationContext = new SerializationContext { StringReferenceResolver = container };
			_deserializationContext = new DeserializationContext { StringReferenceResolver = container };

			_currentProfileIndex = 0;
			GeneratePath();

			LoadFile();
			Application.quitting += SaveFile;
		}

		private static void SaveFile()
		{
			FileSaving?.Invoke();

			var bytes = SerializationUtility.SerializeValue(_data, DATA_FORMAT, _serializationContext);
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
			_data = SerializationUtility.DeserializeValue<Dictionary<string, Item>>(loadBytes, DATA_FORMAT, _deserializationContext);

			if (_data == null)
				_data = new Dictionary<string, Item>(INITIAL_SIZE);
		}

		private static void GeneratePath() =>
			_savePath = Path.Combine(Application.persistentDataPath, $"{FILE_NAME}_{_currentProfileIndex}.data");
	}
}

