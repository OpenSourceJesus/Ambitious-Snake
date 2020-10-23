using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmbitiousSnake;

[ExecuteAlways]
public class ClearData : MonoBehaviour
{
	public virtual void Awake ()
	{
		SaveAndLoadManager.ClearData ();
		DestroyImmediate(this);
	}
}
