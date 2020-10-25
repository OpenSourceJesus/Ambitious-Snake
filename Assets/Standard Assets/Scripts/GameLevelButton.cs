using System.Collections;
using UnityEngine;
using Extensions;

namespace AmbitiousSnake
{
	public class GameLevelButton : LevelButton
	{
		public int scoreToUnlock;
		bool unlocked;
		string displayName;
		[SerializeField]
		[HideInInspector]
		int siblingIndex;
		
		public override void Start ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				if (trs == null)
					trs = GetComponent<Transform>();
				siblingIndex = trs.GetSiblingIndex();
				button.colors = unfinishedColors;
				return;
			}
#endif
			base.Start ();
			level.name = levelName;
			name = levelName;
			nameText.text = levelName;
			parText.enabled = unlocked;
			completed = LevelTime < Mathf.Infinity;
			gotParTime = LevelTime <= parTime;
			if (unlocked)
			{
				if (completed)
				{
					parText.text += string.Format("\n" + "Time: {0:F1}", LevelTime);
					if (GotStar && gotParTime)
						button.colors = acedColors;
					else
						button.colors = completedColors;
				}
				starIcon.SetActive(GotStar);
				parIcon.SetActive(gotParTime);
				button.onClick.RemoveAllListeners ();
				button.onClick.AddListener(delegate { SetLevel (); });
			}
			else
				nameText.text = "Unlock: " + scoreToUnlock + " Score";

			scoreToUnlock = LevelSelect.scoresToUnlock[siblingIndex];
			unlocked = GameManager.Score >= scoreToUnlock;
			button.interactable = unlocked;
			parText.enabled = unlocked;
			if (unlocked)
			{
				name = levelName;
				if (levelName.Contains(" ("))
					displayName = levelName.Substring(0, levelName.IndexOf(" ("));
				else
					displayName = levelName;
				nameText.text = displayName;
				completed = LevelTime < Mathf.Infinity;
				gotParTime = LevelTime <= LevelSelect.parTimes[siblingIndex];
				if (completed)
				{
					nameText.text += string.Format("\n" + "Time: {0:F1}", LevelTime);
					if (GotStar && gotParTime)
						button.colors = acedColors;
					else
						button.colors = completedColors;
				}
				starIcon.SetActive(GotStar);
				parIcon.SetActive(gotParTime);
				button.onClick.RemoveAllListeners ();
				button.onClick.AddListener(delegate { SetLevel (); });
			}
			else
				nameText.text = "Unlock: " + scoreToUnlock + " Score";
		}

#if UNITY_EDITOR
		public override void Update ()
		{
			base.Update ();
			if (Application.isPlaying)
				return;
			LevelSelect.Instance._parTimes[siblingIndex] = parTime;
			LevelSelect.Instance._scoresToUnlock[siblingIndex] = scoreToUnlock;
			parText.text = "Par: " + "\n" + parTime;
		}
#endif

		public override void SetLevel ()
		{
			// GameManager.Instance.UnloadLevel (levelName);
			// GameManager.Instance.LoadLevelAdditive (levelName);
			LevelMap.Instance.Make (levelName);
			LevelSelect.Instance.levelTitle.text = levelName;
			base.SetLevel ();
		}
		
		public override void LoadLevel ()
		{
			base.LoadLevel ();
			GameManager.Instance.StartCoroutine(LoadLevelRoutine ());
		}

		IEnumerator LoadLevelRoutine ()
		{
			button.onClick.RemoveAllListeners();
			LevelSelect.Instance.startButton.onClick.RemoveAllListeners();
			LevelSelect.PreviousLevelIndex = siblingIndex;
			// SaveAndLoadManager.Instance.Save ();
			GameManager.Instance.UnloadLevelAsync ("Level Select");
			yield return new WaitUntil(() => (GameManager.unloadLevel.isDone));
			GameManager.Instance.LoadLevelAdditive ("Level");
			foreach (string sceneName in extraScenes)
				GameManager.Instance.LoadLevelAdditive (sceneName);
			Level.instance = level;
			Level.instance.Start ();
			GameManager.updatables = GameManager.updatables.Add(level);
			yield return new WaitUntil(() => (GameManager.Instance.screenEffectAnimator.GetCurrentAnimatorStateInfo(0).IsName("Invisible Screen")));
			yield return new WaitForEndOfFrame();
			GameManager.Instance.SetPaused (false);
		}
	}
}