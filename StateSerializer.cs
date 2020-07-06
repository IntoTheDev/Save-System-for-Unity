using Sirenix.OdinInspector;
using ToolBox.Utilities;
using UnityEngine;

namespace ToolBox.Serialization
{
	[RequireComponent(typeof(GuidGenerator)), DefaultExecutionOrder(5), DisallowMultipleComponent]
	public sealed class StateSerializer : MonoBehaviour
	{
		[ShowInInspector, ReadOnly] private SerializedMono[] _serializedMonos = null;

		private GuidGenerator _guidGenerator = null;

		private void Awake()
		{
			_serializedMonos = GetComponents<SerializedMono>();
			_guidGenerator = GetComponent<GuidGenerator>();
			string guid = _guidGenerator.InstanceValue;

			for (int i = 0; i < _serializedMonos.Length; i++)
			{
				SerializedMono mono = _serializedMonos[i];
				mono.Setup(guid);
				mono.Load();
			}
		}

		[Button]
		public void Save()
		{
			for (int i = 0; i < _serializedMonos.Length; i++)
				_serializedMonos[i].Save();
		}
	}
}
