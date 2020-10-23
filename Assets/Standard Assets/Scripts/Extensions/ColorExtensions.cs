using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Extensions
{
	public static class ColorExtensions
	{
		public static Color NULL;
		
		public static Color SetAlpha (this Color c, float a)
		{
			c.a = a;
			return c;
		}
		
		public static Color AddAlpha (this Color c, float a)
		{
			c.a += a;
			return c;
		}
		
		public static Color MultiplyAlpha (this Color c, float a)
		{
			c.a *= a;
			return c;
		}
		
		public static Color DivideAlpha (this Color c, float a)
		{
			c.a /= a;
			return c;
		}
	}
}