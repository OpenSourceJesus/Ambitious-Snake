using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace AmbitiousSnake
{
	[ExecuteAlways]
	public class LevelTimer : SingletonMonoBehaviour<LevelTimer>
	{
		public Timer timer;
		public Text text;
		
		public virtual void OnEnable ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				if (text == null)
					text = GetComponent<Text>();
				return;
			}
#endif
		}
		
		public virtual void FixedUpdate ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
#endif
			text.text = string.Format("{0:F1}", timer.TimeElapsed);
			if (IsOverParTime())
				text.color = Color.red;
			else
				text.color = Color.black;
		}
		
		public virtual bool IsOverParTime ()
		{
			return IsOverParTime(timer.TimeElapsed);
		}

		public static bool IsOverParTime (float time)
		{
			return time > LevelSelect.parTimes[LevelSelect.PreviousLevelIndex];
		}
	}
}