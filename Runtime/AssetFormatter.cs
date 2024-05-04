using MessagePack;
using MessagePack.Formatters;
using Object = UnityEngine.Object;

namespace ToolBox.Serialization
{
    public class AssetFormatter<T> : IMessagePackFormatter<T> where T : Object
    {
        public void Serialize(ref MessagePackWriter writer, T value, MessagePackSerializerOptions options)
        {
            DataSerializer.Container.TryGetId(value, out var id);
			
            writer.WriteUInt16(id);
        }

        public T Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            DataSerializer.Container.TryGetObject(reader.ReadUInt16(), out var value);
			
            return (T)value;
        }
    }
}