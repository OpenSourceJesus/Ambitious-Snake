using UnityEngine;
using System.Collections.Generic;
using Extensions;

namespace AmbitiousSnake
{
	public class Wave : MonoBehaviour
	{
		public int difficulty;
		public ComplexTimer ComplexTimer;
		public Wave[] requiredWaves;
		public MoveObject[] moveObjects = new MoveObject[0];
		public Transform trs;
		
		public virtual void OnEnable ()
		{
			if (Random.Range(0, 2) == 1)
				trs.localScale = VectorExtensions.Multiply(trs.localScale, new Vector3(-1, 1, 1));
		}
		
		public virtual bool IsDone ()
		{
			foreach (MoveObject mover in moveObjects)
			{
				if (!mover.hasTraveledFullCycle)
					return false;
			}
			ComplexTimer.Resume();
			return ComplexTimer.IsAtEnd();
		}
	}
}