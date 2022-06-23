using UnityEngine;

namespace ToolBox.Serialization
{
	[System.Serializable]
	internal sealed class AssetEntry
	{
		[SerializeField] private string _guid;
		[SerializeField] private Object _asset;

		public string Guid => _guid;
		public Object Asset => _asset;

		public AssetEntry(string guid, Object asset)
		{
			_guid = guid;
			_asset = asset;
		}
	}
}
