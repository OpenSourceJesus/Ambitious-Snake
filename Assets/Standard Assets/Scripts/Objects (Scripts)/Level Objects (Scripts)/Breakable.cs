using UnityEngine;
using Extensions;

namespace AmbitiousSnake
{
	public class Breakable : NotPartOfLevelEditor
	{
		public ComplexTimer breakTimer;
		public SpriteRenderer spriteRenderer;
		
		void Start ()
		{
			if (breakTimer == null)
				breakTimer = GetComponentInChildren<ComplexTimer>();
			breakTimer.JumpToStart ();
			breakTimer.Pause ();
		}
		
		void Update ()
		{
			spriteRenderer.color = ColorExtensions.SetAlpha(spriteRenderer.color, (breakTimer.value.max - breakTimer.GetValue()) / breakTimer.value.max);
			if (breakTimer.value.GetValue() == breakTimer.value.max)
				Destroy (gameObject);
		}
		
		public void OnCollisionEnter2D (Collision2D coll)
		{
			breakTimer.Resume ();
		}
	}
}