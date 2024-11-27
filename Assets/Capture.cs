#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class Capture
{
	static Capture()
	{
		SceneView.duringSceneGui -= OnSceneGUI;

		SceneView.duringSceneGui += OnSceneGUI;
	}

	private static void OnSceneGUI(SceneView sceneView)
	{
		Event current = Event.current;

		if (current.type == EventType.KeyUp && current.keyCode == KeyCode.F12)
		{
			string path = EditorUtility.SaveFilePanel("", "Сохранить как..", "", "png");

			if (path.Length != 0)
			{
				Debug.Log("SCREEN");
				ScreenCapture.CaptureScreenshot(path);
			}

			Event.current.Use();
		}
	}
}
#endif
