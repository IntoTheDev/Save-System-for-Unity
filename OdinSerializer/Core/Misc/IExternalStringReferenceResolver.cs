//-----------------------------------------------------------------------
// <copyright file="IExternalStringReferenceResolver.cs" company="Sirenix IVS">
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
    /// Resolves external strings references to reference objects during serialization and deserialization.
    /// </summary>
    public interface IExternalStringReferenceResolver
    {
        /// <summary>
        /// Gets or sets the next resolver in the chain.
        /// </summary>
        /// <value>
        /// The next resolver in the chain.
        /// </value>
        IExternalStringReferenceResolver NextResolver { get; set; }

        /// <summary>
        /// Tries to resolve a reference from a given Guid.
        /// </summary>
        /// <param name="id">The <see cref="string"/> to resolve.</param>
        /// <param name="value">The resolved value.</param>
        /// <returns><c>true</c> if the value was resolved; otherwise, <c>false</c>.</returns>
        bool TryResolveReference(string id, out object value);

        /// <summary>
        /// Determines whether this resolver can reference the specified value with a string.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="id">The string which references the value.</param>
        /// <returns><c>true</c> if the value can be referenced; otherwise, <c>false</c>.</returns>
        bool CanReference(object value, out string id);
    }
}