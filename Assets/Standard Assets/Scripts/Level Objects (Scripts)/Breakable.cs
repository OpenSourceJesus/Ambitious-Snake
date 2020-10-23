using UnityEngine;
using System.Collections;
using Extensions;

namespace AmbitiousSnake
{
	public class Breakable : NotPartOfLevelEditor
	{
		public ComplexTimer breakTimer;
		new SpriteRenderer renderer;
		
		void Start ()
		{
			if (breakTimer == null)
				breakTimer = GetComponentInChildren<ComplexTimer>();
			breakTimer.JumpToStart ();
			breakTimer.Pause ();
			renderer = GetComponent<SpriteRenderer>();
		}
		
		void Update ()
		{
			renderer.color = ColorExtensions.SetAlpha(renderer.color, (breakTimer.value.max - breakTimer.GetValue()) / breakTimer.value.max);
			if (breakTimer.value.GetValue() == breakTimer.value.max)
				Destroy (gameObject);
		}
		
		public void OnCollisionEnter2D (Collision2D coll)
		{
			breakTimer.Resume ();
		}
	}
}