using System;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;

namespace Serializer
{
    public class DataSerializerResolver : IFormatterResolver
    {
        public static readonly IFormatterResolver Instance = new DataSerializerResolver();

        private DataSerializerResolver()
        {
        }

        public IMessagePackFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.Formatter;
        }

        private static class FormatterCache<T>
        {
            internal static readonly IMessagePackFormatter<T> Formatter;

            static FormatterCache()
            {
                var formatter = DataSerializerResolverGetFormatterHelper.GetFormatter(typeof(T));

                if (formatter != null)
                {
                    Formatter = (IMessagePackFormatter<T>)formatter;
                }
            }
        }
    }

    internal static class DataSerializerResolverGetFormatterHelper
    {
        private static readonly Dictionary<Type, int> _lookup = new()
        {
            { typeof(Dictionary<string, byte[]>), 0 },
        };

        internal static object GetFormatter(Type t)
        {
            if (!_lookup.TryGetValue(t, out var key))
            {
                return null;
            }

            return key switch
            {
                0 => new DictionaryFormatter<string, byte[]>(),
                _ => null,
            };
        }
    }
}