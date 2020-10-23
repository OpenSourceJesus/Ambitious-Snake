using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Extensions;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AmbitiousSnake
{
	[ExecuteAlways]
	public class UnlockablesManager : SingletonMonoBehaviour<UnlockablesManager>, ISavableAndLoadable
	{
		public WinAnimation winAnimPrefab;
		public Transform trs;
		public Unlock[] unlocks = new Unlock[0];
		public static Dictionary<Unlock, int> unlockCosts = new Dictionary<Unlock, int>();
		[SaveAndLoadValue]
		public static int previousScore = -1;
		[SaveAndLoadValue]
		public static int unlockedCount;
		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}
		[SerializeField]
		int uniqueId;
		public int UniqueId
		{
			get
			{
				return uniqueId;
			}
			set
			{
				uniqueId = value;
			}
		}
		
		public override void Awake ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				if (trs == null)
					trs = GetComponent<Transform>();
				Text[] texts = GetComponentsInChildren<Text>();
				for (int i = 0; i < texts.Length; i ++)
					unlocks[i].text = texts[i];
				GameManager.SetUniqueId (this);
				return;
			}
#endif
			base.Awake ();
			return;
			SetCosts ();
			Unlock unlock;
			for (int i = 0; i < unlockedCount; i ++)
			{
				unlock = unlocks[i];
				unlock.text.text = unlock.text.text.Strikethrough();
				unlock.invokeOnUnlock.onClick.Invoke();
			}
		}
		
		public virtual void SetCosts ()
		{
			unlockCosts.Clear();
			Unlock unlock;
			for (int i = 0; i < unlocks.Length; i ++)
			{
				unlock = unlocks[i];
				unlockCosts.Add(unlock, int.Parse(unlock.text.text.Substring(unlock.text.text.IndexOf("(") + 1).Replace(" score)", "")));
			}
		}

#if UNITY_EDITOR
		[MenuItem("Unlockables/Set display texts")]
		public static void _SetDisplayTexts ()
		{
			GameManager.GetSingleton<UnlockablesManager>().SetDisplayTexts ();
		}
#endif

		public virtual void SetDisplayTexts ()
		{
			Unlock unlock;
			for (int i = 0; i < unlocks.Length; i ++)
			{
				unlock = unlocks[i];
				int indexOfCost = unlock.text.text.LastIndexOf("(") + 1;
				int indexOfCostEnd = unlock.text.text.IndexOf(" score");
				unlock.text.text = unlock.text.text.Replace(unlock.text.text.SubstringStartEnd(indexOfCost, indexOfCostEnd), "" + unlockCosts[unlock]);
			}
		}
		
		public virtual void UnlockGraphicsOptions ()
		{
			ExtraSettings.unlocked = true;
		}
		
		public virtual void UnlockSurvival ()
		{
			Survival.Unlocked = true;
		}
		
		public virtual void NextUnlock ()
		{
			if (GameManager.GetSingleton<UnlockablesManager>() != this)
			{
				GameManager.GetSingleton<UnlockablesManager>().NextUnlock ();
				return;
			}
			unlockedCount ++;
			if (unlockedCount < unlocks.Length)
			{
				unlocks[unlockedCount].unlocked = true;
				unlocks[unlockedCount].invokeOnUnlock.onClick.Invoke();
			}
		}
		
		public virtual void GetUnlocks ()
		{
			if (GameManager.GetSingleton<UnlockablesManager>() != this)
			{
				GameManager.GetSingleton<UnlockablesManager>().GetUnlocks ();
				return;
			}
			foreach (int unlockableCost in unlockCosts.Values)
			{
				if (previousScore < unlockableCost && GameManager.Score >= unlockableCost)
					NextUnlock ();
			}
		}

		[Serializable]
		public class Unlock
		{
			public int cost;
			public Text text;
			public Button invokeOnUnlock;
			public bool unlocked;
		}
	}
}