#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace ToolBox.Serialization.Editor
{
	internal sealed class AssetsContainerEditor : EditorWindow
	{
		private AssetsContainer _provider = null;
		private Vector2 _scroll = default;

		[MenuItem("Window/Assets Container")]
		private static void ShowWindow() =>
			GetWindow<AssetsContainerEditor>("Assets Container").Show();

		private void OnEnable() =>
			_provider = Resources.Load<AssetsContainer>("ToolBoxAssetsContainer");

		private void OnGUI()
		{
			var obj = new SerializedObject(_provider);
			obj.Update();
			var objectsProperty = obj.FindProperty("_savedAssets");
			var pathsProperty = obj.FindProperty("_paths");

			DrawPaths(pathsProperty);
			DrawButtons();
			DrawAssets(objectsProperty);

			obj.ApplyModifiedProperties();
		}

		private void DrawPaths(SerializedProperty pathsProperty)
		{
			var selectContent = new GUIContent("Select Path");
			var removeContent = new GUIContent("x");

			for (int i = 0; i < pathsProperty.arraySize; i++)
			{
				EditorGUILayout.BeginHorizontal();

				var element = pathsProperty.GetArrayElementAtIndex(i);
				EditorGUILayout.PropertyField(element, new GUIContent($"Path {i}"));

				if (GUILayout.Button(selectContent, EditorStyles.miniButtonLeft, GUILayout.Width(75f)))
				{
					string path = EditorUtility.OpenFolderPanel("Select path", "Assets", "");

					if (path != "Assets" || path.Length != 0)
						path = path.Substring(path.IndexOf("Assets"));

					if (AssetDatabase.IsValidFolder(path))
						element.stringValue = path;
					else
						pathsProperty.DeleteArrayElementAtIndex(i);
				}

				if (GUILayout.Button(removeContent, EditorStyles.miniButtonLeft, GUILayout.Width(30f)))
					pathsProperty.DeleteArrayElementAtIndex(i);

				EditorGUILayout.EndHorizontal();
			}

			GUILayout.Space(10f);

			if (GUILayout.Button("Add Path", GUILayout.Height(30f)))
				pathsProperty.InsertArrayElementAtIndex(pathsProperty.arraySize);
		}

		private void DrawButtons()
		{
			EditorGUILayout.BeginHorizontal();

			if (GUILayout.Button("Load assets at paths"))
				_provider.LoadAssets();

			if (GUILayout.Button("Remove assets from container"))
			{
				if (EditorUtility.DisplayDialog("Clear", "Do you really want to remove all referenced assets?", "Yes", "No"))
					_provider.Clear();
			}

			EditorGUILayout.EndHorizontal();
			GUILayout.Space(15f);
		}

		private void DrawAssets(SerializedProperty objectsProperty)
		{
			_scroll = EditorGUILayout.BeginScrollView(_scroll, true, true);

			GUI.enabled = false;
			EditorGUILayout.PropertyField(objectsProperty, true);
			GUI.enabled = true;

			EditorGUILayout.EndScrollView();
		}
	}
}
#endif