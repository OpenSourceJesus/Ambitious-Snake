using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace uCP
{
	/// <summary>Extended GradationUI component. (dynamically change colors)</summary>
	[AddComponentMenu("uCP/Gradation Rect")]
	public partial class GradationRect : GradationUI
	{
		/// <summary>Horizontal relative value.</summary>
		public ColorAxis X_Axis;
		/// <summary>Vertical relative value.</summary>
		public ColorAxis Y_Axis;

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
				{
					_colorPicker.OnChange_Color.RemoveListener( UpdateUIC );
					_colorPicker.OnChange_HSV.RemoveListener( UpdateUIH );
				}
				_colorPicker = value;
				_colorPicker.OnChange_Color.AddListener( UpdateUIC );
				_colorPicker.OnChange_HSV.AddListener( UpdateUIH );
			}
		}

		protected override void Awake()
		{
			if (!Application.isPlaying)
				return;

			base.Awake ();

			// Register events.
			colorPicker.OnChange_Color.AddListener( UpdateUIC );
			colorPicker.OnChange_HSV.AddListener( UpdateUIH );
		}

		/// <summary>Change colors, based on a picker's color.</summary>
		public virtual void UpdateUIC(Color color)
		{
			if (!useAlpha)
				color.a = 1;

			Color[] cols = new Color[4]{color,color,color,color};
			bool changed = false;

			switch (X_Axis)
			{
			case ColorAxis.Red:
				cols[0].r = cols[1].r = 0;
				cols[2].r = cols[3].r = 1;
				changed = true;
				break;
			case ColorAxis.Green:
				cols[0].g = cols[1].g = 0;
				cols[2].g = cols[3].g = 1;
				changed = true;
				break;
			case ColorAxis.Blue:
				cols[0].b = cols[1].b = 0;
				cols[2].b = cols[3].b = 1;
				changed = true;
				break;
			}
			switch (Y_Axis)
			{
			case ColorAxis.Red:
				cols[0].r = cols[3].r = 0;
				cols[1].r = cols[2].r = 1;
				changed = true;
				break;
			case ColorAxis.Green:
				cols[0].g = cols[3].g = 0;
				cols[1].g = cols[2].g = 1;
				changed = true;
				break;
			case ColorAxis.Blue:
				cols[0].b = cols[3].b = 0;
				cols[1].b = cols[2].b = 1;
				changed = true;
				break;
			}

			if (changed)
			{
				colors = cols;
				UpdateColors ();
			}
		}

		/// <summary>Change colors, based on a picker's hsv.</summary>
		public virtual void UpdateUIH(HSV hsv)
		{
			if (!useAlpha)
				hsv.a = 1;

			HSV[] hsvs = new HSV[4]{hsv,hsv,hsv,hsv};
			bool changed = false;

			switch (X_Axis)
			{
			case ColorAxis.Hue:
				hsvs[0].h = hsvs[1].h = 0;
				hsvs[2].h = hsvs[3].h = 1;
				changed = true;
				break;
			case ColorAxis.Saturation:
				hsvs[0].s = hsvs[1].s = 0;
				hsvs[2].s = hsvs[3].s = 1;
				changed = true;
				break;
			case ColorAxis.Value:
				hsvs[0].v = hsvs[1].v = 0;
				hsvs[2].v = hsvs[3].v = 1;
				changed = true;
				break;
			case ColorAxis.Alpha:
				hsvs[0].a = hsvs[1].a = 0;
				hsvs[2].a = hsvs[3].a = 1;
				changed = true;
				break;
			}

			switch (Y_Axis)
			{
			case ColorAxis.Hue:
				hsvs[0].h = hsvs[3].h = 0;
				hsvs[1].h = hsvs[2].h = 1;
				changed = true;
				break;
			case ColorAxis.Saturation:
				hsvs[0].s = hsvs[3].s = 0;
				hsvs[1].s = hsvs[2].s = 1;
				changed = true;
				break;
			case ColorAxis.Value:
				hsvs[0].v = hsvs[3].v = 0;
				hsvs[1].v = hsvs[2].v = 1;
				changed = true;
				break;
			case ColorAxis.Alpha:
				hsvs[0].a = hsvs[3].a = 0;
				hsvs[1].a = hsvs[2].a = 1;
				changed = true;
				break;
			}
			
			if (changed)
			{
				colors[0] = hsvs[0];
				colors[1] = hsvs[1];
				colors[2] = hsvs[2];
				colors[3] = hsvs[3];
				UpdateColors ();
			}
		}
	}
}
