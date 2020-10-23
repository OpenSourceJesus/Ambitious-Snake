using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace AmbitiousSnake
{
	public class Portal : NotPartOfLevelEditor
	{
		Portal endPortal;
		Vector2 toEndPortal;
		static List<Teleportee> tryingToTeleport = new List<Teleportee>();
		
		void Start ()
		{
			foreach (Portal portal in FindObjectsOfType<Portal>())
			{
				if (portal != this && portal.GetComponent<SpriteRenderer>().color == GetComponent<SpriteRenderer>().color)
				{
					endPortal = portal;
					return;
				}
			}
		}
		
		void OnTriggerEnter2D (Collider2D other)
		{
			Transform otherTrs = other.transform.root;
			foreach (Teleportee teleportee in tryingToTeleport)
			{
				if (teleportee.root == otherTrs)
				{
					if (!teleportee.justTeleported)
					{
						teleportee.justTeleported = true;
						teleportee.teleportRoutine = StartCoroutine(Teleport (teleportee));
					}
					return;
				}
			}
			Teleportee _teleportee = new Teleportee();
			_teleportee.root = otherTrs;
			_teleportee.teleportRoutine = StartCoroutine(Teleport (_teleportee));
			tryingToTeleport.Add(_teleportee);
		}
		
		IEnumerator Teleport (Teleportee teleportee)
		{
			if (teleportee.root == GameManager.GetSingleton<Snake>().trs)
			{
				bool done = false;
				int hitColliderIndex = 0;
				while (!done)
				{
					for (int i = 1; i < GameManager.GetSingleton<Snake>().verticies.Count; i ++)
					{
						toEndPortal = endPortal.transform.position - transform.position;
						while (Physics2D.Linecast(GameManager.GetSingleton<Snake>().GetVertexPos(i) + toEndPortal, GameManager.GetSingleton<Snake>().GetVertexPos(i - 1) + toEndPortal, GameManager.GetSingleton<Snake>().whatICrashInto).collider != null)
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
							if (Physics2D.Linecast(GameManager.GetSingleton<Snake>().GetVertexPos(i) + toEndPortal, GameManager.GetSingleton<Snake>().GetVertexPos(i - 1) + toEndPortal, GameManager.GetSingleton<Snake>().whatICrashInto).collider != null)
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
				GameManager.GetSingleton<Snake>().trs.position += (Vector3) toEndPortal;
			}
			else
			{
				
			}
		}
		
		void OnTriggerExit2D (Collider2D other)
		{
			Transform otherTrs = other.transform.root;
			for (int i = 0; i < tryingToTeleport.Count; i ++)
			{
				Teleportee teleportee = tryingToTeleport[i];
				if (teleportee.root == otherTrs)
				{
					if (teleportee.justTeleported)
					{
						if (teleportee.teleportRoutine != null)
							StopCoroutine(teleportee.teleportRoutine);
						teleportee.justTeleported = false;
					}
					return;
				}
			}
		}
		
		class Teleportee
		{
			public Transform root;
			public Coroutine teleportRoutine;
			public bool justTeleported;
		}
	}
}