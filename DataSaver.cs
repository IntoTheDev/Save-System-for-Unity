using UnityEngine;

namespace ToolBox.Serialization
{
	public class DataSaver : MonoBehaviour
	{
		private void Awake() =>
			DontDestroyOnLoad(this);

		private void OnApplicationQuit() =>
			DataSerializer.SaveFile();
	}
}

