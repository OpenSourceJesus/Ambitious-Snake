using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Linq;


namespace uCP
{
	/// <summary>This component is used for picking color in rect area.</summary>
	[AddComponentMenu("uCP/Get Color In Rect")]
	public partial class GetColorInRect : MonoBehaviour, IPointerDownHandler, IDragHandler
	{
		/// <summary>Horizontal relative value.</summary>
		public ColorAxis X_Axis;
		/// <summary>Vertical relative value.</summary>
		public ColorAxis Y_Axis;
		/// <summary>When a color is changed, colorPointer go there.</summary>
		public RectTransform colorPointer;

		/// <summary>Parent canvas object.</summary>
		[NonSerialized] public Canvas _canvas; // Parent. 
		/// <summary>Parent canvas object.</summary>
		public Canvas canvas{
			get{
				if(_canvas == null)
					_canvas = GetComponentInParent<Canvas> ();
				return _canvas;
			}
			set{_canvas = value;}
		}
		/// <summary>Parent color picker object.</summary>
		[NonSerialized] public ColorPicker _colorPicker; // Parent. 
		/// <summary>Parent color picker object.</summary>
		public ColorPicker colorPicker{
			get{
				if(_colorPicker == null)
					_colorPicker = GetComponentInParent<ColorPicker> ();
				return _colorPicker;
			}
			set{
				if(_colorPicker != null)
					_colorPicker.OnChange_HSV.RemoveListener( UpdateUI );
				_colorPicker = value;
				_colorPicker.OnChange_HSV.AddListener( UpdateUI );
			}
		}
		[NonSerialized] public RectTransform rect;

		protected virtual void Start() // When it is "Awake()" can not instantiated.
		{
			// Get components.
			rect = GetComponent<RectTransform> ();

			// Register events.
			colorPicker.OnChange_HSV.AddListener( UpdateUI );
		}

		public virtual void OnPointerDown(PointerEventData e)
		{
			// Begin picking.
			OnDrag (e);
		}

		public virtual void OnDrag(PointerEventData e)
		{
			// Get position.
			Vector2 p;
			if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
				RectTransformUtility.ScreenPointToLocalPointInRectangle (rect, Input.mousePosition, null, out p);
			else if (canvas.worldCamera != null)
				RectTransformUtility.ScreenPointToLocalPointInRectangle (rect, Input.mousePosition, canvas.worldCamera, out p);
			else
				RectTransformUtility.ScreenPointToLocalPointInRectangle (rect, Input.mousePosition, Camera.main, out p);

			// Caluclate pivots.
			p.x += rect.rect.width * rect.pivot.x;
			p.y += rect.rect.height * rect.pivot.y;

			// Normalize.
			p.x /= rect.rect.width;
			p.y /= rect.rect.height;

			// Clamp.
			p.x = Mathf.Clamp01 (p.x);
			p.y = Mathf.Clamp01 (p.y);

			// Set picker's color.
			Color c = colorPicker.color;
			switch (X_Axis)
			{
			case ColorAxis.Red:
				c.r = p.x;
				colorPicker.color = c;
				break;
			case ColorAxis.Green:
				c.g = p.x;
				colorPicker.color = c;
				break;
			case ColorAxis.Blue:
				c.b = p.x;
				colorPicker.color = c;
				break;
			case ColorAxis.Hue:
				colorPicker.hsv.h = p.x;
				break;
			case ColorAxis.Saturation:
				colorPicker.hsv.s = p.x;
				break;
			case ColorAxis.Value:
				colorPicker.hsv.v = p.x;
				break;
			case ColorAxis.Alpha:
				colorPicker.hsv.a = p.x;
				break;
			case ColorAxis.SaturationAndValue:
				colorPicker.hsv.s = 1f - ((p.x - .5f) * 2);
				colorPicker.hsv.v = p.x * 2;
				break;
			default:
				break;
			}

			switch (Y_Axis)
			{
			case ColorAxis.Red:
				c.r = p.y;
				colorPicker.color = c;
				break;
			case ColorAxis.Green:
				c.g = p.y;
				colorPicker.color = c;
				break;
			case ColorAxis.Blue:
				c.b = p.y;
				colorPicker.color = c;
				break;
			case ColorAxis.Hue:
				colorPicker.hsv.h = p.y;
				break;
			case ColorAxis.Saturation:
				colorPicker.hsv.s = p.y;
				break;
			case ColorAxis.Value:
				colorPicker.hsv.v = p.y;
				break;
			case ColorAxis.Alpha:
				colorPicker.hsv.a = p.y;
				break;
			case ColorAxis.SaturationAndValue:
				colorPicker.hsv.s = 1f - ((p.y - .5f) * 2);
				colorPicker.hsv.v = p.y * 2;
				break;
			default:
				break;
			}

			// Fire events in parent object.
			colorPicker.UpdateUI ();
		}

		/// <summary>Update cursor, when change color.</summary>
		public virtual void UpdateUI (HSV hsv)
		{
			if (colorPointer == null)
				return;
		
			float x=0, y=0;
			var color = (Color)hsv;

			switch (X_Axis)
			{
			case ColorAxis.Red:
				x = color.r;
				break;
			case ColorAxis.Green:
				x = color.g;
				break;
			case ColorAxis.Blue:
				x = color.b;
				break;
			case ColorAxis.Hue:
				x = hsv.h;
				break;
			case ColorAxis.Saturation:
				x = hsv.s;
				break;
			case ColorAxis.Value:
				x = hsv.v;
				break;
			case ColorAxis.Alpha:
				x = hsv.a;
				break;
			case ColorAxis.SaturationAndValue:
				if (hsv.s >= 0 && hsv.v == 0)
					x = 1f - (hsv.s - 1f);
				else
					x = hsv.v / 2;
				break;
			default:
				x = 0.5f;
				break;
			}

			switch (Y_Axis)
			{
			case ColorAxis.Red:
				y = color.r;
				break;
			case ColorAxis.Green:
				y = color.g;
				break;
			case ColorAxis.Blue:
				y = color.b;
				break;
			case ColorAxis.Hue:
				y = hsv.h;
				break;
			case ColorAxis.Saturation:
				y = hsv.s;
				break;
			case ColorAxis.Value:
				y = hsv.v;
				break;
			case ColorAxis.Alpha:
				y = hsv.a;
				break;
			case ColorAxis.SaturationAndValue:
				if (hsv.s >= 0 && hsv.v == 0)
					y = 1f - (hsv.s - 1f);
				else
					y = hsv.v / 2;
				break;
			default:
				y = 0.5f;
				break;
			}

			// Caluculate localposition.
			x = (x - rect.pivot.x) * rect.rect.width;
			y = (y - rect.pivot.y) * rect.rect.height;

			// Set localposition.
			colorPointer.localPosition = new Vector2 (x,y);
		}
	}
}
