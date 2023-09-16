using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Object = UnityEngine.Object;

namespace ToolBox.Serialization
{
	internal sealed class AssetsContainer : ScriptableObject
	{
		[SerializeField] private AssetEntry[] _savedAssets;
		[SerializeField] private string[] _paths;

		public bool TryResolveId(Object value, out string id)
		{
			id = null;

			if (!TryGetByObject(value, out var entry))
			{
				return false;
			}

			id = entry.Guid;
			return true;
		}

		public bool TryResolveReference(string id, out Object value)
		{
			value = null;
			
			if (!TryGetById(id, out var entry))
			{
				return false;
			}

			value = entry.Asset;
			return true;
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
			_savedAssets ??= new AssetEntry[0];

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

			var newEntries = new List<AssetEntry>();

			foreach (var asset in assets)
			{
				if (!TryGetByObject(asset, out _))
				{
					newEntries.Add(new AssetEntry(Guid.NewGuid().ToString(), asset));
				}

				var children = AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(asset));

				foreach (var child in children)
				{
					if (TryGetByObject(child, out _))
					{
						continue;
					}
					
					newEntries.Add(new AssetEntry(Guid.NewGuid().ToString(), child));
				}
			}

			ArrayUtility.AddRange(ref _savedAssets, newEntries.ToArray());
			EditorUtility.SetDirty(this);
		}
		
		public void Clear()
		{
			_savedAssets = null;
			EditorUtility.SetDirty(this);
		}
#endif

		private bool TryGetById(string guid, out AssetEntry entry)
		{
			foreach (var asset in _savedAssets)
			{
				if (asset.Guid != guid)
				{
					continue;
				}

				entry = asset;
				return true;
			}

			entry = null;
			return false;
		}

		private bool TryGetByObject(Object value, out AssetEntry entry)
		{
			foreach (var asset in _savedAssets)
			{
				if (asset.Asset != value)
				{
					continue;
				}

				entry = asset;
				return true;
			}

			entry = null;
			return false;
		}
	}
}
