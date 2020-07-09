using Sirenix.OdinInspector;
using System;
using ToolBox.Utilities;
using UnityEngine;

namespace ToolBox.Serialization
{
	[DefaultExecutionOrder(5), DisallowMultipleComponent, RequireComponent(typeof(GuidGenerator))]
	public sealed class StateSerializer : MonoBehaviour
	{
		[SerializeReference, HideInPlayMode] private ISerializableState[] _serializablePOCOS = null;
		[ShowInInspector, ReadOnly, HideInEditorMode] private ISerializableState[] _serializables = null;
		[ShowInInspector, ReadOnly, HideInEditorMode] private string _instanceID = "";

		private GuidGenerator _guidGenerator = null;
		private ISerializableState[] _serializedMonos = null;

		private void Awake()
		{
			Entities.BeforeSave += Save;
			_guidGenerator = GetComponent<GuidGenerator>();
			_serializedMonos = GetComponents<ISerializableState>();
			_serializables = new ISerializableState[_serializablePOCOS.Length + _serializedMonos.Length];

			_instanceID = Guid.NewGuid().ToString();

			for (int i = 0; i < _serializablePOCOS.Length; i++)
			{
				ISerializableState serializable = _serializablePOCOS[i];

				_serializables[i] = serializable;
				serializable.Setup(_instanceID);
			}

			for (int i = 0; i < _serializedMonos.Length; i++)
			{
				int index = i + _serializablePOCOS.Length;
				ISerializableState serializable = _serializedMonos[i];

				_serializables[index] = serializable;
				serializable.Setup(_instanceID);
			}

			_serializablePOCOS = null;
			_serializedMonos = null;
		}

		private void OnDestroy() =>
			Entities.BeforeSave -= Save;


		public void Load(string guid)
		{
			_instanceID = guid;

			for (int i = 0; i < _serializables.Length; i++)
			{
				ISerializableState serializable = _serializables[i];

				serializable.Setup(guid);
				serializable.Load();
			}
		}

		[Button]
		public void Save()
		{
			Entities.Add(_guidGenerator.PrefabValue, _instanceID);

			for (int i = 0; i < _serializables.Length; i++)
				_serializables[i].Save();
		}
	}
}
