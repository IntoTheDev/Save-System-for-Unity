using System;

namespace ToolBox.Serialization
{
    public interface ITypeProvider
    {
        Type[] GetTypes();
    }
}