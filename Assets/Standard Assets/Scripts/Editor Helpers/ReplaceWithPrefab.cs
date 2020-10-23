using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class ReplcaeWithPrefab : MonoBehaviour
{
	public Transform prefab;
	Transform trs;
	
	void Start ()
	{
		this.trs = this.GetComponent<Transform>();
	}
	
	void Update ()
	{
		if (this.prefab != null)
		{
			Transform newTrs = (Transform) Instantiate(this.prefab, this.trs.position, this.trs.rotation, this.trs.parent);
			newTrs.localScale = this.trs.localScale;
			DestroyImmediate(gameObject);
		}
	}
}
