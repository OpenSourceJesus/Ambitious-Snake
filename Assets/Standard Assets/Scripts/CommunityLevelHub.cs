using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CoroutineWithData = ThreadingUtilities.CoroutineWithData;
using System;

namespace AmbitiousSnake
{
	public class CommunityLevelHub : SingletonMonoBehaviour<CommunityLevelHub>
	{
		[HideInInspector]
		public CommunityLevelButton currentLevel;
		public Text mapNameText;
		public Text mapUsernameText;
		public CommunityLevelButton levelButtonPrefab;
		public Transform levelButtonsParent;
		public GameObject[] canvases;
		public Button startButton;
		
		public override void Awake ()
		{
			base.Awake ();
			Refresh ();
		}
		
		public virtual void Refresh ()
		{
			StartCoroutine (GetMapListRoutine ());
		}
		
		public virtual IEnumerator GetMapListRoutine ()
		{
			CoroutineWithData cd = new CoroutineWithData(this, GameManager.GetSingleton<NetworkManager>().PostFormToResourceRoutine("GetMapList", NetworkManager.defaultDatabaseAccessForm));
			string result = "";
			Exception exception;
			while (string.IsNullOrEmpty(result))
			{
				yield return cd.coroutine;
				exception = cd.result as Exception;
				if (exception != null)
				{
					GameManager.GetSingleton<NetworkManager>().notificationText.text = exception.Message;
					StartCoroutine(GameManager.GetSingleton<NetworkManager>().notificationTextObject.DisplayRoutine ());
					yield break;
				}
				else
					result = cd.result as string;
			}
			string mapInfo = result;
			mapInfo = mapInfo.Substring(0, mapInfo.LastIndexOf(LevelEditor.endOfInfoIndicator));
			string textBeforeName = "Map name: ";
			string textBeforeUsername = "Map username: ";
			string textBeforePar = "Map par: ";
			string textBeforeData = "Map data: ";
			string mapName;
			string username;
			string mapPar;
			string mapData;
			int indexOfTextBeforeName;
			while (!string.IsNullOrEmpty(mapInfo))
			{
				mapInfo = mapInfo.Remove(0, mapInfo.IndexOf(textBeforeName) + textBeforeName.Length);
				indexOfTextBeforeName = mapInfo.IndexOf(textBeforeName);
				if (indexOfTextBeforeName == -1)
					yield break;
				CommunityLevelButton levelButton = Instantiate(levelButtonPrefab);
				levelButton.transform.SetParent(levelButtonsParent);
				levelButton.transform.localScale = Vector3.one;
				mapName = mapInfo.Substring(0, indexOfTextBeforeName);
				levelButton.levelName = mapName;
				mapInfo = mapInfo.Remove(0, mapName.Length + textBeforeUsername.Length);
				username = mapInfo.Substring(0, mapInfo.IndexOf(textBeforeData));
				levelButton.username = username;
				mapPar = mapInfo.Substring(0, mapInfo.IndexOf(textBeforePar));
				if (string.IsNullOrEmpty(mapPar))
					levelButton.parTime = 0;
				else
					levelButton.parTime = float.Parse(mapPar);
				mapInfo = mapInfo.Remove(0, mapPar.Length + textBeforePar.Length);
				indexOfTextBeforeName = mapInfo.IndexOf(textBeforeName);
				if (indexOfTextBeforeName == -1)
					indexOfTextBeforeName = mapInfo.Length;
				mapData = mapInfo.Substring(0, indexOfTextBeforeName);
				levelButton.mapData = mapData;
				mapInfo = mapInfo.Remove(0, mapData.Length + textBeforeName.Length);
			}
		}
	}
}