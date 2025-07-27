using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MessagePack;
using MessagePack.Resolvers;
using UnityEngine;

namespace ToolBox.Serialization
{
    public static class DataSerializer
    {
        private static Dictionary<string, byte[]> _data = new();
        private static readonly string _persistentDataPath = Application.persistentDataPath;

        public static AssetsContainer Container { get; private set; }

        public static MessagePackSerializerOptions Options { get; set; } = ContractlessStandardResolverAllowPrivate.Options;

        public static void Save<T>(string key, T data)
        {
            var bytes = Serialize(data);

            if (_data.TryAdd(key, bytes))
            {
                return;
            }

            _data[key] = bytes;
        }

        public static T Load<T>(string key)
        {
            return Deserialize<T>(_data[key]);
        }

        public static bool TryLoad<T>(string key, out T data)
        {
            bool hasKey;

            if (_data.TryGetValue(key, out var bytes))
            {
                data = Deserialize<T>(bytes);
                hasKey = true;
            }
            else
            {
                data = default;
                hasKey = false;
            }

            return hasKey;
        }

        public static T LoadOrDefault<T>(string key, T defaultValue)
        {
            if (_data.TryGetValue(key, out var bytes))
            {
                return Deserialize<T>(bytes);
            }

            return defaultValue;
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

        public static void SaveFile(string fileName)
        {
            if (_data == null || _data.Count == 0)
            {
                return;
            }

            var path = GetPath(fileName);
            var tempPath = path + ".tmp";

            try
            {
                var fileExists = File.Exists(path);

                File.WriteAllBytes(tempPath, Serialize(_data));

                if (TryLoadFile(tempPath, out var tempData))
                {
                    if (fileExists)
                    {
                        File.Delete(path);
                    }

                    File.Move(tempPath, path);
                }
                else
                {
                    throw new Exception("Temporary save file validation failed. Aborting save.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);

                if (File.Exists(tempPath))
                {
                    File.Delete(tempPath);
                }
            }
        }

        public static void LoadFile(string fileName)
        {
            var path = GetPath(fileName);

            if (TryLoadFile(path, out var data))
            {
                _data = data;
                File.Copy(path, path + ".bak", true);
                return;
            }

            Debug.LogWarning("Primary save file is invalid or does not exist. Attempting to load from backup.");

            if (TryLoadFile(path + ".bak", out var backupData))
            {
                _data = backupData;
                return;
            }

            Debug.LogWarning("No valid backup save file found. Initializing to an empty state.");
            _data = new Dictionary<string, byte[]>();
        }

        private static bool TryLoadFile(string filePath, out Dictionary<string, byte[]> data)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    var bytes = File.ReadAllBytes(filePath);

                    if (bytes.Length > 0)
                    {
                        data = Deserialize<Dictionary<string, byte[]>>(bytes);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }

            data = null;
            return false;
        }

        public static byte[] Serialize<T>(T data)
        {
            return MessagePackSerializer.Serialize(data, Options);
        }

        public static T Deserialize<T>(byte[] bytes)
        {
            return MessagePackSerializer.Deserialize<T>(bytes, Options);
        }

        // Should we get rid of Setup and let user configure DataSerializer (there's only Container lol) themselves?
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Setup()
        {
            Container = Resources.Load<AssetsContainer>("ToolBoxAssetsContainer");
        }

        private static string GetPath(string fileName)
        {
            var path = Path.Combine(_persistentDataPath, fileName);

            if (!File.Exists(path))
            {
                File.Create(path).Close();
            }

            return path;
        }
    }
}