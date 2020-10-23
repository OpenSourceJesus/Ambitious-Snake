using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;


namespace uCP
{
	/// <summary>This component is used for input.</summary>
	[AddComponentMenu("uCP/Color Input Field")]
	[RequireComponent(typeof(InputField))]
	public partial class ColorInputField : MonoBehaviour
	{
		/// <summary>Relative value.</summary>
		public ColorAxis Property;

		/// <summary>Current InputField.</summary>
		[NonSerialized] public InputField inputField;

		/// <summary>Parent color picker object.</summary>
		[NonSerialized] public ColorPicker _colorPicker; // Parent. 
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

		protected virtual void Awake()
		{
			// Get components.
			inputField = GetComponent<InputField>();

			// Register events.
			inputField.onEndEdit.AddListener (endEdit);
			colorPicker.OnChange_HSV.AddListener (UpdateUI);
		}

		/// <summary>Event, for complete input.</summary>
		protected virtual void endEdit(string s)
		{
			if(!string.IsNullOrEmpty (s))
			{
				var f = System.Convert.ToSingle (s);

				if(Property == ColorAxis.Hue)
					f/=359;
				else
					f/=255;

				f = Mathf.Clamp01(f);

				var col = colorPicker.color;
				switch (Property)
				{
				case ColorAxis.Red:
					col.r = f;
					break;
				case ColorAxis.Green:
					col.g = f;
					break;
				case ColorAxis.Blue:
					col.b = f;
					break;
				}
				if(col != colorPicker.color)
					colorPicker.color = col;

				switch (Property)
				{
				case ColorAxis.Hue:
					colorPicker.hsv.h = f;
					break;
				case ColorAxis.Saturation:
					colorPicker.hsv.s = f;
					break;
				case ColorAxis.Value:
					colorPicker.hsv.v = f;
					break;
				case ColorAxis.Alpha:
					colorPicker.hsv.a = f;
					break;
				}

				colorPicker.UpdateUI();
			}
		}

		/// <summary>Change text, based on a picker's hsv.</summary>
		protected virtual void UpdateUI(HSV hsv)
		{
			switch (Property)
			{
			case ColorAxis.Red:
				inputField.text = (((Color)hsv).r*255).ToString("F0");
				break;
			case ColorAxis.Green:
				inputField.text = (((Color)hsv).g*255).ToString("F0");
				break;
			case ColorAxis.Blue:
				inputField.text = (((Color)hsv).b*255).ToString("F0");
				break;
			case ColorAxis.Hue:
				inputField.text = (hsv.h*359).ToString("F0"); // Only hue is 0-359.
				break;
			case ColorAxis.Saturation:
				inputField.text = (hsv.s*255).ToString("F0");
				break;
			case ColorAxis.Value:
				inputField.text = (hsv.v*255).ToString("F0");
				break;
			case ColorAxis.Alpha:
				inputField.text = (hsv.a*255).ToString("F0");
				break;
			}
		}
	}
}
