using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Reflection;
using UnityEngine.UI;
using Extensions;
using System.IO;
using System;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;

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
		public static Dictionary<Type, object> singletons = new Dictionary<Type, object>();
		static bool initialized;
		public static IUpdatable[] updatables = new IUpdatable[0];
		public static SortedDictionary<int, IUpdatable> pausedUpdatablesDict = new SortedDictionary<int, IUpdatable>();
		public static IFixedUpdatable[] fixedUpdatables = new IFixedUpdatable[0];
		public static SortedDictionary<int, IFixedUpdatable> pausedFixedUpdatablesDict = new SortedDictionary<int, IFixedUpdatable>();
		public static string pauseMenuLevelName = "Pause Menu";

		public override void Awake ()
		{
			base.Awake ();
			if (!initialized)
			{
				SceneManager.sceneLoaded += WaitForLevelTransitionEnd;
				initialized = true;
			}
			GetSingleton<SaveAndLoadManager>().Load ();
			GetSingleton<UnlockablesManager>().GetUnlocks ();
			// StartCoroutine(InitSettingsRoutine ());
		}

		public virtual void OnLevelLoaded ()
		{
			OnLevelLoaded (default(Scene), default(LoadSceneMode));
		}

		public virtual void OnLevelLoaded (Scene scene = new Scene(), LoadSceneMode loadMode = LoadSceneMode.Single)
		{
			if (GetSingleton<LevelSelect>() == null)
			{
				if (GetSingleton<Level>() != null)
				{
					SetPaused (false);
					GetSingleton<Level>().Start ();
				}
			}
			else
				updatables = updatables.Remove(GetSingleton<Level>());
		}

		public static T GetSingleton<T> ()
		{
			if (!singletons.ContainsKey(typeof(T)))
				return GetSingleton<T>(FindObjectsOfType<Object>());
			else
			{
				if (singletons[typeof(T)] == null || singletons[typeof(T)].Equals(default(T)))
				{
					T singleton = GetSingleton<T>(FindObjectsOfType<Object>());
					singletons[typeof(T)] = singleton;
					return singleton;
				}
				else
					return (T) singletons[typeof(T)];
			}
		}

		public static T GetSingleton<T> (Object[] objects)
		{
			if (typeof(T).IsSubclassOf(typeof(Object)))
			{
				foreach (Object obj in objects)
				{
					if (obj is T)
					{
						singletons.Remove(typeof(T));
						singletons.Add(typeof(T), obj);
						break;
					}
				}
			}
			if (singletons.ContainsKey(typeof(T)))
				return (T) singletons[typeof(T)];
			else
				return default(T);
		}

		public virtual void Update ()
		{
			foreach (IUpdatable updatable in updatables)
				updatable.DoUpdate ();
		}

		public virtual void FixedUpdate ()
		{
			foreach (IFixedUpdatable fixedUpdatable in fixedUpdatables)
				fixedUpdatable.DoFixedUpdate ();
		}

		public virtual IEnumerator InitSettingsRoutine ()
		{
			LoadLevelAdditive ("Settings");
			yield return new WaitForEndOfFrame();
			GetSingleton<Settings>().Init ();
			UnloadLevel ("Settings");
			LoadLevelAdditive ("Extra Settings");
			yield return new WaitForEndOfFrame();
			GetSingleton<ExtraSettings>().Init ();
			UnloadLevel ("Extra Settings");
		}

		public virtual void FadeIn (Scene scene = new Scene(), LoadSceneMode loadMode = LoadSceneMode.Single)
		{
			if (GetSingleton<GameManager>() != this)
			{
				GetSingleton<GameManager>().FadeIn (scene, loadMode);
				return;
			}
			WaitForLevelTransitionEnd (scene, loadMode);
			screenEffectAnimator.Play("Fade In");
			SceneManager.sceneLoaded -= FadeIn;
		}

		public virtual void WaitForLevelTransitionEnd (Scene scene = new Scene(), LoadSceneMode loadMode = LoadSceneMode.Single)
		{
			if (GetSingleton<GameManager>() != this)
			{
				GetSingleton<GameManager>().WaitForLevelTransitionEnd (scene, loadMode);
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
			if (GetSingleton<GameManager>() != this)
			{
				GetSingleton<GameManager>().SetGosActive ();
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
			if (GetSingleton<GameManager>() != this)
			{
				GetSingleton<GameManager>().ActivateGoForever (go);
				return;
			}
			go.SetActive(true);
			disabledGosString = disabledGosString.Replace(go.name + STRING_SEPERATOR, "");
			if (!enabledGosString.Contains(go.name))
				enabledGosString += go.name + STRING_SEPERATOR;
		}
		
		public virtual void DeactivateGoForever (GameObject go)
		{
			if (GetSingleton<GameManager>() != this)
			{
				GetSingleton<GameManager>().DeactivateGoForever (go);
				return;
			}
			go.SetActive(false);
			enabledGosString = enabledGosString.Replace(go.name + STRING_SEPERATOR, "");
			if (!disabledGosString.Contains(go.name))
				disabledGosString += go.name + STRING_SEPERATOR;
		}
		
		public virtual void ActivateGoForever (string goName)
		{
			if (GetSingleton<GameManager>() != this)
			{
				GetSingleton<GameManager>().ActivateGoForever (goName);
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
			if (GetSingleton<GameManager>() != this)
			{
				GetSingleton<GameManager>().DeactivateGoForever (goName);
				return;
			}
			GameObject go = GameObject.Find(goName);
			if (go != null)
				DeactivateGoForever (go);
		}

		public virtual void OnApplicationFocus (bool isFocused)
		{
			if (!isFocused && SceneManager.GetSceneByName("Level").isLoaded && GetSingleton<LevelSelect>() == null && !SceneManager.GetSceneByName(pauseMenuLevelName).isLoaded)
			{
				LoadLevelAdditive (pauseMenuLevelName);
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
			if (GetSingleton<GameManager>() != this)
			{
				GetSingleton<GameManager>().SetPaused (pause);
				return;
			}
			paused = pause;
			if (pauseButton != null)
			{
				pauseButton.SetActive(!pause);
				if (paused)
					GameManager.GetSingleton<LevelTimer>().timer.Stop ();
				else
					GameManager.GetSingleton<LevelTimer>().timer.Start ();
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
			if (GetSingleton<GameManager>() != this)
			{
				GetSingleton<GameManager>().UnloadLevelAsync (levelName);
				return;
			}
			if (SceneManager.GetSceneByName(levelName).isLoaded)
				unloadLevel = SceneManager.UnloadSceneAsync(levelName);
		}
		
		public virtual void UnloadLevel (string levelName)
		{
			if (SceneManager.GetSceneByName(levelName).isLoaded)
				SceneManager.UnloadScene(levelName);
		}
		
		public virtual void UnloadLevel (int levelIndex)
		{
			UnloadLevel (SceneManager.GetSceneAt(levelIndex).name);
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