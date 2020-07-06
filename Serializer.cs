namespace ToolBox.Serialization
{
	public abstract class Serializer<T> : ISerializableState where T : ISerializableState
	{
		private string _saveKey = "";

		public void Setup(string guid) =>
			_saveKey = $"{guid}{GetType()}";

		public void Save() =>
			DataSerializer.Save(_saveKey, this);

		public void Load()
		{
			T data = DataSerializer.Load<T>(_saveKey);

			if (data != null)
				Load(data);
		}

		protected abstract void Load(T data);
	}
}
