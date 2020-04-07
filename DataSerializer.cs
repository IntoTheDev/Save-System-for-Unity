using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ToolBox.Serialization
{
	public static class DataSerializer
	{
		private static Dictionary<int, ISerializable> data = null;
		private static int currentProfileIndex = 0;
		[NonSerialized, ShowInInspector] private static bool isInitialized = false;

		private const string FILE_NAME = "Save";

		public static void Save<T>(int instanceID, T dataToSave) where T : ISerializable
		{
			if (!isInitialized)
				Initialize();
			
			if (data.ContainsKey(instanceID))
				data[instanceID] = dataToSave;
			else
				data.Add(instanceID, dataToSave);
		}

		public static T Load<T>(int instanceID) where T : ISerializable
		{
			if (!isInitialized)
				Initialize();

			if (data.TryGetValue(instanceID, out ISerializable value))
				return (T)value;

			return default;
		}

		public static void CreateFile(int profileIndex, bool overwrite)
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

		public static void SaveFile()
		{
			string filePath = GetFilePath(currentProfileIndex);

			if (!isInitialized || !File.Exists(filePath))
				return;

			byte[] bytes = SerializationUtility.SerializeValue(data, DataFormat.Binary);
			File.WriteAllBytes(filePath, bytes);

			if (!isInitialized)
				Initialize();
		}

		public static void LoadFile()
		{
			if (isInitialized)
				return;

			string filePath = GetFilePath(currentProfileIndex);

			if (!File.Exists(filePath))
				CreateFile(currentProfileIndex, true);

			byte[] loadBytes = File.ReadAllBytes(filePath);
			data = SerializationUtility.DeserializeValue<Dictionary<int, ISerializable>>(loadBytes, DataFormat.Binary);

			if (data == null)
				data = new Dictionary<int, ISerializable>(10);

			if (!isInitialized)
				Initialize();
		}

		public static void ChangeProfile(int profileIndex, bool saveCurrentFile)
		{
			if (currentProfileIndex == profileIndex)
				return;

			if (saveCurrentFile)
				SaveFile();

			currentProfileIndex = profileIndex;
			isInitialized = false;

			LoadFile();
		}

		public static void UnloadFile()
		{
			data = null;
			isInitialized = false;
		}

		private static string GetFilePath(int profileIndex) =>
			Path.Combine(Application.persistentDataPath, $"{FILE_NAME}_{profileIndex}.data");

		private static void Initialize()
		{
			isInitialized = true;
			new GameObject("[DATA SAVER]", typeof(DataSaver));
			LoadFile();
		}
	}
}

