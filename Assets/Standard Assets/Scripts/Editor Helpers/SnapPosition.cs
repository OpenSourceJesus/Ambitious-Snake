using UnityEngine;
using Extensions;

[ExecuteAlways]
public class SnapPosition : MonoBehaviour
{
	public Vector3 snapTo;
	public Vector3 offset;
	public Transform trs;
	Vector3 newPos;
	
	public virtual void Start ()
	{
		if (Application.isPlaying)
		{
			DestroyImmediate(this);
			return;
		}
		trs = GetComponent<Transform>();
	}
	
	public virtual void Update ()
	{
		newPos = VectorExtensions.Snap(trs.position, snapTo) + offset;
		if (snapTo.x == 0)
			newPos.x = trs.position.x;
		if (snapTo.y == 0)
			newPos.y = trs.position.y;
		if (snapTo.z == 0)
			newPos.z = trs.position.z;
		trs.position = newPos;
	}
}