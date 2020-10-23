using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

namespace AmbitiousSnake
{
	public class SparkCreator : MonoBehaviour
	{
		public int sparkPrefabIndex;
		ushort sparkCount;
		public float sparkRate;
		float lastSparkTime;
		public ushort maxSparks = 10;
		
		public virtual void Start ()
		{
			enabled = SaveAndLoadManager.GetValue<bool>("Friction Sparks", true);
		}
		
		public virtual void OnCollisionEnter2D (Collision2D coll)
		{
			StartCoroutine(CreateSparkRoutine (coll.contacts[0].point));
		}
		
		public virtual void OnCollisionStay2D (Collision2D coll)
		{
			OnCollisionEnter2D (coll);
		}
		
		public virtual void CreateSpark (Vector2 position)
		{
			StartCoroutine(CreateSparkRoutine (position));
		}

		public virtual IEnumerator CreateSparkRoutine (Vector2 position)
		{
			if (!enabled || sparkCount >= maxSparks || Time.time - lastSparkTime < sparkRate)
				yield break;
			Transform spark = GameManager.GetSingleton<ObjectPool>().SpawnComponent<Transform>(sparkPrefabIndex, position);
			lastSparkTime = Time.time;
			sparkCount ++;
			yield return new WaitForSeconds(.1f);
			GameManager.GetSingleton<ObjectPool>().Despawn(sparkPrefabIndex, spark.gameObject, spark);
			sparkCount --;
		}
	}
}