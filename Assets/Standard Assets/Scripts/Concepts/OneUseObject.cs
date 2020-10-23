using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneUseObject : MonoBehaviour
{
	static List<string> instances = new List<string>();
	
	void Awake ()
	{
		if (instances.Contains(this.name))
		{
			DestroyImmediate(this.gameObject);
			return;
		}
		instances.Add(this.name);
	}
}
