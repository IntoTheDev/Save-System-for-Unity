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
		[SerializeField] private AssetEntry[] _savedAssets = Array.Empty<AssetEntry>();
		[SerializeField] private string[] _paths;

		public bool TryResolveId(object value, out string id)
		{
			id = null;

			if (value is not Object obj || !TryGetValue(obj, out var entry)) 
				return false;
			
			id = entry.Guid;
			return true;
		}

		public bool TryResolveReference(string id, out object value)
		{
			value = null;

			if (id == null)
				return false;

			var contains = TryGetValue(id, out var entry);
			value = entry.Asset;

			return contains;
		}

#if UNITY_EDITOR
		public void LoadAssets()
		{
			if (_paths == null)
				return;

			_paths = _paths.Where(x => !string.IsNullOrEmpty(x) && AssetDatabase.IsValidFolder(x)).ToArray();

			if (_paths.Length == 0)
				return;

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
				var path = AssetDatabase.GetAssetPath(asset);
				var guid = AssetDatabase.AssetPathToGUID(path);

				if (!TryGetValue(asset, out _))
					newEntries.Add(new AssetEntry(guid, asset));

				var childAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(asset));

				foreach (var child in childAssets)
				{
					if (TryGetValue(child, out _)) 
						continue;
					
					var childGuid = Guid.NewGuid().ToString();
					newEntries.Add(new AssetEntry(childGuid, child));
				}
			}

			ArrayUtility.AddRange(ref _savedAssets, newEntries.ToArray());
			EditorUtility.SetDirty(this);
		}
		
		public void Clear()
		{
			_savedAssets = Array.Empty<AssetEntry>();
			EditorUtility.SetDirty(this);
		}
#endif

		private bool TryGetValue(string guid, out AssetEntry entry)
		{
			foreach (var asset in _savedAssets)
			{
				if (asset.Guid != guid) 
					continue;
				
				entry = asset;
				return true;
			}

			entry = null;
			return false;
		}

		private bool TryGetValue(Object obj, out AssetEntry entry)
		{
			foreach (var asset in _savedAssets)
			{
				if (asset.Asset != obj) 
					continue;
				
				entry = asset;
				return true;
			}

			entry = null;
			return false;
		}
	}
}
