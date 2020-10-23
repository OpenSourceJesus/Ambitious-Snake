using UnityEngine;
using System.Collections;

namespace AmbitiousSnake
{
	[RequireComponent(typeof(Rigidbody2D), typeof(Acceleratable))]
	public class Bullet : MonoBehaviour
	{
		public int prefabIndex;
		public int speed;
		bool hasBeenAccelerated;
		public int range;
		public Transform trs;
		public Rigidbody2D rigid;
		public VectorGridForce gridForce;
		public Acceleratable acceleratable;
		
		void OnEnable ()
		{
			hasBeenAccelerated = false;
			acceleratable.forces.Clear();
			GameManager.GetSingleton<ObjectPool>().RangeDespawn(prefabIndex, gameObject, trs, range);
		}
		
		void FixedUpdate ()
		{
			if (GameManager.paused)
				return;
			if (acceleratable.forces.Count > 0)
				hasBeenAccelerated = true;
			if (!hasBeenAccelerated)
				rigid.velocity = trs.up * speed * Time.fixedDeltaTime;
			else
				trs.up = rigid.velocity;
			gridForce.direction = rigid.velocity;
		}
		
		void OnCollisionEnter2D (Collision2D coll)
		{
			GameManager.GetSingleton<ObjectPool>().Despawn(prefabIndex, gameObject, trs);
		}
	}
}