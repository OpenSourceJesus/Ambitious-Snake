using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace AmbitiousSnake
{
	public class LevelButton : _Selectable
	{
		public string[] extraScenes = new string[0];
		public Transform trs;
		public string levelName;
		[HideInInspector]
		public ColorBlock unfinishedColors;
		[HideInInspector]
		public ColorBlock completedColors;
		[HideInInspector]
		public ColorBlock acedColors;
		[HideInInspector]
		public bool completed;
		[HideInInspector]
		public bool gotParTime;
		public Button button;
		public float parTime;
		public Text parText;
		public Text nameText;
		public GameObject parIcon;
		public GameObject starIcon;
		public float doubleClickRate;
		public Level level = new Level();
		public virtual float LevelTime
		{
			get
			{
				return SaveAndLoadManager.GetValue<float>(levelName + " Time", Mathf.Infinity);
			}
			set
			{
				SaveAndLoadManager.SetValue (levelName + " Time", value);
			}
		}
		public virtual bool GotStar
		{
			get
			{
				return SaveAndLoadManager.GetValue<bool>(levelName + " Got Star", false);
			}
			set
			{
				SaveAndLoadManager.SetValue (levelName + " Got Star", value);
			}
		}
		float lastPressedTime;

		public virtual void Start ()
		{
		}

		public virtual void SetLevel ()
		{
			// if (Time.realtimeSinceStartup - lastPressedTime <= doubleClickRate)
			// 	LoadLevel ();
			lastPressedTime = Time.realtimeSinceStartup;
		}

		public virtual void LoadLevel ()
		{
			SceneManager.sceneLoaded += GameManager.Instance.FadeIn;
			GameManager.onLevelTransitionDone += GameManager.Instance.OnLevelLoaded;
		}
	}
}