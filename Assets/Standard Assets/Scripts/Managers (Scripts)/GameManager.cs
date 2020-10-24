using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Extensions;
using System;
using Random = UnityEngine.Random;
using UnityEngine.InputSystem;

namespace AmbitiousSnake
{
	public class GameManager : SingletonMonoBehaviour<GameManager>
	{
		public static bool paused;
		public static bool isInLevelTransition;
		public GameObject pauseButton;
		public PartOfLevelEditor[] levelEditorPrefabs;
		AsyncOperation unloadLevel;
		public Animator screenEffectAnimator;
		public List<GameObject> registeredGos = new List<GameObject>();
		static string enabledGosString = "";
		static string disabledGosString = "";
		public const string STRING_SEPERATOR = "⧫";
		public static int[] uniqueIds = new int[0];
		public static uint Score
		{
			get
			{
				return SaveAndLoadManager.GetValue<uint>("Score", 0);
			}
			set
			{
				SaveAndLoadManager.SetValue("Score", value);
			}
		}
		public delegate void OnLevelTransitionDone();
		public static event OnLevelTransitionDone onLevelTransitionDone;
		// public static Dictionary<Type, object> singletons = new Dictionary<Type, object>();
		static bool initialized;
		public static IUpdatable[] updatables = new IUpdatable[0];
		// public static SortedDictionary<int, IUpdatable> pausedUpdatablesDict = new SortedDictionary<int, IUpdatable>();
		public const string PAUSE_MENU_SCENE_NAME = "Pause Menu";
#if UNITY_EDITOR
		public bool doEditorUpdates;
#endif

		public override void Awake ()
		{
			base.Awake ();
			if (!initialized)
			{
				SceneManager.sceneLoaded += WaitForLevelTransitionEnd;
				initialized = true;
			}
			SaveAndLoadManager.Instance.Load ();
			UnlockablesManager.Instance.GetUnlocks ();
			// StartCoroutine(InitSettingsRoutine ());
		}

		public virtual void OnLevelLoaded ()
		{
			OnLevelLoaded (default(Scene), default(LoadSceneMode));
		}

		public virtual void OnLevelLoaded (Scene scene = new Scene(), LoadSceneMode loadMode = LoadSceneMode.Single)
		{
			if (LevelSelect.Instance == null)
			{
				if (Level.instance != null)
				{
					SetPaused (false);
					Level.instance.Start ();
				}
			}
			else
				updatables = updatables.Remove(Level.instance);
		}

		public virtual void Update ()
		{
			foreach (IUpdatable updatable in updatables)
				updatable.DoUpdate ();
			Physics2D.Simulate (Time.deltaTime);
			InputSystem.Update ();
		}

		public virtual IEnumerator InitSettingsRoutine ()
		{
			LoadLevelAdditive ("Settings");
			yield return new WaitForEndOfFrame();
			Settings.Instance.Init ();
			UnloadLevelAsync ("Settings");
			LoadLevelAdditive ("Extra Settings");
			yield return new WaitForEndOfFrame();
			ExtraSettings.Instance.Init ();
			UnloadLevelAsync ("Extra Settings");
		}

		public virtual void FadeIn (Scene scene = new Scene(), LoadSceneMode loadMode = LoadSceneMode.Single)
		{
			if (Instance != this)
			{
				Instance.FadeIn (scene, loadMode);
				return;
			}
			WaitForLevelTransitionEnd (scene, loadMode);
			screenEffectAnimator.Play("Fade In");
			SceneManager.sceneLoaded -= FadeIn;
		}

		public virtual void WaitForLevelTransitionEnd (Scene scene = new Scene(), LoadSceneMode loadMode = LoadSceneMode.Single)
		{
			if (Instance != this)
			{
				Instance.WaitForLevelTransitionEnd (scene, loadMode);
				return;
			}
			StartCoroutine(WaitForLevelTransitionEndRoutine ());
			// SceneManager.sceneLoaded -= WaitForLevelTransitionEnd;
		}

		public virtual IEnumerator WaitForLevelTransitionEndRoutine ()
		{
			yield return new WaitUntil(() => (!screenEffectAnimator.GetCurrentAnimatorStateInfo(0).IsName("Invisible Screen")));
			while (true)
			{
				if (screenEffectAnimator.GetCurrentAnimatorStateInfo(0).IsName("Invisible Screen"))
				{
					if (onLevelTransitionDone != null)
					{
						onLevelTransitionDone ();
						onLevelTransitionDone = null;
					}
					isInLevelTransition = false;
					yield break;
				}
				yield return new WaitForEndOfFrame();
			}
		}

		public static void SetUniqueId (IIdentifiable identifiable)
		{
			while (identifiable.UniqueId == 0 || uniqueIds.Contains(identifiable.UniqueId))
				identifiable.UniqueId = Random.Range(int.MinValue, int.MaxValue);
			uniqueIds = uniqueIds.Add(identifiable.UniqueId);
		}
			
		public virtual void SetGosActive ()
		{
			if (Instance != this)
			{
				Instance.SetGosActive ();
				return;
			}
			string[] stringSeperators = { STRING_SEPERATOR };
			string[] enabledGos = enabledGosString.Split(stringSeperators, StringSplitOptions.None);
			foreach (string goName in enabledGos)
			{
				for (int i = 0; i < registeredGos.Count; i ++)
				{
					if (goName == registeredGos[i].name)
					{
						registeredGos[i].SetActive(true);
						break;
					}
				}
			}
			string[] disabledGos = disabledGosString.Split(stringSeperators, StringSplitOptions.None);
			foreach (string goName in disabledGos)
			{
				GameObject go = GameObject.Find(goName);
				if (go != null)
					go.SetActive(false);
			}
		}
		
		public virtual void ActivateGoForever (GameObject go)
		{
			if (Instance != this)
			{
				Instance.ActivateGoForever (go);
				return;
			}
			go.SetActive(true);
			disabledGosString = disabledGosString.Replace(go.name + STRING_SEPERATOR, "");
			if (!enabledGosString.Contains(go.name))
				enabledGosString += go.name + STRING_SEPERATOR;
		}
		
		public virtual void DeactivateGoForever (GameObject go)
		{
			if (Instance != this)
			{
				Instance.DeactivateGoForever (go);
				return;
			}
			go.SetActive(false);
			enabledGosString = enabledGosString.Replace(go.name + STRING_SEPERATOR, "");
			if (!disabledGosString.Contains(go.name))
				disabledGosString += go.name + STRING_SEPERATOR;
		}
		
		public virtual void ActivateGoForever (string goName)
		{
			if (Instance != this)
			{
				Instance.ActivateGoForever (goName);
				return;
			}
			for (int i = 0; i < registeredGos.Count; i ++)
			{
				if (goName == registeredGos[i].name)
				{
					ActivateGoForever (registeredGos[i]);
					return;
				}
			}
		}
		
		public virtual void DeactivateGoForever (string goName)
		{
			if (Instance != this)
			{
				Instance.DeactivateGoForever (goName);
				return;
			}
			GameObject go = GameObject.Find(goName);
			if (go != null)
				DeactivateGoForever (go);
		}

		public virtual void OnApplicationFocus (bool isFocused)
		{
			if (!isFocused && SceneManager.GetSceneByName("Level").isLoaded && LevelSelect.Instance == null && !SceneManager.GetSceneByName(PAUSE_MENU_SCENE_NAME).isLoaded)
			{
				LoadLevelAdditive (PAUSE_MENU_SCENE_NAME);
				return;
			}
		}
		
		public virtual void DestroyObject (GameObject go)
		{
			if (go != null)
				Destroy(go);
		}
		
		public virtual void DestroyObject (string goName)
		{
			DestroyObject (GameObject.Find(goName));
		}
		
		public virtual void DeactivateObject (string goName)
		{
			GameObject.Find(goName).SetActive(false);
		}
		
		public virtual void TriggerButton (Button button)
		{
			button.onClick.Invoke();
		}
		
		public virtual void TriggerButton (string goName)
		{
			GameObject.Find(goName).GetComponent<Button>().onClick.Invoke();
		}
		
		public virtual void SetPaused (bool pause)
		{
			if (Instance != this)
			{
				Instance.SetPaused (pause);
				return;
			}
			paused = pause;
			if (pauseButton != null)
			{
				pauseButton.SetActive(!pause);
				if (paused)
					LevelTimer.Instance.timer.Stop ();
				else
					LevelTimer.Instance.timer.Start ();
			}
			Time.timeScale = 1 - paused.GetHashCode();
			foreach (Rigidbody2D rigid in _Rigidbody2D.allInstances)
				rigid.simulated = !pause;
		}

		public virtual void SetTimeScale (float timeScale)
		{
			Time.timeScale = timeScale;
		}
		
		public virtual void LoadLevel (string levelName, LoadSceneMode loadMode)
		{
			SceneManager.LoadScene(levelName, loadMode);
		}
		
		public virtual void LoadLevel (string levelName)
		{
			LoadLevel (levelName, LoadSceneMode.Single);
		}
		
		public virtual void LoadLevel (int levelIndex)
		{
			LoadLevel (SceneManager.GetSceneAt(levelIndex).name);
		}
		
		public virtual void LoadLevelAdditive (string levelName)
		{
			LoadLevel (levelName, LoadSceneMode.Additive);
		}
		
		public virtual void LoadLevelAdditive (int levelIndex)
		{
			LoadLevel (SceneManager.GetSceneAt(levelIndex).name, LoadSceneMode.Additive);
		}
		
		public virtual void UnloadLevelAsync (string levelName)
		{
			if (Instance != this)
			{
				Instance.UnloadLevelAsync (levelName);
				return;
			}
			if (SceneManager.GetSceneByName(levelName).isLoaded)
				unloadLevel = SceneManager.UnloadSceneAsync(levelName);
		}
		
		public virtual void UnloadLevelAsync (int levelIndex)
		{
			UnloadLevelAsync (SceneManager.GetSceneAt(levelIndex).name);
		}
		
		public virtual void Quit ()
		{
			Application.Quit ();
		}

		public virtual void OnApplicationQuit ()
		{
			SceneManager.sceneLoaded -= WaitForLevelTransitionEnd;
			onLevelTransitionDone -= OnLevelLoaded;
		}
		
		public static List<Transform> GetAllChildren (Transform root)
		{
			List<Transform> output = new List<Transform>();
			List<Transform> transformsToSearch = new List<Transform>();
			output.Add(root);
			transformsToSearch.Add(root);
			while (transformsToSearch.Count > 0)
			{
				for (int i = 0; i < transformsToSearch.Count; i ++)
				{
					foreach (Transform child in transformsToSearch[i].GetComponentsInChildren<Transform>())
					{
						if (!output.Contains(child))
						{
							output.Add(child);
							transformsToSearch.Add(child);
						}
					}
					transformsToSearch.RemoveAt(0);
				}
			}
			return output;
		}
		
		public virtual void ToggleActive (GameObject go)
		{
			go.SetActive(!go.activeSelf);
		}
		
		public virtual void ToggleUIToggle (UIToggle uiToggle)
		{
			uiToggle.toggle.isOn = !uiToggle.toggle.isOn;
		}
		
		public virtual void ActivateUIToggle (UIToggle uiToggle)
		{
			uiToggle.toggle.isOn = true;
		}
		
		public virtual void DeactivateUIToggle (UIToggle uiToggle)
		{
			uiToggle.toggle.isOn = false;
		}
	}
}