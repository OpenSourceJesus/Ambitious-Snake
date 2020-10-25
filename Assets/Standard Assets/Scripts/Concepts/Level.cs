using System;
using AmbitiousSnake;
using UnityEngine.SceneManagement;
// using AmbitiousSnake.Analytics;

[Serializable]
public class Level : IUpdatable
{
	public static Level instance;
	public string name;
	public static bool hasStar;
	public static bool KeptStar
	{
		get
		{
			return SaveAndLoadManager.GetValue<bool>(Level.instance.name + " Kept Star", false);
		}
		set
		{
			SaveAndLoadManager.SetValue (Level.instance.name + " Kept Star", value);
		}
	}
	public bool PauseWhileUnfocused
	{
		get
		{
			return true;
		}
	}
	public string restartButton;
	public string[] extraScenes = new string[0];

	public virtual void Start ()
	{
		Restart ();
	}

	public virtual void DoUpdate ()
	{
		if (InputManager.RestartLevelInput)
			Restart ();
	}

	public virtual void Restart ()
	{
		hasStar = false;
		if (CommunityLevelHub.Instance == null)
		{
			ObjectPool.Instance.DespawnAll ();
			GameManager.Instance.screenEffectAnimator.Play("Opaque Screen");
			if (LevelTimer.Instance != null)
			{
				GameManager.Instance.UnloadLevelAsync (LevelMap.previousLevelName);
				LevelTimer.Instance.timer.Reset ();
			}
			SceneManager.sceneLoaded += GameManager.Instance.FadeIn;
			GameManager.onLevelTransitionDone += GameManager.Instance.OnLevelLoaded;
			GameManager.Instance.LoadLevel (LevelMap.previousLevelName, LoadSceneMode.Additive);
		}
		else
			CommunityLevelHub.Instance.currentLevel.ReloadLevel ();
		if (!GameManager.paused)
			SnakeRecorder.areRecording = new SnakeRecorder[0];
		// AnalyticsManager.PlayerDiedEvent playerDiedEvent = new AnalyticsManager.PlayerDiedEvent();
		// AnalyticsManager.Instance.LogEvent (playerDiedEvent);
	}
}