using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Extensions
{
	public static class RectTransformExtensions
	{
		public static Rect GetWorldRect (this RectTransform rectTrs)
		{
			Vector3[] worldCorners = new Vector3[4];
			rectTrs.GetWorldCorners(worldCorners);
			return Rect.MinMaxRect(worldCorners[0].x, worldCorners[0].y, worldCorners[2].x, worldCorners[2].y);
		}
		
		public static Vector2 GetCenterInCanvasNormalized (this RectTransform rectTrs, RectTransform canvasRectTrs)
		{
			return canvasRectTrs.GetWorldRect().ToNormalizedPosition(rectTrs.GetWorldRect().center);
		}

		public static Rect GetRectInCanvasNormalized (this RectTransform rectTrs, RectTransform canvasRectTrs)
		{
			Rect output = rectTrs.GetWorldRect();
			Rect canvasRect = canvasRectTrs.GetWorldRect();
			Vector2 outputMin = canvasRect.ToNormalizedPosition(output.min);
			Vector2 outputMax = canvasRect.ToNormalizedPosition(output.max);
			return Rect.MinMaxRect(outputMin.x, outputMin.y, outputMax.x, outputMax.y);
		}

		public static void CopyCorners (this RectTransform recTrs, RectTransform copy)
		{
			recTrs.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
			recTrs.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, 0);
			recTrs.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 0);
			recTrs.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, 0);
		}
	}
}