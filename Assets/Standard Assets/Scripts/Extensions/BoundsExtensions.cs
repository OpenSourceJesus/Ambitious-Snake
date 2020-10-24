using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Extensions
{
	public static class BoundsExtensions
	{
		public static Bounds NULL = new Bounds(Vector3.zero, VectorExtensions.NULL);
		
		public static Bounds Combine (Bounds[] boundsArray)
		{
			if (boundsArray.Length == 0)
				return NULL;
			Bounds output = boundsArray[0];
			for (int i = 1; i < boundsArray.Length; i ++)
			{
				Bounds bounds = boundsArray[i];
				Vector3 min = output.min;
				Vector3 max = output.max;
				if (bounds.min.x < output.min.x)
					min.x = bounds.min.x;
				if (bounds.max.x > output.max.x)
					max.x = bounds.max.x;
				if (bounds.min.y < output.min.y)
					min.y = bounds.min.y;
				if (bounds.max.y > output.max.y)
					max.y = bounds.max.y;
				if (bounds.min.z < output.min.z)
					min.z = bounds.min.z;
				if (bounds.max.z > output.max.z)
					max.z = bounds.max.z;
				output.SetMinMax(min, max);
			}
			return output;
		}
	}
}