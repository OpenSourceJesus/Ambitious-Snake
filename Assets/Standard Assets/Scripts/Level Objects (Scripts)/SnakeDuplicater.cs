using UnityEngine;
using Extensions;

namespace AmbitiousSnake
{
	public class SnakeDuplicater : MonoBehaviour
	{
		public SnakeMimic snakeMimicPrefab;
		bool hasBeenUsed;
		public SpriteRenderer[] spriteRenderers = new SpriteRenderer[0];
		
		public virtual void OnTriggerEnter2D (Collider2D other)
		{
			if (hasBeenUsed || SnakeRecorder.areRecording.Length == 0)
				return;
			hasBeenUsed = true;
			foreach (SpriteRenderer spriteRenderer in spriteRenderers)
				spriteRenderer.color = spriteRenderer.color.DivideAlpha(2);
			SnakeMimic mimic = (SnakeMimic) Instantiate(snakeMimicPrefab);
			mimic.playing = SnakeRecorder.areRecording[SnakeRecorder.areRecording.Length - 1].currentRecording;
			SnakeRecorder.areRecording[0].isRecording = false;
		}
	}
}