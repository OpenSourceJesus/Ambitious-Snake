using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Extensions;

namespace AmbitiousSnake
{
	public class CommunityLevelButton : LevelButton
	{
		[HideInInspector]
		public string mapData;
		[HideInInspector]
		public string username;
		public override float LevelTime
		{
			get
			{
				return SaveAndLoadManager.GetValue<float>("Community " + levelName + " Time", Mathf.Infinity);
			}
			set
			{
				SaveAndLoadManager.SetValue("Community " + levelName + " Time", value);
			}
		}
		public override bool GotStar
		{
			get
			{
				return SaveAndLoadManager.GetValue<bool>("Community " + levelName + " Kept Star", false);
			}
			set
			{
				SaveAndLoadManager.SetValue("Community " + levelName + " Kept Star", value);
			}
		}
		
		public virtual void Start ()
		{
			name = levelName;
			nameText.text = levelName;
			parText.text = "Par: " + "\n" + parTime;
			completed = LevelTime < Mathf.Infinity;
			gotParTime = LevelTime <= parTime;
			if (completed)
			{
				parText.text += string.Format("\n" + "Time: {0:F1}", LevelTime);
				if (GotStar && gotParTime)
					button.colors = acedColors;
				else
					button.colors = completedColors;
			}
			starIcon.gameObject.SetActive(GotStar);
			parIcon.gameObject.SetActive(gotParTime);
			button.onClick.AddListener(delegate { SetLevel (); });
		}
		
		public virtual void SetLevel ()
		{
			GameManager.GetSingleton<CommunityLevelHub>().currentLevel = this;
			if (LevelMap.previousLevelName == levelName)
			{
				LoadLevel ();
				return;
			}
			GameManager.GetSingleton<GameManager>().SetPaused(true);
			for (int i = 0; i < PartOfLevelEditor.instances.Count; i ++)
			{
				PartOfLevelEditor part = PartOfLevelEditor.instances[i];
				DestroyImmediate (part.gameObject);
			}
			PartOfLevelEditor[] createdParts = PartOfLevelEditor.CreateObjects(mapData);
			List<Renderer> createdRenderers = new List<Renderer>();
			foreach (PartOfLevelEditor part in createdParts)
				createdRenderers.Add(part.GetComponent<Renderer>());
			GameManager.GetSingleton<LevelMap>().MakeLevelMap (createdRenderers.ToArray());
			GameManager.GetSingleton<CommunityLevelHub>().mapNameText.text = levelName;
			GameManager.GetSingleton<CommunityLevelHub>().mapUsernameText.text = username;
			GameManager.GetSingleton<CommunityLevelHub>().startButton.onClick.RemoveAllListeners();
			GameManager.GetSingleton<CommunityLevelHub>().startButton.onClick.AddListener(delegate { LoadLevel (); });
			GameManager.GetSingleton<CommunityLevelHub>().startButton.interactable = true;
		}
		
		public virtual void ReloadLevel ()
		{
			for (int i = 0; i < PartOfLevelEditor.instances.Count; i ++)
			{
				PartOfLevelEditor part = PartOfLevelEditor.instances[i];
				DestroyImmediate (part.gameObject);
			}
			PartOfLevelEditor.CreateObjects(mapData);
		}
		
		public virtual void LoadLevel ()
		{
			GameManager.GetSingleton<GameManager>().SetPaused(false);
			GameManager.GetSingleton<GameManager>().LoadLevelAdditive("Level");
			foreach (GameObject canvas in GameManager.GetSingleton<CommunityLevelHub>().canvases)
				canvas.SetActive(false);
		}
	}
}