using UnityEngine;
using System.Collections.Generic;
using Extensions;

namespace AmbitiousSnake
{
	public class Turret : NotPartOfLevelEditor
	{
		public ComplexTimer fireRate;
		public int bulletPrefabIndex;
		public float bulletWidth;
		LineRenderer line;
		public Color lockedOnColor;
		public Color searchingColor;
		Transform trs;
		Vector2 shootDir;
		Vector3 snakeVertex;
		Vector2 toSnakeVertex;
		RaycastHit2D hitSnake;
		Laser laser;
		
		void Start ()
		{
			trs = GetComponent<Transform>();
			line = trs.GetChild(0).GetComponent<LineRenderer>();
			laser = GetComponentInChildren<Laser>();
			laser.Start ();
		}
		
		void FixedUpdate ()
		{
			if (GameManager.paused)
				return;
			shootDir = VectorExtensions.NULL;
			for (int i = GameManager.GetSingleton<Snake>().verticies.Count; i >= 0; i --)
			{
				snakeVertex = GameManager.GetSingleton<Snake>().GetVertexPos(i);
				toSnakeVertex = snakeVertex - trs.position;
				trs.rotation = Quaternion.LookRotation(Vector3.forward, toSnakeVertex);
				laser.Update ();
				if (laser.hitBlocker.collider != null && laser.hitBlocker.transform.root == GameManager.GetSingleton<Snake>().trs)
				{
					shootDir = toSnakeVertex;
					line.SetPosition(1, Vector2.up * toSnakeVertex.magnitude);
					break;
				}
			}
			if (shootDir != (Vector2) VectorExtensions.NULL)
			{
				trs.rotation = Quaternion.LookRotation(Vector3.forward, shootDir);
				line.startColor = lockedOnColor;
				line.endColor = lockedOnColor;
				if (fireRate.IsAtEnd())
				{
					fireRate.JumpToStart();
					GameManager.GetSingleton<ObjectPool>().Spawn(bulletPrefabIndex, trs.position, Quaternion.LookRotation(Vector3.forward, shootDir));
				}
			}
			else
			{
				transform.rotation = Quaternion.LookRotation(Vector3.forward, GameManager.GetSingleton<Snake>().GetHeadPos() - (Vector2) trs.position);
				line.startColor = searchingColor;
				line.endColor = searchingColor;
			}
		}
	}
}