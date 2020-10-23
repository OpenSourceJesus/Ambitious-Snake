using UnityEngine;
using System.Collections.Generic;
using Extensions;

public class SphereAnimation : SingletonMonoBehaviour<SphereAnimation>
{
	public float missileMapBounds;
	Vector2 missilePos;
	Vector2 missileDest;
	Vector2 missileVel;
	public float missileTurnRate;
	public float missileSpeed;
	float missileDistToDest;
	bool missileHasReducedDistToDest;
	
	public override void Awake ()
	{
		base.Awake ();
		missileVel = Random.insideUnitCircle.normalized * missileSpeed;
		missileDest = Random.insideUnitCircle * missileMapBounds;
	}

	public virtual void Update ()
	{
		missileDistToDest = Vector2.Distance(missilePos, missileDest);
		float missileIdealTurnAmount = Vector2.Angle(missileVel, missileDest - missilePos);
		float missileTurnAmount = Mathf.Clamp(missileIdealTurnAmount, -missileTurnRate * Time.deltaTime, missileTurnRate * Time.deltaTime);
		missileVel = VectorExtensions.Rotate(missileVel, missileTurnAmount);
		missileVel = missileVel.normalized * missileSpeed * Time.deltaTime;
		missilePos += missileVel;
		if (missileHasReducedDistToDest && Vector2.Distance(missilePos, missileDest) > missileDistToDest)
		{
			missileDest = Random.insideUnitCircle * missileMapBounds;
			missileHasReducedDistToDest = false;
		}
		else if (Vector2.Distance(missilePos, missileDest) < missileDistToDest)
			missileHasReducedDistToDest = true;
		transform.RotateAround(transform.position, transform.forward, missileVel.x);
		transform.RotateAround(transform.position, transform.right, missileVel.y);
	}
}
