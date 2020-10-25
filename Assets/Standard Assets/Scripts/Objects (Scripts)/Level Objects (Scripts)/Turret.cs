using UnityEngine;
using Extensions;

namespace AmbitiousSnake
{
	public class Turret : NotPartOfLevelEditor, IUpdatable
	{
		public bool PauseWhileUnfocused
		{
			get
			{
				return true;
			}
		}
		public ComplexTimer fireRate;
		public int bulletPrefabIndex;
		public float bulletWidth;
		public LineRenderer line;
		public Color lockedOnColor;
		public Color searchingColor;
		public Transform trs;
		public Laser laser;
		Vector2 shootDir;
		Vector3 snakeVertex;
		Vector2 toSnakeVertex;
		RaycastHit2D hitSnake;

		void OnEnable ()
		{
			GameManager.updatables = GameManager.updatables.Add(this);
		}
		
		public void DoUpdate ()
		{
			if (GameManager.paused)
				return;
			shootDir = VectorExtensions.NULL;
			for (int i = Snake.instance.verticies.Count; i >= 0; i --)
			{
				snakeVertex = Snake.instance.GetVertexPosition(i);
				toSnakeVertex = snakeVertex - trs.position;
				trs.rotation = Quaternion.LookRotation(Vector3.forward, toSnakeVertex);
				laser.DoUpdate ();
				if (laser.hitBlocker.collider != null && laser.hitBlocker.transform.root == Snake.instance.trs)
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
					ObjectPool.Instance.Spawn(bulletPrefabIndex, trs.position, Quaternion.LookRotation(Vector3.forward, shootDir));
				}
			}
			else
			{
				transform.rotation = Quaternion.LookRotation(Vector3.forward, Snake.instance.GetHeadPosition() - (Vector2) trs.position);
				line.startColor = searchingColor;
				line.endColor = searchingColor;
			}
		}

		void OnDisable ()
		{
			GameManager.updatables = GameManager.updatables.Remove(this);
		}
	}
}