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