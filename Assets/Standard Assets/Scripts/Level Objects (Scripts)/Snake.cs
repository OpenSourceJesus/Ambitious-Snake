using UnityEngine;
using System.Collections.Generic;
using Extensions;

namespace AmbitiousSnake
{
	[ExecuteAlways]
	[RequireComponent(typeof(SparkCreator), typeof(EdgeCollider2D))]
	public class Snake : NotPartOfLevelEditor, IUpdatable
	{
		public static Snake instance;
		[HideInInspector]
		public float facingAngle;
		[HideInInspector]
		public Vector2 facingVector;
		public int moveSpeed;
		public int growRate;
		public ClampedFloat targetLength;
		[HideInInspector]
		public List<Vector2> verticies = new List<Vector2>();
		public LineRenderer line;
		public Rigidbody2D rigid;
		public int numberOfGridForces;
		[HideInInspector]
		public VectorGridForce[] gridForces;
		public VectorGridForce gridForceTemplate;
		public float airGravity;
		public string moveXAxis;
		public string moveYAxis;
		public string growAxis;
		public float floatAmount;
		public SparkCreator sparkCreator;
		public Transform trs;
		public new EdgeCollider2D collider;
		public float actualLength;
		public LayerMask whatICrashInto;
		public float stuckGravity;
		public LayerMask whatIStickTo;
		// public float makeVertexInterval = 0.01f;
		// float distRemainingTillMakeVertex;
		public CircleCollider2D headCollider;
		public static bool hasStar;
		public bool PauseWhileUnfocused
		{
			get
			{
				return true;
			}
		}
		
		public override void Awake ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				if (trs == null)
					trs = GetComponent<Transform>();
				if (headCollider == null)
					headCollider = GetComponent<CircleCollider2D>();
				return;
			}
#endif
			base.Awake ();
			instance = this;
			// distRemainingTillMakeVertex = makeVertexInterval;
			headCollider.enabled = false;
			whatICrashInto = Physics2D.GetLayerCollisionMask(gameObject.layer);
			LevelMap.mapBounds = LevelMap.GetBounds();
			SetFacing (trs.right);
			trs.eulerAngles = Vector3.zero;
			verticies.Add(Vector3.zero);
			SetFacing (facingAngle);
			hasStar = false;
			gridForceTemplate = GetComponent<VectorGridForce>();
			gridForces = new VectorGridForce[numberOfGridForces];
			for (int i = 0; i < numberOfGridForces; i ++)
			{
				VectorGridForce gridForce = new GameObject().AddComponent<VectorGridForce>();
				gridForce.trs = gridForce.GetComponent<Transform>();
				gridForce.trs.SetParent(trs);
				gridForce.scale = gridForceTemplate.scale;
				gridForce.direction = gridForceTemplate.direction;
				gridForce.radius = gridForceTemplate.radius;
				gridForce.color = gridForceTemplate.color;
				gridForce.hasColor = gridForceTemplate.hasColor;
				gridForces[i] = gridForce;
			}
			GameManager.updatables = GameManager.updatables.Add(this);
		}

		public virtual void OnDestroy ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
#endif
			GameManager.updatables = GameManager.updatables.Remove(this);
		}
		
		public virtual void DoUpdate ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
#endif
			if (GameManager.isInSceneTransition || GameManager.paused)
				return;
			HandleSetFacing ();
			Vector2 moveVector = (facingVector.normalized * moveSpeed * Time.deltaTime) + (rigid.velocity * Time.deltaTime);
			Physics2D.queriesStartInColliders = true;
			Collider2D hit = Physics2D.OverlapPoint(GetHeadPosition() + moveVector, whatICrashInto);
			SetLength ();
			if (hit == null)
				Move ();
			else
			{
				Breakable breakTile = hit.GetComponent<Breakable>();
				if (breakTile != null)
					breakTile.OnCollisionEnter2D (null);
				else
				{
					Hazard hazard = hit.GetComponent<Hazard>();
					if (hazard != null)
						hazard.OnCollisionEnter2D(null);
				}
				StartCoroutine(sparkCreator.CreateSparkRoutine (GetHeadPosition() + moveVector));
			}
			MakeRipples ();
			if (IsStuck())
				Gravity (stuckGravity);
			else
				Gravity (airGravity);
		}
		
		public virtual bool IsStuck ()
		{
			Physics2D.queriesStartInColliders = true;
			Vector2 previousVertexPosition = GetVertexPosition(0);
			Vector2 vertexPosition;
			for (int i = 1; i < verticies.Count; i ++)
			{
				vertexPosition = GetVertexPosition(i);
				if (Physics2D.Linecast(previousVertexPosition, vertexPosition, whatIStickTo).collider != null)
					return true;
				previousVertexPosition = vertexPosition;
			}
			return false;
		}
		
		public virtual void MakeRipples ()
		{
			for (int i = 0; i < gridForces.Length; i ++)
			{
				VectorGridForce gridForce = gridForces[i];
				Vector2 vertexPosition = GetVertexPosition((int) ((verticies.Count - 1) * ((float) i / (gridForces.Length - 1))));
				gridForce.trs.position = vertexPosition;
				Vector3 direction = new Vector3();
				if (i > 0 || i < gridForces.Length - 1)
					direction = -(vertexPosition - GetVertexPosition((int) ((verticies.Count - 1) * ((float) i / (gridForces.Length - 1)) + 1)));
				else if (i == 0)
					direction = -(GetVertexPosition(1) - vertexPosition);
				else if (i == gridForces.Length - 1)
					direction = -(GetVertexPosition(verticies.Count - 2) - vertexPosition);
				if (i > 0)
					direction = gridForces[i - 1].trs.position - gridForce.trs.position;
				direction.z = gridForceTemplate.direction.z;
				gridForce.direction = direction + (Vector3) (rigid.velocity / 100);
				gridForce.trs.up = gridForce.direction;
			}
		}
		
		public virtual void HandleSetFacing ()
		{
			if (InputManager.UsingGamepad)
			{
				Vector2 newFacing = InputManager.MoveInput;
				if (newFacing.magnitude > 0)
					SetFacing (newFacing);
			}
			else
				SetFacing ((Vector2) Camera.main.ScreenToWorldPoint(InputManager.MousePosition) - GetHeadPosition());
		}

		float speed;
		Vector2 move;
		public virtual void Move ()
		{
			speed = moveSpeed * Time.deltaTime;
			// float extraAmount = (actualLength + speed) - targetLength.GetValue();
			// if (extraAmount < speed && extraAmount > 0)
			// 	speed -= extraAmount;
			move = facingVector * speed;
			// if (distRemainingTillMakeVertex <= 0)
			// {
			// 	do
			// 	{
					verticies.Add(verticies[verticies.Count - 1] + move);
					actualLength += speed;
					// distRemainingTillMakeVertex += makeVertexInterval;
				// } while (distRemainingTillMakeVertex <= 0);
			// }
			// else
			// 	distRemainingTillMakeVertex -= speed;
			collider.points = verticies.ToArray();
			line.positionCount = verticies.Count;
			for (int i = 0; i < verticies.Count; i ++)
				line.SetPosition(i, verticies[i]);
		}
		
		public virtual void SetLength ()
		{
			targetLength.SetValue(targetLength.GetValue() + InputManager.ChangeLengthInput * growRate * Time.deltaTime);
			if (actualLength >= targetLength.GetValue())
			{
				do
				{
					Vector2 vertex0 = verticies[0];
					Vector2 vertex1 = verticies[1];
					float distance = Vector2.Distance(vertex0, vertex1);
					float extraAmount = actualLength - targetLength.GetValue();
					if (distance > 0 && extraAmount < distance && extraAmount > 0)
					{
						verticies[0] += (vertex1 - vertex0).normalized * extraAmount;
						actualLength -= extraAmount;
						return;
					}
					else
					{
						verticies.RemoveAt(0);
						actualLength -= distance;
					}
				} while (actualLength >= targetLength.GetValue());
				// collider.points = verticies.ToArray();
			}
		}
		
		public virtual Vector2 GetHeadPosition ()
		{
			return GetVertexPosition(verticies.Count - 1);
		}
		
		public virtual Vector2 GetTailPosition ()
		{
			return GetVertexPosition(0);
		}
		
		public virtual Vector2 GetVertexPosition (int vertexIndex)
		{
			vertexIndex = Mathf.Clamp(vertexIndex, 0, verticies.Count - 1);
			return trs.TransformPoint(verticies[vertexIndex]);
		}
		
		public virtual void SetHasStar (bool hasStar)
		{
			Level.hasStar = hasStar;
		}
		
		public virtual void SetFacing (float angle)
		{
			facingAngle = angle;
			facingVector = VectorExtensions.FromFacingAngle(facingAngle);
		}
		
		public virtual void SetFacing (Vector2 direction)
		{
			facingVector = direction.normalized;
			facingAngle = facingVector.GetFacingAngle();
		}
		
		public virtual void Gravity (float gravity)
		{
			Physics2D.queriesStartInColliders = false;
			float distFromGround = Mathf.Infinity;
			RaycastHit2D hit;
			for (int i = 0; i < verticies.Count; i ++)
			{
				Vector2 vertexPosition = GetVertexPosition(i);
				hit = Physics2D.Linecast(vertexPosition, vertexPosition + (Vector2.down * gravity * Time.deltaTime), whatICrashInto);
				if (hit.collider != null)
				{
					float checkDistFromGround = vertexPosition.y - hit.point.y;
					if (checkDistFromGround >= 0 && checkDistFromGround < distFromGround)
	                    distFromGround = checkDistFromGround;
					Breakable breakTile = hit.collider.GetComponent<Breakable>();
					if (breakTile != null)
						breakTile.OnCollisionEnter2D (null);
					else
					{
						Hazard hazard = hit.collider.GetComponent<Hazard>();
						if (hazard != null)
							hazard.OnCollisionEnter2D (null);
					}
					StartCoroutine(sparkCreator.CreateSparkRoutine (hit.point));
				}
			}
			if (distFromGround == Mathf.Infinity)
				trs.position += Vector3.down * gravity * Time.deltaTime;
			else
			{
				trs.position += Vector3.down * distFromGround;
				trs.position += Vector3.up * (Physics2D.defaultContactOffset + floatAmount);
			}
		}
		
		public virtual void OnCollisionStay2D (Collision2D coll)
		{
			foreach (ContactPoint2D contact in coll.contacts)
				StartCoroutine(sparkCreator.CreateSparkRoutine (contact.point));
		}
	}
}