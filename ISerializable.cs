public interface ISerializable 
{
	void Setup(string guid);

	void Save();

	void Load();
}
