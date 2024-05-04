using ToolBox.Serialization.OdinSerializer;
using UnityEngine;

namespace ToolBox.Serialization
{
    internal sealed class ReferenceResolver : IExternalIndexReferenceResolver
    {
        private readonly AssetsContainer _assetsContainer;

        public ReferenceResolver(AssetsContainer assetsContainer)
        {
            _assetsContainer = assetsContainer;
        }

        public bool TryResolveReference(int index, out object value)
        {
            value = null;
            
            if (index == 0)
            {
                return false;
            }
            
            var success = _assetsContainer.TryGetObject((ushort)index, out var obj);

            value = obj;
            return success;
        }

        public bool CanReference(object value, out int index)
        {
            index = 0;
            
            if (value is not Object obj)
            {
                return false;
            }
            
            var success = _assetsContainer.TryGetId(obj, out var id);

            index = id;
            return success;
        }
    }
}