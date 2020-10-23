using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace uCP
{
	/// <summary>
	/// HSV color space struct.
	/// You can implicitly convert to UnityEngine.Color.
	/// Also you can compare, using either == or != to compare.
	/// </summary>
	[Serializable]
	public struct HSV
	{
		// About HSV color space.
		// https://en.wikipedia.org/wiki/HSL_and_HSV

		// Convert code from.
		// https://ja.wikipedia.org/wiki/HSV%E8%89%B2%E7%A9%BA%E9%96%93

		/// <summary>
		/// h=Hue | s=(Saturation Chroma) | v=(Value Lightness Brightness) | a=Alpha
		/// </summary>
		[SerializeField]
		public float h,s,v,a;

		public HSV (float H, float S, float V)
		{
			h = H;
			s = S;
			v = V;
			a = 1;
		}
		public HSV (float H, float S, float V, float A)
		{
			h = H;
			s = S;
			v = V;
			a = A;
		}

		/// <summary>( 0, 1, 1, 1 )</summary>
		public static readonly HSV red = new HSV( 0,1,1,1 );
		/// <summary>( 0.33, 1, 1, 1 )</summary>
		public static readonly HSV green = new HSV( 0.33f,1,1,1 );
		/// <summary>( 0.66, 1, 1, 1 )</summary>
		public static readonly HSV blue = new HSV( 0.66f,1,1,1 );
		/// <summary>( 0.5, 1, 1, 1 )</summary>
		public static readonly HSV cyan = new HSV( 0.5f,1,1,1 );
		/// <summary>( 0.83, 1, 1, 1 )</summary>
		public static readonly HSV magenta = new HSV( 0.83f,1,1,1 );
		/// <summary>( 0.16, 1, 1, 1 )</summary>
		public static readonly HSV yellow = new HSV( 0.16f,1,1,1 );
		/// <summary>( 0, 0, 1, 1 )</summary>
		public static readonly HSV white = new HSV( 0,0,1,1 );
		/// <summary>( 0, 0.5, 1, 1 )</summary>
		public static readonly HSV gray = new HSV( 0,0.5f,1,1 );
		/// <summary>( 0, 0.5, 1, 1 )</summary>
		public static readonly HSV grey = new HSV( 0,0.5f,1,1 );
		/// <summary>( 0, 0, 0, 1 )</summary>
		public static readonly HSV black = new HSV( 0,0,0,1 );
		/// <summary>( 0, 0, 0, 0 )</summary>
		public static readonly HSV clear = new HSV( 0,0,0,0 );
		
		/// <summary>Convert Color to HSV.</summary>
		public static HSV ColorToHSV(Color color)
		{
			float r = color.r;
			float g = color.g;
			float b = color.b;

			float max = r > g ? r : g;
			max = max > b ? max : b;
			float min = r < g ? r : g;
			min = min < b ? min : b;
			float h = max - min;
			if (h > 0.0f) {
				if (max == r) {
					h = (g - b) / h;
					if (h < 0.0f) {
						h += 6.0f;
					}
				} else if (max == g) {
					h = 2.0f + (b - r) / h;
				} else {
					h = 4.0f + (r - g) / h;
				}
			}
			h /= 6.0f;
			float s = (max - min);
			if (max != 0.0f)
				s /= max;
			float v = max;
			
			return new HSV (h, s, v, color.a);
		}

		/// <summary>Convert HSV to Color.</summary>
		public static Color HSVToColor(HSV hsv)
		{
			float h = hsv.h;
			float s = hsv.s;
			float v = hsv.v;
			float r = v;
			float g = v;
			float b = v;

			if (s > 0.0f) {
				h *= 6.0f;
				int i = (int) h;
				float f = h - (float) i;
				switch (i) {
				default:
				case 0:
					g *= 1 - s * (1 - f);
					b *= 1 - s;
					break;
				case 1:
					r *= 1 - s * f;
					b *= 1 - s;
					break;
				case 2:
					r *= 1 - s;
					b *= 1 - s * (1 - f);
					break;
				case 3:
					r *= 1 - s;
					g *= 1 - s * f;
					break;
				case 4:
					r *= 1 - s * (1 - f);
					g *= 1 - s;
					break;
				case 5:
					g *= 1 - s;
					b *= 1 - s * f;
					break;
				}
			}
			return new Color (r, g, b, hsv.a);
		}

		/// <summary>Return a string "(h,s,v,a)".</summary>
		public override string ToString (){ return string.Format ("({0},{1},{2},{3})",h,s,v,a); }

		public override bool Equals(object Obj)
		{
			if (Obj is HSV)
				return this.Equals((HSV)Obj);
			return false;
		}
		public bool Equals(HSV hsv){ return (h == hsv.h && s == hsv.s && v == hsv.v && a == hsv.a); }

		public override int GetHashCode()
		{
			return h.GetHashCode() ^
					s.GetHashCode() ^
					v.GetHashCode() ^
					a.GetHashCode();
		} 

		public static bool operator != (HSV a, HSV b){ return a.Equals (b); }
		public static bool operator == (HSV a, HSV b){ return !(a.Equals (b)); }

		public static implicit operator HSV(Color c){ return ColorToHSV(c); }
		public static implicit operator Color(HSV h){ return HSVToColor(h); }
	}
}
