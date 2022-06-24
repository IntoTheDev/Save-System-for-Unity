using MessagePack;
using MessagePack.Formatters;
using Object = UnityEngine.Object;

namespace ToolBox.Serialization
{
    public class AssetFormatter<T> : IMessagePackFormatter<T> where T : Object
    {
        public void Serialize(ref MessagePackWriter writer, T value, MessagePackSerializerOptions options)
        {
            DataSerializer.Container.TryResolveId(value, out var id);
			
            writer.Write(id);
        }

        public T Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            DataSerializer.Container.TryResolveReference(reader.ReadString(), out var value);
			
            return (T)value;
        }
    }
}