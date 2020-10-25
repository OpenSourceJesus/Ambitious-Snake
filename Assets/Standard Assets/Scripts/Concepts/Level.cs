using System;
using AmbitiousSnake;
using UnityEngine.SceneManagement;
// using AmbitiousSnake.Analytics;

[Serializable]
public class Level : IUpdatable
{
	public static Level instance;
	public string name;
	public bool PauseWhileUnfocused
	{
		get
		{
			return true;
		}
	}
	public string restartButton;
	public string[] extraScenes = new string[0];
	bool restartLevelInput;
	bool previousRestartLevelInput;

	public virtual void Start ()
	{
		Restart ();
	}

	public virtual void DoUpdate ()
	{
		restartLevelInput = InputManager.RestartLevelInput;
		if (restartLevelInput && !previousRestartLevelInput)
			Restart ();
		previousRestartLevelInput = restartLevelInput;
	}

	public virtual void Restart ()
	{
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