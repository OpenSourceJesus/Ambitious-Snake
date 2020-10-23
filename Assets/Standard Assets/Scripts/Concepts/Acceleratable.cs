using UnityEngine;
using System.Collections.Generic;

public class Acceleratable : MonoBehaviour
{
	[HideInInspector]
	public List<Vector2> forces = new List<Vector2>();
	public Rigidbody2D rigid;
	
	void FixedUpdate ()
	{
		List<Vector2> appliedForces = new List<Vector2>();
		foreach (Vector2 force in forces)
		{
			if (!appliedForces.Contains(force))
			{
				this.rigid.AddForce(force);
				appliedForces.Add(force);
			}
		}
	}
}
