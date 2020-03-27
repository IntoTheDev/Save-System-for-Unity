using Sirenix.OdinInspector;
using Sirenix.Serialization;
using ToolBox.Observer;
using UnityEngine;

public abstract class Serializer : SerializedMonoBehaviour, IGameEventListener
{
	[SerializeField, AssetSelector, Required] private GameEvent onSerializationRequired = null;

	[OdinSerialize] protected ISerializable[] serializables = null;

	private void Awake() =>
		onSerializationRequired.AddListener(this);

	private void OnEnable() =>
		onSerializationRequired.AddListener(this);

	private void OnDisable() =>
		onSerializationRequired.RemoveListener(this);

	public void OnEventRaised() =>
		Serialize();

#if UNITY_EDITOR
	[Button("Set Serializables", ButtonSizes.Medium)]
	private void SetSerializables() =>
		serializables = GetComponents<ISerializable>();
#endif

	protected abstract void Serialize();
}
