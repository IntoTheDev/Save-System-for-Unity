using UnityEngine;

namespace ToolBox.Serialization
{
	public abstract class SerializedMono : MonoBehaviour, ISerializableState
	{
		protected string _saveKey = "";

		public void Setup(string guid) =>
			_saveKey = $"{guid}{GetType()}";

		public abstract void Save();

		public abstract void Load();
	}

	public abstract class SerializedMono<T> : SerializedMono where T : ISerializable
	{
		protected T _data = default;

		public override void Save()
		{
			SaveData();
			DataSerializer.Save(_saveKey, _data);
		}

		public override void Load()
		{
			_data = DataSerializer.Load<T>(_saveKey);

			if (_data != null)
				Load(_data);
		}

		protected abstract void SaveData();
		protected abstract void Load(T data);
	}
}
