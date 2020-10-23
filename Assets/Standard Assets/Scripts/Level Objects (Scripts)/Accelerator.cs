using UnityEngine;
using System.Collections;

namespace AmbitiousSnake
{
	public class Accelerator : NotPartOfLevelEditor
	{
		public int force;
		
		void OnCollisionEnter2D (Collision2D coll)
		{
			OnTriggerEnter2D (coll.collider);
		}
		
		void OnTriggerEnter2D (Collider2D other)
		{
			Acceleratable acceleratable = other.GetComponentInParent<Acceleratable>();
			if (!acceleratable.forces.Contains(transform.up * force))
				acceleratable.forces.Add(transform.up * force);
		}
		
		void OnCollisionStay2D (Collision2D coll)
		{
			OnTriggerStay2D (coll.collider);
		}
		
		void OnTriggerStay2D (Collider2D other)
		{
			Acceleratable acceleratable = other.GetComponentInParent<Acceleratable>();
			if (!acceleratable.forces.Contains(transform.up * force))
				acceleratable.forces.Add(transform.up * force);
		}
		
		void OnCollisionExit2D (Collision2D coll)
		{
			OnTriggerExit2D (coll.collider);
		}
		
		void OnTriggerExit2D (Collider2D other)
		{
			Acceleratable acceleratable = other.GetComponentInParent<Acceleratable>();
			if (acceleratable.forces.Contains(transform.up * force))
				acceleratable.forces.Remove(transform.up * force);
		}
	}
}