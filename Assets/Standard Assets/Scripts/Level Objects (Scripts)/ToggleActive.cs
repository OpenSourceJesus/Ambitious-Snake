using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Extensions;

namespace AmbitiousSnake
{
	[ExecuteAlways]
	public class ToggleActive : NotPartOfLevelEditor
	{
		public ComplexTimer toggleTimer;
		public LayerMask whatICantAppearInto;
		float timerValue;
		SpriteRenderer spriteRenderer;
		new Collider2D collider;
		public Text timerText;
		const float reduceColliderBoundsExtents = .2f;
		Transform trs;
		
		void Start ()
		{
			spriteRenderer = GetComponent<SpriteRenderer>();
			collider = GetComponent<Collider2D>();
			trs = GetComponent<Transform>();
			HandleSetActive ();
		}
		
		void Update ()
		{
			timerValue = toggleTimer.GetValue();
			if (!Application.isPlaying)
			{
				HandleSetActive ();
				return;
			}
			float timeRemaining = timerValue;
			if (toggleTimer.changeAmountMultiplier > 0)
				timeRemaining = toggleTimer.value.max - timerValue;
			if (timeRemaining > 1)
				timerText.text = "" + MathfExtensions.RoundToInterval(timeRemaining, 1f);
			else
				timerText.text = "" + MathfExtensions.RoundToInterval(timeRemaining, .1f);
			HandleSetActive ();
		}
		
		void HandleSetActive ()
		{
			if (timerValue == toggleTimer.value.min)
				_SetActive(true);
			else if (timerValue == toggleTimer.value.max)
				_SetActive(false);
		}
		
		void _SetActive (bool _active)
		{
			if (Application.isPlaying && _active)
			{
				bool boxColliding = collider.GetType() == typeof(BoxCollider2D) && Physics2D.OverlapBox(trs.position, (Vector2) trs.lossyScale - (Vector2.one * reduceColliderBoundsExtents * 2), whatICantAppearInto) != null;
				bool circleColliding = collider.GetType() == typeof(CircleCollider2D) && Physics2D.OverlapCircle(trs.position, trs.lossyScale.x / 2 - reduceColliderBoundsExtents, whatICantAppearInto) != null;
				if (boxColliding || circleColliding)
					return;
			}
			if (timerValue == toggleTimer.value.max)
				toggleTimer.changeAmountMultiplier = -1;
			else
				toggleTimer.changeAmountMultiplier = 1;
			if (_active)
				spriteRenderer.color = ColorExtensions.SetAlpha(spriteRenderer.color, 1);
			else
				spriteRenderer.color = ColorExtensions.SetAlpha(spriteRenderer.color, .25f);
			collider.enabled = _active;
		}
	}
}