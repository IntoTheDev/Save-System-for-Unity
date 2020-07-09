using Sirenix.OdinInspector;
using ToolBox.Utilities;

namespace ToolBox.Serialization
{
	public abstract class SerializedAsset<T> : AssetWithGuid, ISerializable where T : ISerializable
	{
		protected T _data = default;

		protected override void OnEnable()
		{
			base.OnEnable();

			DataSerializer.FileSaving += Save;		
#if UNITY_EDITOR
			DataSerializer.OnFileLoaded += Load;
#else
			Load();
#endif
		}

		[Button]
		private void Save()
		{
			SaveData();
			DataSerializer.Save(_value, _data);
		}

		private void Load()
		{
			_data = DataSerializer.Load<T>(_value);

			if (_data != null)
				Load(_data);
		}

		protected abstract void SaveData();
		protected abstract void Load(T data);
	}
}

