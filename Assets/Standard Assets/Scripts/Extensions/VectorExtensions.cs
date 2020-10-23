using UnityEngine;

namespace Extensions
{
	public static class VectorExtensions
	{
		public static Vector3 NULL = new Vector3(MathfExtensions.NULL_FLOAT, MathfExtensions.NULL_FLOAT, MathfExtensions.NULL_FLOAT);
		
		public static Vector3 Snap (this Vector3 v, Vector3 snapTo)
		{
			return new Vector3(MathfExtensions.RoundToInterval(v.x, snapTo.x), MathfExtensions.RoundToInterval(v.y, snapTo.y), MathfExtensions.RoundToInterval(v.z, snapTo.z));
		}
		
		public static Vector2 Snap (this Vector2 v, Vector2 snapTo)
		{
			return new Vector2(MathfExtensions.RoundToInterval(v.x, snapTo.x), MathfExtensions.RoundToInterval(v.y, snapTo.y));
		}
		
		public static Vector3 Multiply (this Vector3 v1, Vector3 v2)
		{
			return new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
		}
		
		public static Vector3 Divide (this Vector3 v1, Vector3 v2)
		{
			return new Vector3(v1.x / v2.x, v1.y / v2.y, v1.z / v2.z);
		}
		
		public static Vector2 Rotate (this Vector2 v, float degrees)
		{
			float ang = GetAngleFrom(v) + degrees;
			ang *= Mathf.Deg2Rad;
			return new Vector2(Mathf.Cos(ang), Mathf.Sin(ang));
		}
		
		public static float GetAngleFrom (Vector2 v)
		{
			return Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
		}
		
		public static Vector2 GetFromAngle (float angle)
		{
			return Rotate(Vector2.right, angle);
		}
		
		public static Vector3 ClampComponents (this Vector3 v, Vector3 min, Vector3 max)
		{
			v.x = Mathf.Clamp(v.x, min.x, max.x);
			v.y = Mathf.Clamp(v.y, min.y, max.y);
			v.z = Mathf.Clamp(v.z, min.z, max.z);
			return v;
		}
		
		public static Vector3 ClampComponents (this Vector2 v, Vector2 min, Vector2 max)
		{
			v.x = Mathf.Clamp(v.x, min.x, max.x);
			v.y = Mathf.Clamp(v.y, min.y, max.y);
			return v;
		}
		
		public static Vector3 SetZ (this Vector3 v, float z)
		{
			v.z = z;
			return v;
		}
		
		public static bool IsInsideBounds (Vector3 v, Vector3 boundsExtents, bool canBeEqualTo)
		{
			if (canBeEqualTo)
				return v.x <= boundsExtents.x && v.x >= -boundsExtents.x && v.y <= boundsExtents.y && v.y >= -boundsExtents.y && v.z <= boundsExtents.z && v.z >= -boundsExtents.z;
			else
				return v.x < boundsExtents.x && v.x > -boundsExtents.x && v.y < boundsExtents.y && v.y > -boundsExtents.y && v.z < boundsExtents.z && v.z > -boundsExtents.z;
		}

		public static Vector2 ToVec2 (this Vector2Int v)
		{
			return new Vector2(v.x, v.y);
		}
	}
}
