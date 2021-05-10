using System.Collections.Generic;
using System.Linq;
using ToolBox.Serialization.OdinSerializer;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace ToolBox.Serialization
{
	internal sealed class AssetsContainer : ScriptableObject, IExternalStringReferenceResolver
	{
		[SerializeField] private AssetEntry[] _savedAssets = null;
		[SerializeField] private string[] _paths = null;

		public IExternalStringReferenceResolver NextResolver { get; set; }

		public bool CanReference(object value, out string id)
		{
			id = null;

			if (!(value is Object obj) || !TryGetValue(obj, out var entry)) 
				return false;
			
			id = entry.Guid;
			return true;
		}

		public bool TryResolveReference(string id, out object value)
		{
			value = null;

			if (id == null)
				return false;

			bool contains = TryGetValue(id, out var entry);
			value = entry.Asset;

			return contains;
		}

#if UNITY_EDITOR
		// TODO: Make everything with loops and lists instead of LINQ
		public void LoadAssets()
		{
			if (_paths == null)
				return;

			_paths = _paths.Where(x => !string.IsNullOrEmpty(x) && AssetDatabase.IsValidFolder(x)).ToArray();

			if (_paths.Length == 0)
				return;

			List<Object> assets;

			assets = AssetDatabase
				.FindAssets("t:Object", _paths)
				.Select(AssetDatabase.GUIDToAssetPath)
				.Select(AssetDatabase.LoadAssetAtPath<Object>)
				.Where(x =>
				{
					string fileNamespace = x.GetType().Namespace;

					return x != null && (fileNamespace == null || !fileNamespace.Contains("UnityEditor"));
				}) // Change UnityEditor to Editor?
				.ToList();

			var newEntries = new List<AssetEntry>();

			foreach (var asset in assets)
			{
				string path = AssetDatabase.GetAssetPath(asset);
				string guid = AssetDatabase.AssetPathToGUID(path);

				if (!TryGetValue(asset, out _))
					newEntries.Add(new AssetEntry(guid, asset));

				var childAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(asset));

				for (int i = 0; i < childAssets.Length; i++)
				{
					var child = childAssets[i];

					if (!TryGetValue(child, out _))
					{
						string childGuid = System.Guid.NewGuid().ToString();
						newEntries.Add(new AssetEntry(childGuid, child));
					}
				}
			}

			ArrayUtility.AddRange(ref _savedAssets, newEntries.ToArray());
			EditorUtility.SetDirty(this);
		}
		
		public void Clear()
		{
			_savedAssets = new AssetEntry[0];
			EditorUtility.SetDirty(this);
		}
#endif

		private bool TryGetValue(string guid, out AssetEntry entry)
		{
			for (int i = 0; i < _savedAssets.Length; i++)
			{
				var asset = _savedAssets[i];

				if (asset.Guid == guid)
				{
					entry = asset;
					return true;
				}
			}

			entry = null;
			return false;
		}

		private bool TryGetValue(Object obj, out AssetEntry entry)
		{
			for (int i = 0; i < _savedAssets.Length; i++)
			{
				var asset = _savedAssets[i];

				if (asset.Asset == obj)
				{
					entry = asset;
					return true;
				}
			}

			entry = null;
			return false;
		}
	}
}
