using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Extensions;

namespace AmbitiousSnake
{
	public class End : NotPartOfLevelEditor
	{
		public virtual void OnTriggerEnter2D (Collider2D other)
		{
			float levelTime = SaveAndLoadManager.GetValue<float>(LevelMap.previousLevelName + " Time", Mathf.Infinity);
			if (levelTime == Mathf.Infinity)
				GameManager.Score ++;
			if (!LevelTimer.Instance.IsOverParTime() && LevelTimer.IsOverParTime(levelTime))
				GameManager.Score ++;
			if (LevelTimer.Instance.timer.TimeElapsed < levelTime)
				SaveAndLoadManager.SetValue (LevelMap.previousLevelName + " Time", LevelTimer.Instance.timer.TimeElapsed);
			if (Snake.hasStar)
			{
				if (!SaveAndLoadManager.GetValue<bool>(LevelMap.previousLevelName + " Got Star", false))
					GameManager.Score ++;
				SaveAndLoadManager.SetValue (LevelMap.previousLevelName + " Got Star", true);
			}
			SaveAndLoadManager.Instance.Save ();
			UnlockablesManager.Instance.GetUnlocks ();
			WinAnimation.instance.enabled = true;
		}
	}
}