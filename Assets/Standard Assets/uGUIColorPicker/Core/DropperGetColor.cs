using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;

namespace uCP
{
	public class DropperGetColor : MonoBehaviour, IPointerClickHandler
	{
		/// <summary>For callback event.</summary>
		[SerializeField] public GetColorEvent OnGetColor;

		/// <summary>Screen texture.</summary>
		[NonSerialized] public Texture2D texture;// = new Texture2D(1,1, TextureFormat.RGB24, false);

		/// <summary>It's true, when clicked</summary>
		[NonSerialized] public bool isClicked;
		/// <summary></summary>
		[NonSerialized] public Vector2 ClickedPosition = Vector2.zero;
		
		void Awake ()
		{
			texture = Texture2D.CreateExternalTexture(1,1, TextureFormat.RGB24, false, false, IntPtr.Zero);
		}
		
		public void OnGUI()
		{
			if(isClicked)
			{
				texture.ReadPixels(new Rect(ClickedPosition.x, ClickedPosition.y, 1, 1), 0, 0);
				OnGetColor.Invoke(texture.GetPixel(0,0));
				Destroy(gameObject);
			}
		}

		public void OnPointerClick(PointerEventData p)
		{
			isClicked = true;
			ClickedPosition = p.position;
		}

		// For callback event.
		[Serializable] public class GetColorEvent : UnityEvent<Color>{}
	}
}