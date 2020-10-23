using UnityEngine;
using System.Collections;

namespace AmbitiousSnake
{
	public class ComplexTimer : NotPartOfLevelEditor
	{
		public string title;
		public bool reactsToPause;
		public bool realtime;
		public bool useFixedUpdate = true;
		public ClampedFloat value;
		public float changeAmountMultiplier;
		public RepeatType repeatType;
		float timeSinceLastGetValue;
		float previousChangeAmountMultiplier;
		[HideInInspector]
		public float initValue;
		
		public virtual float GetValue ()
		{
			value.SetValue(value.GetValue() + timeSinceLastGetValue * changeAmountMultiplier);
			if ((value.GetValue() == value.max && changeAmountMultiplier > 0) || (value.GetValue() == value.min && changeAmountMultiplier < 0))
			{
				if (repeatType == RepeatType.Loop)
					JumpToStart ();
				else if (repeatType  == RepeatType.PingPong)
					changeAmountMultiplier *= -1;
			}
			timeSinceLastGetValue = 0;
			return value.GetValue();
		}
		
		public override void Awake ()
		{
			base.Awake();
			if (value == null)
				value = new ClampedFloat();
			initValue = value.GetValue();
		}
		
		public virtual void Update ()
		{
			if (!useFixedUpdate && !(GameManager.paused && reactsToPause))
			{
				if (realtime)
					UpdateTimer(Time.unscaledDeltaTime);
				else if (!GameManager.paused)
					UpdateTimer(Time.deltaTime);
			}
		}
		
		public virtual void FixedUpdate ()
		{
			if (useFixedUpdate && !(GameManager.paused && reactsToPause))
			{
				if (realtime)
					UpdateTimer(Time.unscaledDeltaTime);
				else if (!GameManager.paused)
					UpdateTimer(Time.fixedDeltaTime);
			}
		}
		
		public virtual void UpdateTimer (float deltaTime)
		{
			timeSinceLastGetValue += deltaTime;
		}
		
		public virtual bool IsAtStart ()
		{
			float timerValue = GetValue();
			return (timerValue == value.max && changeAmountMultiplier < 0) || (timerValue == value.min && changeAmountMultiplier > 0) || ((timerValue == value.min || timerValue == value.max) && changeAmountMultiplier == 0);
		}
		
		public virtual bool IsAtEnd ()
		{
			float timerValue = GetValue();
			return (timerValue == value.min && changeAmountMultiplier < 0) || (timerValue == value.max && changeAmountMultiplier > 0) || ((timerValue == value.min || timerValue == value.max) && changeAmountMultiplier == 0);
		}
		
		public virtual void Pause ()
		{
			if (changeAmountMultiplier == 0)
				return;
			previousChangeAmountMultiplier = changeAmountMultiplier;
			changeAmountMultiplier = 0;
		}
		
		public virtual void Resume ()
		{
			if (changeAmountMultiplier != 0)
				return;
			changeAmountMultiplier = previousChangeAmountMultiplier;
		}
		
		public virtual void JumpToStart ()
		{
			if (changeAmountMultiplier > 0 || (changeAmountMultiplier == 0 && previousChangeAmountMultiplier > 0))
				value.SetValue(value.min);
			else
				value.SetValue(value.max);
		}
		
		public virtual void JumpToEnd ()
		{
			if (changeAmountMultiplier > 0 || (changeAmountMultiplier == 0 && previousChangeAmountMultiplier > 0))
				value.SetValue(value.max);
			else
				value.SetValue(value.min);
		}
		
		public virtual void JumpToInitValue ()
		{
			value.SetValue(initValue);
		}
	}

	public enum RepeatType
	{
		End = 0,
		Loop = 1,
		PingPong = 2
	}
}