using System;
using System.Collections.Generic;
using System.IO;
using ToolBox.Serialization.OdinSerializer;
using UnityEngine;

namespace ToolBox.Serialization
{
	public static class DataSerializer
	{
		private static Dictionary<string, byte[]> _data;
		private static int _currentProfileIndex;
		private static string _savePath;
		private static SerializationContext _serializationContext;
		private static DeserializationContext _deserializationContext;
		private static ReferenceResolver _referenceResolver;
		private const string FileName = "Save";
		private const DataFormat DataFormat = OdinSerializer.DataFormat.Binary;
		private const int InitialSize = 64;

		public static AssetsContainer Container { get; private set; }
		public static event Action FileSaving;

		public static void Save<T>(string key, T dataToSave)
		{
			_data[key] = Serialize(dataToSave);
		}

		public static T Load<T>(string key)
		{
			_data.TryGetValue(key, out var value);

			return Deserialize<T>(value);
		}

		public static bool TryLoad<T>(string key, out T data)
		{
			bool hasKey;

			if (_data.TryGetValue(key, out var value))
			{
				data = Deserialize<T>(value);
				hasKey = true;
			}
			else
			{
				data = default;
				hasKey = false;
			}

			return hasKey;
		}

		public static bool HasKey(string key)
		{
			return _data.ContainsKey(key);
		}

		public static void DeleteKey(string key)
		{
			_data.Remove(key);
		}

		public static void DeleteAll()
		{
			_data.Clear();
		}

		public static void ChangeProfile(int profileIndex)
		{
			if (_currentProfileIndex == profileIndex)
			{
				return;
			}

			SaveFile();

			_currentProfileIndex = profileIndex;
			GeneratePath();
			LoadFile();
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void Setup()
		{
			Container = Resources.Load<AssetsContainer>("ToolBoxAssetsContainer");
			_referenceResolver = new ReferenceResolver(Container);
			_serializationContext = new SerializationContext(_referenceResolver);
			_deserializationContext = new DeserializationContext(_referenceResolver);
			
			GeneratePath();
			LoadFile();
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void CreateObserver()
		{
			var obj = new GameObject("ApplicationStateObserver");
			var observer = obj.AddComponent<ApplicationStateObserver>();
			UnityEngine.Object.DontDestroyOnLoad(obj);
			observer.OnQuit += SaveFile;
		}

		private static void SaveFile()
		{
			FileSaving?.Invoke();

			var bytes = Serialize(_data);
			File.WriteAllBytes(_savePath, bytes);
		}

		private static void LoadFile()
		{
			if (!File.Exists(_savePath))
			{
				var fileStream = File.Create(_savePath);
				fileStream.Close();
			}
			
			_data = Deserialize<Dictionary<string, byte[]>>(File.ReadAllBytes(_savePath)) ?? new Dictionary<string, byte[]>(InitialSize);
		}

		private static void GeneratePath()
		{
			_savePath = Path.Combine(Application.persistentDataPath, $"{FileName}_{_currentProfileIndex.ToString()}.data");
		}

		private static byte[] Serialize<T>(T data)
		{
			var bytes = SerializationUtility.SerializeValue(data, DataFormat, _serializationContext);
			_serializationContext.ResetToDefault();
			_serializationContext.IndexReferenceResolver = _referenceResolver;

			return bytes;
		}

		private static T Deserialize<T>(byte[] bytes)
		{
			var data = SerializationUtility.DeserializeValue<T>(bytes, DataFormat, _deserializationContext);
			_deserializationContext.Reset();
			_deserializationContext.IndexReferenceResolver = _referenceResolver;

			return data;
		}
	}
}

