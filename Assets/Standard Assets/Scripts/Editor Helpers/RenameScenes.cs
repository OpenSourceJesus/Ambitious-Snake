using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AmbitiousSnake
{
	[ExecuteAlways]
	public class RenameScenes : MonoBehaviour
	{
		public bool update;
		public int addToSceneNumber;
		#if UNITY_EDITOR
		public List<SceneAsset> scenes;
		
		void Update ()
		{
			if (!update)
				return;
			update = false;
			scenes.Sort(CompareScenesByName);
			if (addToSceneNumber >= 0)
				scenes.Reverse();
			foreach (SceneAsset scene in scenes)
			{
				foreach (EditorBuildSettingsScene scene2 in EditorBuildSettings.scenes)
				{
					string scene2Name = GetSceneName(scene2.path);
					if (scene.name == scene2Name)
					{
						int levelNumber = int.Parse(scene2Name);
						string newName = "" + (levelNumber + addToSceneNumber);
						AssetDatabase.RenameAsset(scene2.path, newName);
						GameObject levelButtonGo = GameObject.Find(scene2Name);
						levelButtonGo.name = newName;
						levelButtonGo.GetComponentInChildren<Text>().text = newName;
						GameLevelButton levelButton = levelButtonGo.GetComponent<GameLevelButton>();
						levelButton.levelName = newName;
						List<string> extraScenes = new List<string>();
						foreach (EditorBuildSettingsScene scene3 in EditorBuildSettings.scenes)
						{
							string scene3Name = GetSceneName(scene3.path);
							if (scene3Name.Contains("("))
							{
								int lowerBound = int.Parse(scene3Name.Substring(scene3Name.IndexOf("(") + 1, scene3Name.IndexOf("-") - scene3Name.IndexOf("(") - 1));
								int upperBound = int.Parse(scene3Name.Substring(scene3Name.IndexOf("-") + 1, scene3Name.IndexOf(")") - scene3Name.IndexOf("-") - 1));
								if (levelNumber >= lowerBound && levelNumber <= upperBound)
									extraScenes.Add(scene3Name);
							}
						}
						levelButton.extraScenes = extraScenes.ToArray();
					}
				}
			}
		}
		
		string GetSceneName (string path)
		{
			return path.Substring(path.LastIndexOf("/") + 1).Replace(".unity", "");
		}
		
		int CompareScenesByName (SceneAsset a, SceneAsset b)
		{
			int intA = int.Parse(a.name);
			int intB = int.Parse(b.name);
			if (intA > intB)
				return 1;
			else if (intA < intB)
				return -1;
			return 0;
		}
		#endif
	}
}