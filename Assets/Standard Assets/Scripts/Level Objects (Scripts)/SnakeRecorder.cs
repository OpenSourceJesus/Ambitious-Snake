using System.Collections;
using UnityEngine;
using Extensions;

namespace AmbitiousSnake
{
	public class SnakeRecorder : MonoBehaviour
	{
		public static SnakeRecorder[] areRecording = new SnakeRecorder[0];
		[HideInInspector]
		public bool isRecording;
		public SnakeRecording currentRecording;
		bool hasBeenUsed;
		public SpriteRenderer[] spriteRenderers = new SpriteRenderer[0];
		
		public virtual void OnTriggerEnter2D (Collider2D other)
		{
			if (hasBeenUsed)
				return;
			hasBeenUsed = true;
			foreach (SpriteRenderer renderer in spriteRenderers)
				renderer.color = renderer.color.DivideAlpha(2);
			StartCoroutine(Record ());
		}

		public virtual IEnumerator Record ()
		{
			areRecording = areRecording.Add(this);
			isRecording = true;
			currentRecording = new SnakeRecording();
			foreach (Vector3 vertex in Snake.instance.verticies)
				currentRecording.nextPositions.Add(vertex);
			currentRecording.translations.Add(Snake.instance.trs.position);
			while (isRecording)
			{
				currentRecording.nextPositions.Add(Snake.instance.verticies[Snake.instance.verticies.Count - 1]);
				currentRecording.lengths.Add(Snake.instance.actualLength);
				currentRecording.translations.Add(Snake.instance.trs.position);
				yield return new WaitForFixedUpdate();
			}
			areRecording = areRecording.Remove(this);
		}
	}
}