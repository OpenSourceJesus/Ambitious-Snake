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
		public static Dictionary<string, string> data = new Dictionary<string, string>();
		
#if UNITY_EDITOR
		public virtual void OnEnable ()
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
		
		public virtual void Start ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
			if (!usePlayerPrefs)
				print(Application.persistentDataPath);
#endif
			Setup ();
		}

		public virtual void Setup ()
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
			if (data.ContainsKey(key))
				data[key] = Serialize(value, value.GetType());
			else
				data.Add(key, Serialize(value, value.GetType()));
		}

		public static T GetValue<T> (string key, T defaultValue = default(T))
		{
			string dataString;
			if (data.TryGetValue(key, out dataString))
				return (T) Deserialize(dataString, typeof(T));
			else
				return defaultValue;
		}
		
		public virtual void SaveToCurrentAccount ()
		{
			if (SaveAndLoadManager.Instance != this)
			{
				SaveAndLoadManager.Instance.SaveToCurrentAccount ();
				return;
			}
			Save (ArchivesManager.currentAccountIndex);
		}
		
		public virtual void Save (int accountIndex)
		{
			if (SaveAndLoadManager.Instance != this)
			{
				SaveAndLoadManager.Instance.SaveToCurrentAccount ();
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
		
		public virtual void Save (int accountIndex, int saveIndex)
		{
			if (SaveAndLoadManager.Instance != this)
			{
				SaveAndLoadManager.Instance.SaveToCurrentAccount ();
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
		
		public virtual void LoadFromCurrentAccount ()
		{
			if (SaveAndLoadManager.Instance != this)
			{
				SaveAndLoadManager.Instance.LoadFromCurrentAccount ();
				return;
			}
			Load (ArchivesManager.currentAccountIndex);
		}
		
		public virtual void Load (int accountIndex)
		{
			if (SaveAndLoadManager.Instance != this)
			{
				SaveAndLoadManager.Instance.Load (accountIndex);
				return;
			}
			if (!usePlayerPrefs)
			{
				if (File.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + "Saved Data.txt"))
					savedData = File.ReadAllText(Application.persistentDataPath + Path.DirectorySeparatorChar + "Saved Data.txt");
				else
					File.CreateText(Application.persistentDataPath + Path.DirectorySeparatorChar + "Saved Data.txt");
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
			yield return new WaitForEndOfFrame();
			Setup ();
			for (int i = 0; i < saveEntries.Length; i ++)
				saveEntries[i].Load (accountIndex, saveIndex);
			OnLoaded ();
		}

		public virtual void OnLoaded ()
		{
			GameManager.Instance.SetGosActive ();
		}

		public virtual void DeleteCurrentAccount ()
		{
			if (SaveAndLoadManager.Instance != this)
			{
				SaveAndLoadManager.Instance.DeleteCurrentAccount ();
				return;
			}
			Delete (ArchivesManager.currentAccountIndex);
		}

		public virtual void Delete (int accountIndex)
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

		public virtual void Delete (int accountIndex, int saveIndex)
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

		public virtual void DeleteAll ()
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
			
			public virtual void Save (int accountIndex, int saveIndex)
			{
				foreach (MemberEntry memberEntry in memberEntries)
				{
					if (!memberEntry.isField)
					{
						PropertyInfo property = memberEntry.member as PropertyInfo;
						if (SaveAndLoadManager.Instance.usePlayerPrefs)
						{
							string data = Serialize(property.GetValue(saveableAndLoadable), property.PropertyType);
							PlayerPrefs.SetString(GetKeyNameForMemberEntry(accountIndex, saveIndex, memberEntry), data);
							SaveAndLoadManager.Instance.savedData += data;
						}
						else
							SaveAndLoadManager.Instance.savedData += GetKeyNameForMemberEntry(accountIndex, saveIndex, memberEntry) + VALUE_GROUP_SEPERATOR + Serialize(property.GetValue(saveableAndLoadable), property.PropertyType) + VALUE_GROUP_SEPERATOR;
					}
					else
					{
						FieldInfo field = memberEntry.member as FieldInfo;
						if (SaveAndLoadManager.Instance.usePlayerPrefs)
						{
							string data = Serialize(field.GetValue(saveableAndLoadable), field.FieldType);
							PlayerPrefs.SetString(GetKeyNameForMemberEntry(accountIndex, saveIndex, memberEntry), data);
							SaveAndLoadManager.Instance.savedData += data;
						}
						else
							SaveAndLoadManager.Instance.savedData += GetKeyNameForMemberEntry(accountIndex, saveIndex, memberEntry) + VALUE_GROUP_SEPERATOR + Serialize(field.GetValue(saveableAndLoadable), field.FieldType) + VALUE_GROUP_SEPERATOR;
					}
				}
			}
			
			public virtual void Load (int accountIndex, int saveIndex)
			{
				object value;
				foreach (MemberEntry memberEntry in memberEntries)
				{
					if (!memberEntry.isField)
					{
						PropertyInfo property = memberEntry.member as PropertyInfo;
						if (SaveAndLoadManager.Instance.usePlayerPrefs)
						{
							value = Deserialize(PlayerPrefs.GetString(GetKeyNameForMemberEntry(accountIndex, saveIndex, memberEntry), Serialize(property.GetValue(saveableAndLoadable), property.PropertyType)), property.PropertyType);
							property.SetValue(saveableAndLoadable, value);
						}
						else
						{
							string[] valueGroups = SaveAndLoadManager.Instance.savedData.Split(new string[] { VALUE_GROUP_SEPERATOR }, StringSplitOptions.None);
							for (int i = 0; i < valueGroups.Length; i += 2)
							{
								string valueGroup = valueGroups[i];
								if (valueGroup == GetKeyNameForMemberEntry(accountIndex, saveIndex, memberEntry))
								{
									valueGroup = valueGroups[i + 1];
									value = Deserialize(valueGroup, property.PropertyType);
									property.SetValue(saveableAndLoadable, value);
								}
							}
						}
					}
					else
					{
						FieldInfo field = memberEntry.member as FieldInfo;
						if (SaveAndLoadManager.Instance.usePlayerPrefs)
						{
							value = Deserialize(PlayerPrefs.GetString(GetKeyNameForMemberEntry(accountIndex, saveIndex, memberEntry), Serialize(field.GetValue(saveableAndLoadable), field.FieldType)), field.FieldType);
							field.SetValue(saveableAndLoadable, value);
						}
						else
						{
							string[] valueGroups = SaveAndLoadManager.Instance.savedData.Split(new string[] { VALUE_GROUP_SEPERATOR }, StringSplitOptions.None);
							for (int i = 0; i < valueGroups.Length; i += 2)
							{
								string valueGroup = valueGroups[i];
								if (valueGroup == GetKeyNameForMemberEntry(accountIndex, saveIndex, memberEntry))
								{
									valueGroup = valueGroups[i + 1];
									value = Deserialize(valueGroup, field.FieldType);
									field.SetValue(saveableAndLoadable, value);
								}
							}
						}
					}
				}
			}

			public virtual void Delete (int accountIndex, int saveIndex)
			{
				foreach (MemberEntry memberEntry in memberEntries)
				{
					if (SaveAndLoadManager.Instance.usePlayerPrefs)
						PlayerPrefs.DeleteKey(GetKeyNameForMemberEntry(accountIndex, saveIndex, memberEntry));
					else
					{
						string[] valueGroups = SaveAndLoadManager.Instance.savedData.Split(new string[] { VALUE_GROUP_SEPERATOR }, StringSplitOptions.None);
						for (int i = 0; i < valueGroups.Length; i += 2)
						{
							string valueGroup = valueGroups[i];
							if (valueGroup == GetKeyNameForMemberEntry(accountIndex, saveIndex, memberEntry))
								SaveAndLoadManager.Instance.savedData = SaveAndLoadManager.Instance.savedData.RemoveEach(valueGroup + VALUE_GROUP_SEPERATOR + valueGroups[i + 1] + VALUE_GROUP_SEPERATOR);
						}
					}
				}
			}

			public virtual string GetKeyNameForMemberEntry (int accountIndex, int saveIndex, MemberEntry memberEntry)
			{
				// if (memberEntry.isShared)
				// 	return VALUE_SEPERATOR + saveableAndLoadable.UniqueId + VALUE_SEPERATOR + memberEntry.member.Name;
				// else
					return accountIndex + VALUE_SEPERATOR + saveIndex + VALUE_SEPERATOR + saveableAndLoadable.UniqueId + VALUE_SEPERATOR + memberEntry.member.Name;
			}

			public class MemberEntry
			{
				public MemberInfo member;
				public bool isField;
				// public bool isShared;
			}
		}
	}
}