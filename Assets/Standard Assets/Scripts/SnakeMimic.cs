using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmbitiousSnake
{
	public class SnakeMimic : Snake
	{
		[HideInInspector]
		public SnakeRecording playing;
		int currentFrame;
		int positionIndex;
		public Transform head;
		bool update;
		
		public override void Awake ()
		{
			line = GetComponent<LineRenderer>();
			trs = GetComponent<Transform>();
			collider = GetComponent<EdgeCollider2D>();
			whatICrashInto = Physics2D.GetLayerCollisionMask(gameObject.layer);
			//whatICrashInto = LayerMaskExtensions.AddToMask(whatICrashInto, gameObject.layer);
			StartCoroutine(Init ());
		}
		
		public override void DoUpdate ()
		{
			if (currentFrame >= playing.lengths.Count - 1)
			{
				StartCoroutine(Init ());
				return;
			}
			if (!update || GameManager.paused || WillCollide())
				return;
			Translate ();
			Move ();
			SetLength ();
			NextFrame ();
		}
		
		public override void OnCollisionStay2D (Collision2D coll)
		{
			
		}
		
		public override void Move ()
		{
			verticies.Add(playing.nextPositions[positionIndex]);
			if (positionIndex > 0)
			{
				actualLength += Vector2.Distance(verticies[verticies.Count - 1], verticies[verticies.Count - 2]);
				SetFacing (verticies[verticies.Count - 1] - verticies[verticies.Count - 2]);
			}
			head.right = facingVector;
			head.position = trs.TransformPoint(verticies[verticies.Count - 1]);
			collider.points = verticies.ToArray();
		}
		
		public override void SetLength ()
		{
			line.positionCount = verticies.Count;
			for (int i = 0; i < verticies.Count; i ++)
				line.SetPosition(i, verticies[i]);
			while (actualLength > playing.lengths[currentFrame])
			{
				actualLength -= Vector2.Distance(verticies[0], verticies[1]);
				verticies.RemoveAt(0);
			}
		}
		
		bool WillCollide ()
		{
			Physics2D.queriesStartInColliders = false;
			Vector2 offset = playing.translations[currentFrame + 1] - (Vector2) trs.position;
			verticies.Add(playing.nextPositions[positionIndex + 1]);
			for (int i = 1; i < verticies.Count; i ++)
			{
				if (Physics2D.Linecast(GetVertexPos(i) + offset, GetVertexPos(i - 1) + offset, whatICrashInto).collider != null)
					return true;
			}
			verticies.RemoveAt(verticies.Count - 1);
			return false;
		}
		
		IEnumerator Init ()
		{
			update = false;
			currentFrame = 0;
			positionIndex = 0;
			verticies.Clear();
			actualLength = 0;
			Translate ();
			bool done = false;
			int hitColliderIndex = 0;
			Physics2D.queriesStartInColliders = true;
			while (!done)
			{
				for (int i = 1; i < playing.nextPositions.Count - playing.lengths.Count; i ++)
				{
					while (Physics2D.Linecast(trs.TransformPoint(playing.nextPositions[i]), trs.TransformPoint(playing.nextPositions[i - 1]), whatICrashInto).collider != null)
					{
						hitColliderIndex = i;
						yield return new WaitForFixedUpdate();
					}
				}
				if (hitColliderIndex > 0)
				{
					bool hitAnotherCollider = false;
					for (int i = 1; i < hitColliderIndex; i ++)
					{
						if (Physics2D.Linecast(trs.TransformPoint(playing.nextPositions[i]), trs.TransformPoint(playing.nextPositions[i - 1]), whatICrashInto).collider != null)
						{
							hitAnotherCollider = true;
							break;
						}
					}
					if (!hitAnotherCollider)
						done = true;
				}
				else
					done = true;
			}
			head.gameObject.SetActive(true);
			for (int i = 0; i < playing.nextPositions.Count - playing.lengths.Count; i ++)
			{
				Move ();
				positionIndex ++;
			}
			update = true;
		}
		
		void Translate ()
		{
			trs.position = playing.translations[currentFrame];
		}
		
		void NextFrame ()
		{
			if (currentFrame < playing.lengths.Count - 1)
			{
				currentFrame ++;
				positionIndex ++;
			}
		}
	}
}