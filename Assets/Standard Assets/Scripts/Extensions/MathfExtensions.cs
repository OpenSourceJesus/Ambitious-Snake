using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Extensions
{
	public static class MathfExtensions
	{
		public const int NULL_INT = 1234567890;
		public const float NULL_FLOAT = NULL_INT;
		public const float INCHES_TO_CENTIMETERS = 2.54f;
		
		public static int Sign (float f)
		{
			if (f != 0)
				f = Mathf.Sign(f);
			return (int) f;
		}
		
		public static int RotationDirectionToAngle (float fromAngle, float toAngle)
		{
			return Sign(360 - (NormalizeAngle(toAngle) - NormalizeAngle(fromAngle)));
		}
		
		public static float NormalizeAngle (float angle)
		{
			while (angle < 0 || angle > 360)
				angle -= 360 * Mathf.Sign(angle);
			return angle;
		}
		
		public static float RoundToInterval (float f, float interval)
		{
			return Mathf.Round(f / interval) * interval;
		}
		
		public static bool DoLineSegmentsIntersect (Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
		{
			Vector2 a = p2 - p1;
			Vector2 b = p3 - p4;
			Vector2 c = p1 - p3;
			float alphaNumerator = b.y * c.x - b.x * c.y;
			float alphaDenominator = a.y * b.x - a.x * b.y;
			float betaNumerator  = a.x * c.y - a.y * c.x;
			float betaDenominator  = a.y * b.x - a.x * b.y;
			bool doIntersect = true;
			if (alphaDenominator == 0 || betaDenominator == 0)
				doIntersect = false;
			else
			{
				if (alphaDenominator > 0)
				{
					if (alphaNumerator < 0 || alphaNumerator > alphaDenominator)
						doIntersect = false;
				}
				else if (alphaNumerator > 0 || alphaNumerator < alphaDenominator)
					doIntersect = false;
				if (doIntersect && betaDenominator > 0)
				{
					if (betaNumerator < 0 || betaNumerator > betaDenominator)
						doIntersect = false;
				}
				else if (betaNumerator > 0 || betaNumerator < betaDenominator)
					doIntersect = false;
			}
			return doIntersect;
		}
		
		public static float GetCloserToZero (float number, float amount)
		{
			return (Mathf.Abs(number) - amount) * Sign(number);
		}
		
		public static float SnapToInterval (float f, float interval)
		{
			if (interval == 0)
				return f;
			else
				return Mathf.Round(f / interval) * interval;
		}
		
		public static bool AreOppositeSigns (float f1, float f2)
		{
			return Mathf.Abs(Sign(f1) - Sign(f2)) == 2;
		}
		
		public enum RoundingMethod
		{
			HalfOrMoreRoundsUp,
			HalfOrLessRoundsDown,
			RoundUpIfNotWhole,
			RoundDownIfNotWhole
		}

		public static float GetClosestNumber (float f, params float[] numbers)
		{
			float closestNumber = numbers[0];
			float number;
			for (int i = 1; i < numbers.Length; i ++)
			{
				number = numbers[i];
				if (Mathf.Abs(f - number) < Mathf.Abs(f - closestNumber))
					closestNumber = number;
			}
			return closestNumber;
		}

		public static int GetIndexOfClosestNumber (float f, params float[] numbers)
		{
			int indexOfClosestNumber = 0;
			float closestNumber = numbers[0];
			float number;
			for (int i = 1; i < numbers.Length; i ++)
			{
				number = numbers[i];
				if (Mathf.Abs(f - number) < Mathf.Abs(f - closestNumber))
				{
					closestNumber = number;
					indexOfClosestNumber = i;
				}
			}
			return indexOfClosestNumber;
		}

		public static float RegularizeAngle (float angle)
		{
			while (angle >= 360 || angle < 0)
				angle += Mathf.Sign(360 - angle) * 360;
			return angle;
		}
	}
}