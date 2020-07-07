using Sirenix.OdinInspector;
using ToolBox.Utilities;

namespace ToolBox.Serialization
{
	public abstract class SerializedAsset<T> : AssetWithGuid, ISerializable where T : ISerializable
	{
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
		private void Save() =>
			DataSerializer.Save(_value, this);

		private void Load()
		{
			T data = DataSerializer.Load<T>(_value);

			if (data != null)
				Load(data);
		}

		protected abstract void Load(T data);
	}
}

