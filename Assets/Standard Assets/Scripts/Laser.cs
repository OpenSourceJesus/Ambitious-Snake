using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
	LineRenderer line;
	public LayerMask whatBlocksMe;
	Transform trs;
	public RaycastHit2D hitBlocker;
	const int maxLength = int.MaxValue;
	
	public void Start ()
	{
		this.line = this.GetComponent<LineRenderer>();
		this.trs = this.transform;
	}
	
	public void Update ()
	{
		this.hitBlocker = Physics2D.Raycast(this.trs.position, this.trs.up, maxLength, whatBlocksMe);
		if (this.hitBlocker.collider != null)
			this.line.SetPosition(1, this.trs.worldToLocalMatrix.MultiplyPoint(this.hitBlocker.point));
		else
			this.line.SetPosition(1, this.trs.up * maxLength);
	}
}
