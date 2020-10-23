using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.SceneManagement;
using System;
using Extensions;

namespace AmbitiousSnake
{
	[ExecuteAlways]
	public class LevelSelect : SingletonMonoBehaviour<LevelSelect>
	{
		public Transform trs;
		public bool addNewButton;
		public int _firstLevelBuildIndex;
		public static int firstLevelBuildIndex;
		public LevelButton levelButtonPrefab;
		public float[] _parTimes = new float[0];
		public static float[] parTimes = new float[0];
		public int[] _scoresToUnlock = new int[0];
		public static int[] scoresToUnlock = new int[0];
		public Text scoreText;
		public Button survivalButton;
		public Text levelTitle;
		public Button startButton;
		public static int PreviousLevelIndex
		{
			get
			{
				return SaveAndLoadManager.GetValue<int>("Previous level", 0);
			}
			set
			{
				SaveAndLoadManager.SetValue("Previous level", value);
			}
		}
		public LevelButton[] levelButtons;
		
		public override void Awake ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
#endif
			base.Awake ();
			firstLevelBuildIndex = _firstLevelBuildIndex;
			parTimes = _parTimes;
			scoresToUnlock = _scoresToUnlock;
			scoreText.text = "Score: " + GameManager.Score;
			survivalButton.interactable = Survival.Unlocked;
			LevelMap.previousLevelName = "";
			levelButtons = GetComponentsInChildren<LevelButton>();
			Level level;
			foreach (LevelButton levelButton in levelButtons)
			{
				levelButton.level = new Level();
				levelButton.level.name = levelButton.nameText.text;
				levelButton.Start ();
			}
			LevelButton previousLevelButton = levelButtons[PreviousLevelIndex];
			previousLevelButton.Start ();
			previousLevelButton.SetLevel ();
			GameManager.GetSingleton<UIControlManager>().ChangeSelected (previousLevelButton);
			startButton.onClick.AddListener(StartSelectedLevel);
		}
		
#if UNITY_EDITOR
		public virtual void Update ()
		{
			if (addNewButton)
			{
				addNewButton = false;
				LevelButton button = Instantiate(levelButtonPrefab);
				button.trs.SetParent(trs);
				button.trs.localScale = Vector3.one;
				EditorBuildSettingsScene scene = EditorBuildSettings.scenes[_firstLevelBuildIndex + button.trs.GetSiblingIndex()];
				string sceneName = scene.path.Substring(scene.path.LastIndexOf("/") + 1).Replace(".unity", "");
				button.name = sceneName + " [" + button.trs.GetSiblingIndex() + "]";
				button.levelName = sceneName;
				button.nameText.text = sceneName;
			}
			foreach (LevelButton levelButton in levelButtons)
			{
				levelButton.level.extraScenes = levelButton.extraScenes;
				levelButton.level.name = levelButton.levelName;
			}
		}
#endif

		public virtual void StartSelectedLevel ()
		{
			levelButtons[PreviousLevelIndex].LoadLevel ();
		}
	}
}