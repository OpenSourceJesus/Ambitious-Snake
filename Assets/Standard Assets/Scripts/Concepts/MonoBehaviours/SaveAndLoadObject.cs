using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
using SaveEntry = AmbitiousSnake.SaveAndLoadManager.SaveEntry;
using Extensions;

public class SaveAndLoadObject : MonoBehaviour
{
	public string Name
	{
		get
		{
			return name;
		}
		set
		{
			name = value;
		}
	}
	public int UniqueId
	{
		get
		{
			return uniqueId;
		}
		set
		{
			uniqueId = value;
		}
	}
	public int uniqueId = MathfExtensions.NULL_INT;
	public ISavableAndLoadable[] saveables = new ISavableAndLoadable[0];
	// public string typeId;
	public SaveEntry[] saveEntries = new SaveEntry[0];
	
	public virtual void Setup ()
	{
		saveables = GetComponentsInChildren<ISavableAndLoadable>();
		// SaveAndLoadObject sameTypeObj;
		// if (!SaveAndLoadManager.saveAndLoadObjectTypeDict.TryGetValue(typeId, out sameTypeObj))
		// {
			saveEntries = new SaveEntry[saveables.Length];
			for (int i = 0; i < saveables.Length; i ++)
			{
				SaveEntry saveEntry = new SaveEntry();
				saveEntry.saveableAndLoadable = saveables[i];
				List<SaveEntry.MemberEntry> memberEntries = new List<SaveEntry.MemberEntry>();
				MemberInfo[] members = saveEntry.saveableAndLoadable.GetType().GetMembers();
				for (int i2 = 0; i2 < members.Length; i2 ++)
				{
					MemberInfo member = members[i2];
					SaveAndLoadValue saveAndLoadValue = Attribute.GetCustomAttribute(member, typeof(SaveAndLoadValue)) as SaveAndLoadValue;
					if (saveAndLoadValue != null)
					{
						SaveEntry.MemberEntry memberEntry = new SaveEntry.MemberEntry();
						// memberEntry.isShared = saveAndLoadValue.isShared;
						memberEntry.member = member;
						memberEntries.Add(memberEntry);
					}
				}
				saveEntry.memberEntries = memberEntries.ToArray();
				saveEntries[i] = saveEntry;
			}
		// 	SaveAndLoadManager.saveAndLoadObjectTypeDict.Add(typeId, this);
		// }
		// else
		// {
		// 	saveEntries = sameTypeObj.saveEntries;
		// 	SaveEntry saveEntry;
		// 	for (int i = 0; i < saveEntries.Length; i ++)
		// 	{
		// 		saveEntry = saveEntries[i];
		// 		saveEntry.saveableAndLoadable = saveables[i];
		// 		saveEntry.saveableAndLoadObject = this;
		// 	}
		// }
	}
}
