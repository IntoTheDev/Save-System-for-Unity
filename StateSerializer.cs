using Sirenix.OdinInspector;
using ToolBox.Utilities;
using UnityEngine;

namespace ToolBox.Serialization
{
	[RequireComponent(typeof(GuidGenerator)), DefaultExecutionOrder(5), DisallowMultipleComponent]
	public sealed class StateSerializer : MonoBehaviour
	{
		[SerializeReference, HideInPlayMode] private ISerializable[] _serializablePOCOS = null;
		[ShowInInspector, ReadOnly, HideInEditorMode] private ISerializable[] _serializables = null;

		private ISerializable[] _serializedMonos = null;

		private GuidGenerator _guidGenerator = null;

		private void Awake()
		{
			_serializedMonos = GetComponents<ISerializable>();

			_serializables = new ISerializable[_serializablePOCOS.Length + _serializedMonos.Length];

			for (int i = 0; i < _serializablePOCOS.Length; i++)
				_serializables[i] = _serializablePOCOS[i];

			for (int i = 0; i < _serializedMonos.Length; i++)
			{
				int index = i + _serializablePOCOS.Length;
				_serializables[index] = _serializedMonos[i];
			}

			_guidGenerator = GetComponent<GuidGenerator>();
			string guid = _guidGenerator.InstanceValue;

			for (int i = 0; i < _serializables.Length; i++)
			{
				ISerializable mono = _serializables[i];
				mono.Setup(guid);
				mono.Load();
			}
		}

		[Button]
		public void Save()
		{
			for (int i = 0; i < _serializables.Length; i++)
				_serializables[i].Save();
		}
	}
}
