using System;
using UnityEngine;

namespace ToolBox.Serialization
{
	internal sealed class ApplicationStateObserver : MonoBehaviour
	{
		public event Action OnQuit;

#if !UNITY_IOS && !UNITY_ANDROID
		private void OnApplicationQuit()
		{
			OnQuit?.Invoke();
		}
#else
		private void OnApplicationPause(bool pause)
		{
			if (pause)
				OnQuit?.Invoke();
		}

		private void OnApplicationFocus(bool focus)
		{
			if (!focus)
				OnQuit?.Invoke();
		}
#endif
	}
}
