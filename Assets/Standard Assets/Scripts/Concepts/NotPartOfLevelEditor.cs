using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmbitiousSnake
{
	public class NotPartOfLevelEditor : MonoBehaviour
	{
		public virtual void Awake ()
		{
			if (LevelEditor.Instance != null)
				DestroyImmediate(this);
		}
	}
}