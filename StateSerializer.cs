using Sirenix.OdinInspector;
using ToolBox.Utilities;
using UnityEngine;

namespace ToolBox.Serialization
{
	[RequireComponent(typeof(GuidGenerator)), DefaultExecutionOrder(5), DisallowMultipleComponent]
	public sealed class StateSerializer : MonoBehaviour
	{
		[SerializeReference, HideInPlayMode] private ISerializableState[] _serializablePOCOS = null;
		[ShowInInspector, ReadOnly, HideInEditorMode] private ISerializableState[] _serializables = null;

		private ISerializableState[] _serializedMonos = null;

		private GuidGenerator _guidGenerator = null;
		private string _guid = "";

		private void Awake()
		{
			SetData();
			SetSerializables();
		}

		[Button]
		public void Save()
		{
			for (int i = 0; i < _serializables.Length; i++)
				_serializables[i].Save();
		}

		private void SetData()
		{
			_serializedMonos = GetComponents<ISerializableState>();
			_serializables = new ISerializableState[_serializablePOCOS.Length + _serializedMonos.Length];
			_guidGenerator = GetComponent<GuidGenerator>();
			_guid = _guidGenerator.InstanceValue;
		}

		private void SetSerializables()
		{
			for (int i = 0; i < _serializablePOCOS.Length; i++)
			{
				ISerializableState serializable = _serializablePOCOS[i];

				_serializables[i] = serializable;
				serializable.Setup(_guid);
				serializable.Load();
			}

			for (int i = 0; i < _serializedMonos.Length; i++)
			{
				int index = i + _serializablePOCOS.Length;
				ISerializableState serializable = _serializedMonos[i];

				_serializables[index] = serializable;
				serializable.Setup(_guid);
				serializable.Load();
			}

			_serializablePOCOS = null;
			_serializedMonos = null;
		}
	}
}
