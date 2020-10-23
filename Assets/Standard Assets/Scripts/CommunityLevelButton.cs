using System.Collections.Generic;
using UnityEngine;

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
		
		public override void Start ()
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
		
		public override void SetLevel ()
		{
			CommunityLevelHub.Instance.currentLevel = this;
			if (LevelMap.previousLevelName == levelName)
			{
				LoadLevel ();
				return;
			}
			GameManager.Instance.SetPaused(true);
			for (int i = 0; i < PartOfLevelEditor.instances.Count; i ++)
			{
				PartOfLevelEditor part = PartOfLevelEditor.instances[i];
				DestroyImmediate (part.gameObject);
			}
			PartOfLevelEditor[] createdParts = PartOfLevelEditor.CreateObjects(mapData);
			List<Renderer> createdRenderers = new List<Renderer>();
			foreach (PartOfLevelEditor part in createdParts)
				createdRenderers.Add(part.GetComponent<Renderer>());
			LevelMap.Instance.MakeLevelMap (createdRenderers.ToArray());
			CommunityLevelHub.Instance.mapNameText.text = levelName;
			CommunityLevelHub.Instance.mapUsernameText.text = username;
			CommunityLevelHub.Instance.startButton.onClick.RemoveAllListeners();
			CommunityLevelHub.Instance.startButton.onClick.AddListener(delegate { LoadLevel (); });
			CommunityLevelHub.Instance.startButton.interactable = true;
		}
		
		public virtual void ReloadLevel ()
		{
			for (int i = 0; i < PartOfLevelEditor.instances.Count; i ++)
			{
				PartOfLevelEditor part = PartOfLevelEditor.instances[i];
				DestroyImmediate (part.gameObject);
			}
			PartOfLevelEditor.CreateObjects (mapData);
		}
		
		public override void LoadLevel ()
		{
			GameManager.Instance.SetPaused (false);
			GameManager.Instance.LoadLevelAdditive ("Level");
			foreach (GameObject canvas in CommunityLevelHub.Instance.canvases)
				canvas.SetActive(false);
		}
	}
}