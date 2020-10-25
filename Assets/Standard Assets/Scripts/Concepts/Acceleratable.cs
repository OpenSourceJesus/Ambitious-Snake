using UnityEngine;
using System.Collections.Generic;
using Extensions;
using AmbitiousSnake;

public class Acceleratable : MonoBehaviour, IUpdatable
{
	public bool PauseWhileUnfocused
	{
		get
		{
			return true;
		}
	}
	[HideInInspector]
	public List<Vector2> forces = new List<Vector2>();
	public Rigidbody2D rigid;

	void OnEnable ()
	{
		GameManager.updatables = GameManager.updatables.Add(this);
	}

	void OnDisable ()
	{
		GameManager.updatables = GameManager.updatables.Remove(this);
	}
	
	public void DoUpdate ()
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
