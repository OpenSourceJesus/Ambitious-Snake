using UnityEngine;
using System.Collections;

namespace AmbitiousSnake
{
	public class DisplaySnakeFacing : NotPartOfLevelEditor
	{
		void FixedUpdate ()
		{
			transform.position = GetComponentInParent<Snake>().GetHeadPos();
			transform.right = GetComponentInParent<Snake>().facingVector;
		}
	}
}