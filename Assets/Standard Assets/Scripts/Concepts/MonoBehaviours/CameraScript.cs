using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AmbitiousSnake
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	[DisallowMultipleComponent]
	public class CameraScript : SingletonMonoBehaviour<CameraScript>
	{
		public Transform trs;
		public new Camera camera;
		public Vector2 viewSize;
		protected Rect normalizedScreenViewRect;
		protected float screenAspect;
		[HideInInspector]
		public Rect viewRect;
		
		public override void Awake ()
		{
			base.Awake ();
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				if (trs == null)
					trs = GetComponent<Transform>();
				if (camera == null)
					camera = GetComponent<Camera>();
				if (GameManager.Instance.doEditorUpdates)
					EditorApplication.update += DoEditorUpdate;
				return;
			}
			else
				EditorApplication.update -= DoEditorUpdate;
#endif
			// trs.SetParent(null);
			viewRect.size = viewSize;
			HandlePosition ();
			HandleViewSize ();
		}

#if UNITY_EDITOR
		void DoEditorUpdate ()
		{
			// HandleViewSize ();
		}

		void OnDestroy ()
		{
			if (Application.isPlaying)
				return;
			EditorApplication.update -= DoEditorUpdate;
		}

		void OnDisable ()
		{
			if (Application.isPlaying)
				return;
			EditorApplication.update -= DoEditorUpdate;
		}
#endif

		public void DoUpdate ()
		{
			HandlePosition ();
		}
		
		public virtual void HandlePosition ()
		{
			viewRect.center = trs.position;
		}
		
		void HandleViewSize ()
		{
			screenAspect = Screen.width / Screen.height;
			camera.aspect = viewSize.x / viewSize.y;
			camera.orthographicSize = Mathf.Min(viewSize.x / 2 / camera.aspect, viewSize.y / 2);
			normalizedScreenViewRect = new Rect();
			normalizedScreenViewRect.size = new Vector2(camera.aspect / screenAspect, Mathf.Min(1, screenAspect / camera.aspect));
			normalizedScreenViewRect.center = Vector2.one / 2;
			camera.rect = normalizedScreenViewRect;
		}
	}
}