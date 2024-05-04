using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Object = UnityEngine.Object;

namespace ToolBox.Serialization
{
    public sealed class AssetsContainer : ScriptableObject
    {
        [SerializeField] private Object[] _savedAssets;
        [SerializeField] private string[] _paths;

        public bool TryGetObject(ushort id, out Object entry)
        {
            entry = null;

            if (id == 0 || id >= _savedAssets.Length)
            {
                return false;
            }

            entry = _savedAssets[id];
            return true;
        }

        public bool TryGetId(Object value, out ushort id)
        {
            id = 0;

            for (ushort i = 1; i < _savedAssets.Length; i++)
            {
                if (_savedAssets[i] != value)
                {
                    continue;
                }

                id = i;
                return true;
            }

            return false;
        }

#if UNITY_EDITOR
        public void LoadAssets()
        {
            if (_paths == null)
            {
                return;
            }

            _paths = _paths.Where(x => !string.IsNullOrEmpty(x) && AssetDatabase.IsValidFolder(x)).ToArray();

            if (_paths.Length == 0)
            {
                return;
            }

            // ReSharper disable once UseArrayEmptyMethod
            _savedAssets ??= new Object[0];

            var assets = AssetDatabase
                .FindAssets("t:Object", _paths)
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<Object>)
                .Where(x =>
                {
                    var fileNamespace = x.GetType().Namespace;

                    return x != null && (fileNamespace == null || !fileNamespace.Contains("UnityEditor"));
                })
                .ToList();

            var newEntries = new List<Object>();

            foreach (var asset in assets)
            {
                if (!TryGetId(asset, out _))
                {
                    newEntries.Add(asset);
                }

                var children = AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(asset));

                foreach (var child in children)
                {
                    if (TryGetId(child, out _))
                    {
                        continue;
                    }

                    newEntries.Add(child);
                }
            }

            ArrayUtility.AddRange(ref _savedAssets, newEntries.ToArray());

            if (_savedAssets.Length == 0 || _savedAssets[0] != null)
            {
                ArrayUtility.Insert(ref _savedAssets, 0, null);
            }

            EditorUtility.SetDirty(this);
        }

        public void Clear()
        {
            _savedAssets = null;
            EditorUtility.SetDirty(this);
        }
#endif
    }
}