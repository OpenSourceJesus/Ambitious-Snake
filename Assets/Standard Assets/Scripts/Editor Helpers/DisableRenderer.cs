using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableRenderer : MonoBehaviour
{
	void Start ()
	{
		this.GetComponent<Renderer>().enabled = false;
		Destroy(this);
	}
}
