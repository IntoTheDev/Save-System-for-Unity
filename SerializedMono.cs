using UnityEngine;

namespace ToolBox.Serialization
{
	public abstract class SerializedMono : MonoBehaviour, ISerializable
	{
		protected string _saveKey = "";

		public void Setup(string guid) =>
			_saveKey = $"{guid}{GetType()}";

		public void Save() =>
			DataSerializer.Save(_saveKey, this);

		public abstract void Load();
	}

	public abstract class SerializedMono<T> : SerializedMono where T : ISerializable
	{
		public override void Load()
		{
			T data = DataSerializer.Load<T>(_saveKey);

			if (data != null)
				Load(data);
		}

		protected abstract void Load(T data);
	}
}
