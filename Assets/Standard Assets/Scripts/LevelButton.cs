using UnityEngine;
using UnityEngine.UI;

namespace AmbitiousSnake
{
	public class LevelButton : _Selectable
	{
		public string[] extraScenes = new string[0];
		public Transform trs;
		public string levelName;
		public ColorBlock unfinishedColors;
		public ColorBlock completedColors;
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
		public virtual float LevelTime
		{
			get
			{
				return SaveAndLoadManager.GetValue<float>(levelName + " Time", Mathf.Infinity);
			}
			set
			{
				SaveAndLoadManager.SetValue(levelName + " Time", value);
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
				SaveAndLoadManager.SetValue(levelName + " Got Star", value);
			}
		}
		public Level level = new Level();

		public virtual void Start ()
		{
		}

		public virtual void SetLevel ()
		{
		}

		public virtual void LoadLevel ()
		{
			GameManager.onLevelTransitionDone += GameManager.Instance.OnLevelLoaded;
		}
	}
}