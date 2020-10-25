using UnityEngine;
using Extensions; 
using System;
using UnityEngine.Events;

namespace AmbitiousSnake
{
	[ExecuteInEditMode]
	public class Hotkey : MonoBehaviour, IUpdatable
	{
		public bool PauseWhileUnfocused
		{
			get
			{
				return true;
			}
		}
		public bool runWhilePaused;
		public InputButtonTrigger[] inputButtonTriggers = new InputButtonTrigger[0];
		public InputAxisTrigger[] inputAxisTriggers = new InputAxisTrigger[0];
		
		void OnEnable ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
#endif
			GameManager.updatables = GameManager.updatables.Add(this);
		}
		
		public void DoUpdate ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
#endif
			if (!runWhilePaused && GameManager.paused)
				return;
			foreach (InputButtonTrigger inputButtonTrigger in inputButtonTriggers)
				inputButtonTrigger.Update ();
			foreach (InputAxisTrigger inputAxisTrigger in inputAxisTriggers)
				inputAxisTrigger.Update ();
		}

		void OnDisable ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
#endif
			GameManager.updatables = GameManager.updatables.Remove(this);
		}
		
		[Serializable]
		public class InputTrigger<T>
		{
			public string inputMemberPath;
			public InputState state;
			public UnityEvent unityEvent;
			[HideInInspector]
			public T value;
			[HideInInspector]
			public T previousValue;

			public virtual void Update ()
			{
				previousValue = value;
				value = InputManager.Instance.GetMember<T>(inputMemberPath);
			}
		}

		[Serializable]
		public class InputButtonTrigger : InputTrigger<bool>
		{
			public override void Update ()
			{
				base.Update ();
				if (state == InputState.Down)
				{
					if (value && !previousValue)
						unityEvent.Invoke();
				}
				else if (state == InputState.Held)
				{
					if (value)
						unityEvent.Invoke();
				}
				else if (!value && previousValue)
					unityEvent.Invoke();
			}
		}

		[Serializable]
		public class InputAxisTrigger : InputTrigger<float>
		{
			public AxisRange axisRange; 

			public override void Update ()
			{
				base.Update ();
				if (state == InputState.Down)
				{
					if (axisRange == AxisRange.Positive)
					{
						if (value > InputManager.Settings.defaultDeadzoneMin && previousValue <= InputManager.Settings.defaultDeadzoneMin)
							unityEvent.Invoke();
					}
					else if (axisRange == AxisRange.Negative)
					{
						if (value < -InputManager.Settings.defaultDeadzoneMin && previousValue >= -InputManager.Settings.defaultDeadzoneMin)
							unityEvent.Invoke();
					}
					else if (Mathf.Abs(value) > InputManager.Settings.defaultDeadzoneMin && Mathf.Abs(previousValue) <= InputManager.Settings.defaultDeadzoneMin)
						unityEvent.Invoke();
				}
				else if (state == InputState.Held)
				{
					if (axisRange == AxisRange.Positive)
					{
						if (value > InputManager.Settings.defaultDeadzoneMin)
							unityEvent.Invoke();
					}
					else if (axisRange == AxisRange.Negative)
					{
						if (value < -InputManager.Settings.defaultDeadzoneMin)
							unityEvent.Invoke();
					}
					else if (Mathf.Abs(value) > InputManager.Settings.defaultDeadzoneMin)
						unityEvent.Invoke();
				}
				else
				{
					if (axisRange == AxisRange.Positive)
					{
						if (value <= InputManager.Settings.defaultDeadzoneMin && previousValue > InputManager.Settings.defaultDeadzoneMin)
							unityEvent.Invoke();
					}
					else if (axisRange == AxisRange.Negative)
					{
						if (value >= -InputManager.Settings.defaultDeadzoneMin && previousValue < -InputManager.Settings.defaultDeadzoneMin)
							unityEvent.Invoke();
					}
					else if (Mathf.Abs(value) <= InputManager.Settings.defaultDeadzoneMin && Mathf.Abs(previousValue) > InputManager.Settings.defaultDeadzoneMin)
						unityEvent.Invoke();
				}
			}

			public enum AxisRange
			{
				Positive,
				Negative,
				Full
			}
		}

		public enum InputState
		{
			Down,
			Held,
			Up
		}
	}
}