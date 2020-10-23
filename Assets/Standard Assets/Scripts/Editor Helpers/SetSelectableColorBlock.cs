using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[ExecuteAlways]
public class SetSelectableColorBlock : MonoBehaviour
{
	public bool update;
	public ColorBlock colors;
	
	void Awake ()
	{
		if (GetComponent<_Selectable>() == null)
			gameObject.AddComponent<_Selectable>();
	}
	
	void Update ()
	{
		if (!update)
			return;
		GetComponent<Selectable>().colors = colors;
		update = false;
	}
}
