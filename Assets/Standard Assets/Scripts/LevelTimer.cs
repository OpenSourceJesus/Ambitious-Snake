using UnityEngine;
using UnityEngine.UI;
using Extensions;

namespace AmbitiousSnake
{
	[ExecuteAlways]
	public class LevelTimer : SingletonMonoBehaviour<LevelTimer>, IUpdatable
	{
		public bool PauseWhileUnfocused
		{
			get
			{
				return true;
			}
		}
		public Timer timer;
		public Text text;
		
		void OnEnable ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				if (text == null)
					text = GetComponent<Text>();
				return;
			}
#endif
			GameManager.updatables = GameManager.updatables.Add(this);
		}
		
		public void DoUpdate ()
		{
			text.text = string.Format("{0:F1}", timer.TimeElapsed);
			if (IsOverParTime())
				text.color = Color.red;
			else
				text.color = Color.black;
		}

		void OnDisable ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
#endif
			GameManager.updatables = GameManager.updatables.Remove(this);
		}
		
		public bool IsOverParTime ()
		{
			return IsOverParTime(timer.TimeElapsed);
		}

		public static bool IsOverParTime (float time)
		{
			return time > LevelSelect.parTimes[LevelSelect.PreviousLevelIndex];
		}
	}
}