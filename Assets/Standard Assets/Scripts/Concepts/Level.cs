using UnityEngine;
using System;
using Extensions;
using AmbitiousSnake;
using UnityEngine.SceneManagement;
using AmbitiousSnake.Analytics;

[Serializable]
public class Level : IUpdatable
{
	public string name;
	public static bool hasStar;
	public static bool KeptStar
	{
		get
		{
			return SaveAndLoadManager.GetValue<bool>(GameManager.GetSingleton<Level>().name + " Kept Star", false);
		}
		set
		{
			SaveAndLoadManager.SetValue (GameManager.GetSingleton<Level>().name + " Kept Star", value);
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
		hasStar = false;
		GameManager.onLevelTransitionDone += GameManager.GetSingleton<GameManager>().OnLevelLoaded;
		GameManager.updatables = GameManager.updatables.Add(this);
	}

	public virtual void DoUpdate ()
	{
		if (InputManager.RestartLevelInput)
			Restart ();
	}

	public virtual void Restart ()
	{
		hasStar = false;
		if (GameManager.GetSingleton<CommunityLevelHub>() == null)
		{
			GameManager.GetSingleton<ObjectPool>().DespawnAll ();
			GameManager.GetSingleton<GameManager>().screenEffectAnimator.Play("Opaque Screen");
			GameManager.GetSingleton<GameManager>().UnloadLevelAsync (LevelMap.previousLevelName);
			GameManager.GetSingleton<LevelTimer>().timer.Reset ();
			SceneManager.sceneLoaded += GameManager.GetSingleton<GameManager>().FadeIn;
			GameManager.onLevelTransitionDone += GameManager.GetSingleton<GameManager>().OnLevelLoaded;
			GameManager.GetSingleton<GameManager>().LoadLevel (LevelMap.previousLevelName, LoadSceneMode.Additive);
		}
		else
			GameManager.GetSingleton<CommunityLevelHub>().currentLevel.ReloadLevel ();
		if (!GameManager.paused)
			SnakeRecorder.areRecording = new SnakeRecorder[0];
		AnalyticsManager.PlayerDiedEvent playerDiedEvent = new AnalyticsManager.PlayerDiedEvent();
		GameManager.GetSingleton<AnalyticsManager>().LogEvent (playerDiedEvent);
	}
}