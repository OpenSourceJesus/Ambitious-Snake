using UnityEngine;
using System.Collections;
using AmbitiousSnake;

[ExecuteAlways]
public class VectorGridForce : MonoBehaviour 
{
	public Transform trs;
	public float scale;
	public bool isDirectional;
	public Vector3 direction;
	public float radius;
	public Color color = Color.white;
	public bool hasColor;
	
	public virtual void Start ()
	{
// #if UNITY_EDTIOR
// 		if (!Application.isPlaying)
// 		{
			if (trs == null)
				trs = GetComponent<Transform>();
// 			return;
// 		}
// #endif
	}

	public virtual void Update () 
	{
		if (GameManager.paused || !VectorGrid.active)
			return;
		if (isDirectional)
			VectorGrid.instance.AddGridForce(trs.position, direction * scale, radius, color, hasColor);
		else
			VectorGrid.instance.AddGridForce(trs.position, scale, radius, color, hasColor);
	}
}
