using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;


namespace uCP
{
	/// <summary>Color picker UI.</summary>
	[AddComponentMenu("uCP/Color Picker")]
	public partial class ColorPicker : MonoBehaviour
	{
		[Serializable] public class UnityEvent_Color : UnityEvent<Color>{}
		[Serializable] public class UnityEvent_HSV : UnityEvent<HSV>{}
		
		/// <summary>It's called, when change values.</summary>
		public UnityEvent_Color OnChange;
		/// <summary>OnChange event for relative functions.</summary>
		[HideInInspector] public UnityEvent_Color OnChange_Color;
		/// <summary>OnChange event for relative functions.</summary>
		[HideInInspector] public UnityEvent_HSV OnChange_HSV;

		/// <summary>Current and default HSV type color.</summary>
		public HSV hsv = HSV.red;
		/// <summary>Current Color.</summary>
		public virtual Color color
		{
			get{ return (Color)hsv; }
			set
			{
				var tempHsv = hsv;
				hsv = (HSV)value;
				
				// If the color is black or white, keep h value.
				if(value == Color.black || value == Color.white)
					hsv.h = tempHsv.h;
			}
		}

		public float outputMultiplier = 1;

		protected virtual void Start()
		{
			// Initializer.
			UpdateUI ();
		}

		/// <summary>Set color by code, like "#FFFFFF"</summary>
		public virtual void SetColorByColorCode(string code)
		{
			try 
			{
				if(code.Length != 7 || code[0] != '#')
					Debug.LogWarning("Can't get a color code.");

				var tr = code.Substring (1,2);
				var tg = code.Substring (3,2);
				var tb = code.Substring (5,2);
				var ir = Convert.ToInt32( tr, 16 );
				var ig = Convert.ToInt32( tg, 16 );
				var ib = Convert.ToInt32( tb, 16 );
				var r = ir/255;
				var g = ig/255;
				var b = ib/255;

				color = new Color(r,g,b);
				UpdateUI();
			}
			catch (Exception ex)
			{
				Debug.Log (ex.Message);
			}
		}

		/// <summary>Initialize and show the color picker.</summary>
		public virtual void Show(Color color){ Show ((HSV)color); }

		/// <summary>Initialize and show the color picker.</summary>
		public virtual void Show(HSV hsvColor)
		{
			// Convert color.
			hsv = hsvColor;

			// For reflesh UI.
			UpdateUI ();

			// Show picker.
			gameObject.SetActive (true);
		}

		/// <summary>Check values and invoke events.</summary>
		public virtual void UpdateUI()
		{
			var c = color;

			// It's for output.
			if (OnChange != null)
				OnChange.Invoke (c*outputMultiplier);

			// It's for relative objects.
			if (OnChange_Color != null)
				OnChange_Color.Invoke (c);

			// It's for relative objects.
			if (OnChange_HSV != null)
				OnChange_HSV.Invoke (hsv);
		}
	}
}