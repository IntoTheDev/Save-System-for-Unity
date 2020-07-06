namespace ToolBox.Serialization
{
	public interface ISerializable
	{

	}

	public interface ISerializableState : ISerializable
	{
		void Setup(string guid);

		void Save();

		void Load();
	}
}
