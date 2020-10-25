using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using Extensions;
using Utf8Json;
using System;
using Random = UnityEngine.Random;
using System.IO;
using System.Collections;

namespace AmbitiousSnake
{
	//[ExecuteInEditMode]
	public class SaveAndLoadManager : SingletonMonoBehaviour<SaveAndLoadManager>
	{
		// [HideInInspector]
		public List<SaveAndLoadObject> saveAndLoadObjects = new List<SaveAndLoadObject>();
		public static SaveEntry[] saveEntries = new SaveEntry[0];
		// public static Dictionary<string, SaveAndLoadObject> saveAndLoadObjectTypeDict = new Dictionary<string, SaveAndLoadObject>();
		public TemporaryTextObject displayOnSave;
		public int keepPastSavesCount;
		public bool usePlayerPrefs;
		[Multiline]
		public string savedData;
		public bool overwriteSaves;
		
#if UNITY_EDITOR
		public void OnEnable ()
		{
			if (Application.isPlaying)
			{
				displayOnSave.obj.SetActive(false);
				return;
			}
			saveAndLoadObjects.Clear();
			saveAndLoadObjects.AddRange(FindObjectsOfType<SaveAndLoadObject>());
			foreach (SaveAndLoadObject saveAndLoadObject in saveAndLoadObjects)
			{
				saveAndLoadObject.saveables = saveAndLoadObject.GetComponentsInChildren<ISavableAndLoadable>();
				if (saveAndLoadObject.uniqueId == MathfExtensions.NULL_INT || saveAndLoadObject.uniqueId == 0)
					saveAndLoadObject.uniqueId = Random.Range(int.MinValue, int.MaxValue);
				foreach (ISavableAndLoadable saveableAndLoadable in saveAndLoadObject.saveables)
				{
					if (saveableAndLoadable.UniqueId == MathfExtensions.NULL_INT || saveableAndLoadable.UniqueId == 0)
						saveableAndLoadable.UniqueId = Random.Range(int.MinValue, int.MaxValue);
				}
			}
		}
#endif
		
		public void Start ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
			if (!usePlayerPrefs)
				print(Application.persistentDataPath);
#endif
			Setup ();
		}

		public void Setup ()
		{
			saveAndLoadObjects.Clear();
			saveAndLoadObjects.AddRange(FindObjectsOfType<SaveAndLoadObject>());
			// saveAndLoadObjectTypeDict.Clear();
			SaveAndLoadObject saveAndLoadObject;
			List<SaveEntry> _saveEntries = new List<SaveEntry>();
			for (int i = 0; i < saveAndLoadObjects.Count; i ++)
			{
				saveAndLoadObject = saveAndLoadObjects[i];
				saveAndLoadObject.Setup ();
				_saveEntries.AddRange(saveAndLoadObject.saveEntries);
			}
			saveEntries = _saveEntries.ToArray();
		}

		public static void SetValue (string key, object value)
		{
			PlayerPrefs.SetString(key, Serialize(value, value.GetType()));
		}

		public static T GetValue<T> (string key, T defaultValue = default(T))
		{
			return (T) Deserialize(PlayerPrefs.GetString(key, Serialize(defaultValue, typeof(T))), typeof(T));
		}
		
		public void SaveToCurrentAccount ()
		{
			if (SaveAndLoadManager.Instance != this)
			{
				SaveAndLoadManager.Instance.SaveToCurrentAccount ();
				return;
			}
			Save (ArchivesManager.currentAccountIndex);
		}
		
		public void Save (int accountIndex)
		{
			if (SaveAndLoadManager.Instance != this)
			{
				SaveAndLoadManager.Instance.Save (accountIndex);
				return;
			}
			if (accountIndex != -1)
			{
				if (overwriteSaves)
					Save (accountIndex, ArchivesManager.Accounts[ArchivesManager.currentAccountIndex].MostRecentlyUsedSaveIndex + 1);
				else
					Save (accountIndex, ArchivesManager.Accounts[ArchivesManager.currentAccountIndex].LastSaveIndex + 1);
			}
			else
				Save (-1, -1);
		}
		
		public void Save (int accountIndex, int saveIndex)
		{
			if (SaveAndLoadManager.Instance != this)
			{
				SaveAndLoadManager.Instance.Save (accountIndex, saveIndex);
				return;
			}
			OnAboutToSave ();
			// Setup ();
			savedData = "";
			if (!usePlayerPrefs)
			{
				if (!File.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + "Saved Data.txt"))
					File.CreateText(Application.persistentDataPath + Path.DirectorySeparatorChar + "Saved Data.txt");
				else
				{
					savedData = File.ReadAllText(Application.persistentDataPath + Path.DirectorySeparatorChar + "Saved Data.txt");
					string[] valueGroups = savedData.Split(new string[] { SaveEntry.VALUE_GROUP_SEPERATOR }, StringSplitOptions.None);
					for (int i = 0; i < valueGroups.Length; i += 2)
					{
						string valueGroup = valueGroups[i];
						if (valueGroup.StartsWith("" + accountIndex))
							savedData = savedData.RemoveEach(valueGroup + SaveEntry.VALUE_GROUP_SEPERATOR + valueGroups[i + 1] + SaveEntry.VALUE_GROUP_SEPERATOR);
					}
				}
			}
			if (accountIndex != -1)
			{
				Account account = ArchivesManager.Accounts[accountIndex];
				account.MostRecentlyUsedSaveIndex = saveIndex;
				if (account.MostRecentlyUsedSaveIndex > ArchivesManager.CurrentlyPlaying.LastSaveIndex)
					account.LastSaveIndex ++;
				for (int i = 0; i < saveEntries.Length; i ++)
				{
					SaveEntry saveEntry = saveEntries[i];
					if (ArchivesManager.CurrentlyPlaying.MostRecentlyUsedSaveIndex > keepPastSavesCount)
						saveEntry.Delete (accountIndex, ArchivesManager.CurrentlyPlaying.LastSaveIndex - keepPastSavesCount - 1);
					saveEntry.Save (accountIndex, saveIndex);
				}
			}
			else
			{
				for (int i = 0; i < saveEntries.Length; i ++)
					saveEntries[i].Save (-1, -1);
			}
			if (!usePlayerPrefs)
				File.WriteAllText(Application.persistentDataPath + Path.DirectorySeparatorChar + "Saved Data.txt", savedData);
			GameManager.Instance.StartCoroutine(displayOnSave.DisplayRoutine ());
		}

		void OnAboutToSave ()
		{
		}
		
		public void LoadFromCurrentAccount ()
		{
			if (SaveAndLoadManager.Instance != this)
			{
				SaveAndLoadManager.Instance.LoadFromCurrentAccount ();
				return;
			}
			Load (ArchivesManager.currentAccountIndex);
		}
		
		public void Load (int accountIndex)
		{
			if (SaveAndLoadManager.Instance != this)
			{
				SaveAndLoadManager.Instance.Load (accountIndex);
				return;
			}
			StartCoroutine(LoadRoutine (accountIndex));
		}

		IEnumerator LoadRoutine (int accountIndex)
		{
			if (accountIndex != -1)
				yield return StartCoroutine(LoadRoutine (accountIndex, ArchivesManager.CurrentlyPlaying.MostRecentlyUsedSaveIndex));
			else
				yield return StartCoroutine(LoadRoutine (-1, -1));

		}

		IEnumerator LoadRoutine (int accountIndex, int saveIndex)
		{
			if (!usePlayerPrefs)
			{
				if (File.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + "Saved Data.txt"))
					savedData = File.ReadAllText(Application.persistentDataPath + Path.DirectorySeparatorChar + "Saved Data.txt");
				else
					File.CreateText(Application.persistentDataPath + Path.DirectorySeparatorChar + "Saved Data.txt");
			}
			yield return new WaitForEndOfFrame();
			Setup ();
			for (int i = 0; i < saveEntries.Length; i ++)
				saveEntries[i].Load (accountIndex, saveIndex);
			OnLoaded ();
		}

		public void OnLoaded ()
		{
			GameManager.Instance.SetGosActive ();
		}

		public void DeleteCurrentAccount ()
		{
			if (SaveAndLoadManager.Instance != this)
			{
				SaveAndLoadManager.Instance.DeleteCurrentAccount ();
				return;
			}
			Delete (ArchivesManager.currentAccountIndex);
		}

		public void Delete (int accountIndex)
		{
			if (SaveAndLoadManager.Instance != this)
			{
				SaveAndLoadManager.Instance.Delete (accountIndex);
				return;
			}
			if (accountIndex != -1)
			{
				Account account = ArchivesManager.Accounts[accountIndex];
				for (int saveIndex = account.LastSaveIndex - keepPastSavesCount; saveIndex <= account.LastSaveIndex; saveIndex ++)
					Delete (accountIndex, saveIndex);
			}
			else
				Delete (-1, -1);
		}

		public void Delete (int accountIndex, int saveIndex)
		{
			if (SaveAndLoadManager.Instance != this)
			{
				SaveAndLoadManager.Instance.Delete (accountIndex, saveIndex);
				return;
			}
			for (int i = 0; i < saveEntries.Length; i ++)
				saveEntries[i].Delete (accountIndex, saveIndex);
			if (accountIndex != -1 && ArchivesManager.currentAccountIndex == accountIndex && saveIndex == ArchivesManager.CurrentlyPlaying.MostRecentlyUsedSaveIndex)
				ResetPersistantValues ();
			// Save (accountIndex, saveIndex);
		}

		public static void ResetPersistantValues ()
		{
			GameManager.enabledGosString = "";
			GameManager.disabledGosString = "";
			ArchivesManager.currentAccountIndex = -1;
		}

		public void DeleteAll ()
		{
			PlayerPrefs.DeleteAll();
			for (int accountIndex = -1; accountIndex < ArchivesManager.Accounts.Length; accountIndex ++)
				Delete (accountIndex);
		}

		public static string Serialize (object value, Type type)
		{
			return JsonSerializer.NonGeneric.ToJsonString(type, value);
		}

		public static object Deserialize (string serializedState, Type type)
		{
			return JsonSerializer.NonGeneric.Deserialize(type, serializedState);
		}
		
		public class SaveEntry
		{
			public ISavableAndLoadable saveableAndLoadable;
			public MemberEntry[] memberEntries = new MemberEntry[0];
			public const string VALUE_SEPERATOR = "Ⅰ";
			public const string VALUE_GROUP_SEPERATOR = "@";
			
			public SaveEntry ()
			{
			}
			
			public void Save (int accountIndex, int saveIndex)
			{
				foreach (MemberEntry memberEntry in memberEntries)
				{
					PropertyInfo property = memberEntry.member as PropertyInfo;
					string key = GetKeyForMemberEntry(accountIndex, saveIndex, memberEntry);
					if (property != null)
					{
						object value = property.GetValue(saveableAndLoadable, null);
						SetValue (key, value);
						SaveAndLoadManager.Instance.savedData += key + VALUE_GROUP_SEPERATOR + Serialize(value, property.PropertyType) + VALUE_GROUP_SEPERATOR;
					}
					else
					{
						FieldInfo field = memberEntry.member as FieldInfo;
						object value = field.GetValue(saveableAndLoadable);
						SetValue (key, value);
						SaveAndLoadManager.Instance.savedData += key + VALUE_GROUP_SEPERATOR + Serialize(value, field.FieldType) + VALUE_GROUP_SEPERATOR;
					}
				}
			}
			
			public void Load (int accountIndex, int saveIndex)
			{
				foreach (MemberEntry memberEntry in memberEntries)
				{
					string key = GetKeyForMemberEntry(accountIndex, saveIndex, memberEntry);
					if (!PlayerPrefs.HasKey(key))
						return;
					string valueString = PlayerPrefs.GetString(key);
					PropertyInfo property = memberEntry.member as PropertyInfo;
					if (property != null)
					{
						object value = Deserialize(valueString, property.PropertyType);
						property.SetValue(saveableAndLoadable, value, null);
						SaveAndLoadManager.Instance.savedData += key + VALUE_GROUP_SEPERATOR + valueString + VALUE_GROUP_SEPERATOR;
					}
					else
					{
						FieldInfo field = memberEntry.member as FieldInfo;
						object value = Deserialize(valueString, field.FieldType);
						field.SetValue(saveableAndLoadable, value);
						SaveAndLoadManager.Instance.savedData += key + VALUE_GROUP_SEPERATOR + valueString + VALUE_GROUP_SEPERATOR;
					}
				}
			}

			public void Delete (int accountIndex, int saveIndex)
			{
				foreach (MemberEntry memberEntry in memberEntries)
				{
					string[] valueGroups = SaveAndLoadManager.Instance.savedData.Split(new string[] { VALUE_GROUP_SEPERATOR }, StringSplitOptions.None);
					for (int i = 0; i < valueGroups.Length; i += 2)
					{
						string valueGroup = valueGroups[i];
						string key = GetKeyForMemberEntry(accountIndex, saveIndex, memberEntry);
						if (valueGroup == key)
						{
							SaveAndLoadManager.Instance.savedData = SaveAndLoadManager.Instance.savedData.RemoveEach(valueGroup + VALUE_GROUP_SEPERATOR + valueGroups[i + 1] + VALUE_GROUP_SEPERATOR);
							if (SaveAndLoadManager.Instance.usePlayerPrefs)
								PlayerPrefs.DeleteKey(key);
						}
					}
				}
			}

			public string GetKeyForMemberEntry (int accountIndex, int saveIndex, MemberEntry memberEntry)
			{
				// if (memberEntry.isShared)
				// 	return VALUE_SEPERATOR + saveableAndLoadable.UniqueId + VALUE_SEPERATOR + memberEntry.member.Name;
				// else
					return accountIndex + VALUE_SEPERATOR + saveIndex + VALUE_SEPERATOR + saveableAndLoadable.UniqueId + VALUE_SEPERATOR + memberEntry.member.Name;
			}

			public class MemberEntry
			{
				public MemberInfo member;
				// public bool isShared;
			}
		}
	}
}