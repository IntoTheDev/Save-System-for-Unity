using MessagePack;
using MessagePack.Resolvers;
using MessagePack.Unity;
using MessagePack.Unity.Extension;
using Serializer;
using ToolBox.Serialization;
using UnityEngine;

namespace Runtime
{
    public static class MessagePackStartup
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Setup()
        {
            StaticCompositeResolver.Instance.Register(
                UnityBlitResolver.Instance,
                UnityResolver.Instance,
                StandardResolver.Instance,
                DataSerializerResolver.Instance
            );

            var options = ContractlessStandardResolverAllowPrivate.Options.WithResolver(StaticCompositeResolver.Instance);

            DataSerializer.Options = options;
            MessagePackSerializer.DefaultOptions = options;
        }
    }
}