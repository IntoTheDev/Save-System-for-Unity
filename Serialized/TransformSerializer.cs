using UnityEngine;

namespace ToolBox.Serialization
{
	public sealed class TransformSerializer : SerializedPOCO<TransformData>
	{
		[SerializeField] private Transform _transform = null;

		protected override void Load(TransformData data)
		{
			_transform.position = data.Position;
			_transform.rotation = data.Rotation;
			_transform.localScale = data.Scale;
		}

		protected override void OnSaving() =>
			data = new TransformData(_transform.position, _transform.rotation, _transform.localScale);
	}

	[System.Serializable]
	public class TransformData : ISerializable
	{
		[SerializeField, HideInInspector] private Vector3 _position;
		[SerializeField, HideInInspector] private Quaternion _rotation;
		[SerializeField, HideInInspector] private Vector3 _scale;

		public Vector3 Position => _position;
		public Quaternion Rotation => _rotation;
		public Vector3 Scale => _scale;

		public TransformData(Vector3 position, Quaternion rotation, Vector3 scale)
		{
			_position = position;
			_rotation = rotation;
			_scale = scale;
		}
	}
}
