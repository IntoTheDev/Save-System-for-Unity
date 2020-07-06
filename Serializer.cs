namespace ToolBox.Serialization
{
	public abstract class Serializer<T> : ISerializable where T : ISerializable
	{
		private string _saveKey = "";

		public void Setup(string guid) =>
			_saveKey = $"{guid}{GetType()}";

		public void Save()
		{
			DataSerializer.Save(_saveKey, this);
		}

		public void Load()
		{
			T data = DataSerializer.Load<T>(_saveKey);

			if (data != null)
				Load(data);
		}

		protected abstract void Load(T data);
	}
}
