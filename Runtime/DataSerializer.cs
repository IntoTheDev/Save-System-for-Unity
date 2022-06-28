using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MessagePack;
using UnityEngine;

namespace ToolBox.Serialization
{
    public static class DataSerializer
    {
        private static Dictionary<string, byte[]> _data = new();
        private static readonly string _persistentDataPath = Application.persistentDataPath;

        internal static AssetsContainer Container { get; private set; }

        public static MessagePackSerializerOptions Options { get; set; } =
            MessagePack.Resolvers.ContractlessStandardResolverAllowPrivate.Options;

        public static void Save<T>(string key, T data)
        {
            var bytes = Serialize(data);

            if (_data.ContainsKey(key))
            {
                _data[key] = bytes;
            }
            else
            {
                _data.Add(key, bytes);
            }
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

        public static byte[] Serialize<T>(T data)
        {
            return MessagePackSerializer.Serialize(data, Options);
        }

        public static T Deserialize<T>(byte[] bytes)
        {
            return MessagePackSerializer.Deserialize<T>(bytes, Options);
        }

        public static void SaveFile(string fileName)
        {
            var path = CreatePath(fileName);
            var bytes = Serialize(_data);

            File.WriteAllBytes(path, bytes);
        }
        
        public static async Task SaveFileAsync(string fileName, CancellationToken token = default)
        {
            var path = CreatePath(fileName);
            var bytes = Serialize(_data);

            await File.WriteAllBytesAsync(path, bytes, token);
        }

        public static void LoadFile(string fileName)
        {
            var path = CreatePath(fileName);
            var bytes = File.ReadAllBytes(path);

            if (TryDeserializeData(bytes, out var data))
                _data = data;
        }
        
        public static async Task LoadFileAsync(string fileName, CancellationToken token = default)
        {
            var path = CreatePath(fileName);
            var bytes = await File.ReadAllBytesAsync(path, token);

            if (TryDeserializeData(bytes, out var data))
                _data = data;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Setup()
        {
            Container = Resources.Load<AssetsContainer>("ToolBoxAssetsContainer");
        }

        private static string CreatePath(string fileName)
        {
            var path = Path.Combine(_persistentDataPath, fileName);

            if (!File.Exists(path))
                File.Create(path).Close();

            return path;
        }

        private static bool TryDeserializeData(byte[] bytes, out Dictionary<string, byte[]> data)
        {
            data = null;

            if (bytes.Length == 0)
                return false;

            data = Deserialize<Dictionary<string, byte[]>>(bytes);
            return true;
        }
    }
}
