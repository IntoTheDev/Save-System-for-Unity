using UnityEngine;

namespace ToolBox.Serialization
{
	public abstract class SerializedPOCO<T> : ISerializableState where T : ISerializable
	{
		[SerializeField] protected T data = default;

		private string _saveKey = "";

		public void Setup(string guid) =>
			_saveKey = $"{guid}{GetType()}";

		public void Save()
		{
			OnSaving();
			DataSerializer.Save(_saveKey, data);
		}

		public void Load()
		{
			T data = DataSerializer.Load<T>(_saveKey);

			if (data != null)
				Load(data);
		}

		protected abstract void OnSaving();

		protected abstract void Load(T data);
	}
}
