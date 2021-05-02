//-----------------------------------------------------------------------
// <copyright file="UInt16Serializer.cs" company="Sirenix IVS">
// Copyright (c) 2018 Sirenix IVS
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//-----------------------------------------------------------------------

namespace ToolBox.Serialization.OdinSerializer
{
    /// <summary>
    /// Serializer for the <see cref="ushort"/> type.
    /// </summary>
    /// <seealso cref="Serializer{System.UInt16}" />
    public sealed class UInt16Serializer : Serializer<ushort>
    {
        /// <summary>
        /// Reads a value of type <see cref="ushort" />.
        /// </summary>
        /// <param name="reader">The reader to use.</param>
        /// <returns>
        /// The value which has been read.
        /// </returns>
        public override ushort ReadValue(IDataReader reader)
        {
            string name;
            var entry = reader.PeekEntry(out name);

            if (entry == EntryType.Integer)
            {
                ushort value;
                if (reader.ReadUInt16(out value) == false)
                {
                    reader.Context.Config.DebugContext.LogWarning("Failed to read entry '" + name + "' of type " + entry.ToString());
                }
                return value;
            }
            else
            {
                reader.Context.Config.DebugContext.LogWarning("Expected entry of type " + EntryType.Integer.ToString() + ", but got entry '" + name + "' of type " + entry.ToString());
                reader.SkipEntry();
                return default(ushort);
            }
        }

        /// <summary>
        /// Writes a value of type <see cref="ulong" />.
        /// </summary>
        /// <param name="name">The name of the value to write.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="writer">The writer to use.</param>
        public override void WriteValue(string name, ushort value, IDataWriter writer)
        {
            FireOnSerializedType();
            writer.WriteUInt16(name, value);
        }
    }
}