using UnityEngine;

[ExecuteAlways]
public class EditorCamera : SingletonMonoBehaviour<EditorCamera>
{
	public Camera cam;
	
	public override void Awake ()
	{
#if UNITY_EDITOR
		if (!Application.isPlaying)
		{
			if (cam == null)
				cam = GetComponent<Camera>();
			return;
		}
#endif
		base.Awake ();
		cam = GetComponent<Camera>();
		Camera.main.orthographicSize = cam.orthographicSize;
	}
}
