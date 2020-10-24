using System.Collections.Generic;
using UnityEngine;

namespace AmbitiousSnake
{
	public class PartOfLevelEditor : MonoBehaviour
	{
		[HideInInspector]
		public Transform trs;
		public static List<PartOfLevelEditor> instances = new List<PartOfLevelEditor>();
		public static char statementSeperater = ';';
		public static char paramSeperator = ',';
		[HideInInspector]
		public long uniqueId;
		static long lastUniqueId = 0;
		[HideInInspector]
		public long parent;
		public int[] allowedRotations;
		public bool resizable;
		public bool onlyOneExists;
		public bool required;
		static Dictionary<long, PartOfLevelEditor> orphansInNeed = new Dictionary<long, PartOfLevelEditor>();
		
		void Awake ()
		{
			instances.Add(this);
		}
		
		void Start ()
		{
			if (LevelEditor.Instance != null)
			{
				uniqueId = lastUniqueId;
				lastUniqueId ++;
			}
		}
		
		void OnDestroy ()
		{
			instances.Remove(this);
		}
		
		public static PartOfLevelEditor[] CreateObjects (string data)
		{
			orphansInNeed.Clear();
			List<PartOfLevelEditor> output = new List<PartOfLevelEditor>();
			if (string.IsNullOrEmpty(data))
				return output.ToArray();
			string _uniqueId;
			string _typeIndex;
			string _parent;
			string _pos;
			string _rot;
			string _size;
			PartOfLevelEditor type;
			PartOfLevelEditor part;
			while (data.Length > 0)
			{
				_uniqueId = ExtractNextStatement(ref data);
				_typeIndex = ExtractNextStatement(ref data);
				_parent = ExtractNextStatement(ref data);
				_pos = ExtractNextStatement(ref data);
				_rot = ExtractNextStatement(ref data);
				_size = ExtractNextStatement(ref data);
				type = GameManager.Instance.levelEditorPrefabs[int.Parse(ExtractNextParam(ref _typeIndex))];
				part = Instantiate(type);
				part.name = part.name.Replace("(Clone)", "");
				if (LevelEditor.Instance != null)
					LevelEditor.Instance.ForceDeselectObject (part);
				part.parent = long.Parse(_parent);
				if (part.parent > -1)
					orphansInNeed.Add(part.parent, part);
				part.uniqueId = int.Parse(ExtractNextParam(ref _uniqueId));
				if (orphansInNeed.ContainsKey(part.uniqueId))
					orphansInNeed[part.uniqueId].trs.SetParent(part.trs);
				if (string.IsNullOrEmpty(_pos))
					part.trs.position = type.trs.position;
				else
					part.trs.position = new Vector3(int.Parse(ExtractNextParam(ref _pos)), int.Parse(ExtractNextParam(ref _pos)));
				if (string.IsNullOrEmpty(_rot))
					part.trs.eulerAngles = type.trs.eulerAngles;
				else
					part.trs.eulerAngles = Vector3.forward * int.Parse(ExtractNextParam(ref _rot));
				if (string.IsNullOrEmpty(_size))
					part.trs.localScale = type.trs.localScale;
				else
					part.trs.localScale = new Vector3(int.Parse(ExtractNextParam(ref _size)), int.Parse(ExtractNextParam(ref _size)));
				output.Add(part);
			}
			return output.ToArray();
		}
		
		public static string ExtractNextStatement (ref string statements)
		{
			string nextStatement = statements.Substring(0, statements.IndexOf(statementSeperater) + 1);
			statements = statements.Remove(0, nextStatement.Length);
			nextStatement = nextStatement.Replace(statementSeperater.ToString(), "");
			return nextStatement;
		}
		
		public static string ExtractNextParam (ref string parameters)
		{
			if (string.IsNullOrEmpty(parameters))
				return parameters;
			parameters = parameters.Remove(0, 1);
			int indexOfFirstParam = parameters.IndexOf("=");
			if (indexOfFirstParam != -1)
				parameters = parameters.Remove(0, indexOfFirstParam + 1);
			int indexOfNextParamSeperator = parameters.IndexOf(paramSeperator);
			string nextParam = "";
			if (indexOfNextParamSeperator != -1)
				nextParam = parameters.Substring(0, indexOfNextParamSeperator);
			else
				nextParam = parameters;
			parameters = parameters.Remove(0, nextParam.Length);
			return nextParam;
		}
		
		public override string ToString ()
		{
			string output = "";
			int typeIndex = 0;
			PartOfLevelEditor type = GameManager.Instance.levelEditorPrefabs[typeIndex];
			for (int i = 1; i < GameManager.Instance.levelEditorPrefabs.Length; i ++)
			{
				type = GameManager.Instance.levelEditorPrefabs[i];
				if (name == type.name)
				{
					typeIndex = i;
					break;
				}
			}
			output += "id=" + uniqueId + statementSeperater;
			output += "type=" + typeIndex + statementSeperater;
			long parentId = -1;
			if (trs.parent != null)
				parentId = GetComponentInParent<PartOfLevelEditor>().uniqueId;
			if (parentId > -1)
				output += "parent=" + parentId + statementSeperater;
			else
				output += statementSeperater;
			if (trs.position != type.trs.position)
				output += "pos=" + (int) trs.position.x + paramSeperator.ToString() + (int) trs.position.y + statementSeperater;
			else
				output += statementSeperater;
			if (trs.eulerAngles.z != type.trs.eulerAngles.z)
				output += "rot=" + (int) trs.eulerAngles.z + statementSeperater;
			else
				output += statementSeperater;
			if ((Vector2) trs.lossyScale != (Vector2) type.trs.lossyScale)
				output += "size=" + (int) trs.lossyScale.x + paramSeperator.ToString() + (int) trs.lossyScale.y + statementSeperater;
			else
				output += statementSeperater;
			return output;
		}
	}
}