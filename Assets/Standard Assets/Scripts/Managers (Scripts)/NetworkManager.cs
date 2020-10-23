using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;

namespace AmbitiousSnake
{
	[ExecuteAlways]
	public class NetworkManager : SingletonMonoBehaviour<NetworkManager>
	{
		public Text notificationText;
		public TemporaryTextObject notificationTextObject;
		public static WWWForm defaultDatabaseAccessForm;
        public string websiteUri;
		public string serverName;
		public string serverUsername;
		public string serverPassword;
		public string databaseName;
		public const string DEBUG_INDICATOR = "﬩";

		public override void Awake ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
#endif
			base.Awake ();
			defaultDatabaseAccessForm = new WWWForm();
			defaultDatabaseAccessForm.AddField("serverName", serverName);
			defaultDatabaseAccessForm.AddField("serverUsername", serverUsername);
			defaultDatabaseAccessForm.AddField("serverPassword", serverPassword);
			defaultDatabaseAccessForm.AddField("databaseName", databaseName);
		}

        public virtual void PostFormToResource (string resourceName, WWWForm form)
        {
            GameManager.GetSingleton<NetworkManager>().StartCoroutine(PostFormToResourceRoutine (resourceName, form));
        }

		public virtual IEnumerator PostFormToResourceRoutine (string resourceName, WWWForm form)
		{
			using (UnityWebRequest webRequest = UnityWebRequest.Post(websiteUri + "/" + resourceName + "?", form))
			{
				yield return webRequest.SendWebRequest();
				if (webRequest.isHttpError || webRequest.isNetworkError)
				{
					notificationText.text = webRequest.error;
					GameManager.GetSingleton<GameManager>().StartCoroutine(notificationTextObject.DisplayRoutine ());
					yield return new Exception(notificationText.text);
					yield break;
				}
				else
				{
					yield return webRequest.downloadHandler.text;
					yield break;
				}
				webRequest.Dispose();
			}
			notificationText.text = "Unknown error";
			GameManager.GetSingleton<GameManager>().StartCoroutine(notificationTextObject.DisplayRoutine ());
			yield return new Exception(notificationText.text);
		}
	}
}