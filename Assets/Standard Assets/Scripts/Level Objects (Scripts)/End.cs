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
			if (!GameManager.GetSingleton<LevelTimer>().IsOverParTime() && LevelTimer.IsOverParTime(levelTime))
				GameManager.Score ++;
			if (GameManager.GetSingleton<LevelTimer>().timer.TimeElapsed < levelTime)
				SaveAndLoadManager.SetValue (LevelMap.previousLevelName + " Time", GameManager.GetSingleton<LevelTimer>().timer.TimeElapsed);
			if (Snake.hasStar)
			{
				if (!SaveAndLoadManager.GetValue<bool>(LevelMap.previousLevelName + " Got Star", false))
					GameManager.Score ++;
				SaveAndLoadManager.SetValue (LevelMap.previousLevelName + " Got Star", true);
			}
			GameManager.GetSingleton<SaveAndLoadManager>().Save ();
			GameManager.GetSingleton<UnlockablesManager>().GetUnlocks ();
			GameManager.GetSingleton<WinAnimation>().enabled = true;
		}
	}
}