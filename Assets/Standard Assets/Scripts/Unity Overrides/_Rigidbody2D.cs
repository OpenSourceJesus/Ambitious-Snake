using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

namespace AmbitiousSnake
{
	[ExecuteAlways]
	[RequireComponent(typeof(Rigidbody2D))]
	[DisallowMultipleComponent]
	public class _Rigidbody2D : MonoBehaviour
	{
		public static Rigidbody2D[] allInstances = new Rigidbody2D[0];
		public Rigidbody2D rigid;
		
		public virtual void Awake ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				if (rigid == null)
					rigid = GetComponent<Rigidbody2D>();
				return;
			}
#endif
			allInstances = allInstances.Add(rigid);
		}
		
		public virtual void OnDestroy ()
		{
			allInstances = allInstances.Remove(rigid);
		}
	}
}