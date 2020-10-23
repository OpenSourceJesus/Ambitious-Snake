using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace uCP
{
	/// <summary>This component is used for Hue. ( rainbow color rect )</summary>
	[AddComponentMenu("uCP/Alpha Rect")]
	public partial class AlphaRect : GradationUI
	{
		/// <summary>Relative value.</summary>
		public ColorAxis Property;
		/// <summary>If true, alpha will be opposite to [Property].</summary>
		public bool InverseValue;

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

		protected override void Awake()
		{
			if (!Application.isPlaying)
				return;
			
			base.Awake ();

			// Register event.
			colorPicker.OnChange_HSV.AddListener( UpdateUI );
		}

		/// <summary>Change colors, based on a picker's color.</summary>
		public virtual void UpdateUI(HSV hsv)
		{
			var color = (Color)hsv;
			float alpha = 1f;

			switch (Property)
			{
			case ColorAxis.Red:
				alpha = color.r;
				break;
			case ColorAxis.Green:
				alpha = color.g;
				break;
			case ColorAxis.Blue:
				alpha = color.b;
				break;
			case ColorAxis.Hue:
				alpha = hsv.h;
				break;
			case ColorAxis.Saturation:
				alpha = hsv.s;
				break;
			case ColorAxis.Value:
				alpha = hsv.v;
				break;
			case ColorAxis.Alpha:
				alpha = hsv.a;
				break;
			}

			colors [0].a = colors [1].a = colors [2].a = colors [3].a = (InverseValue ? 1 - alpha : alpha);
			UpdateColors ();
		}
	}
}