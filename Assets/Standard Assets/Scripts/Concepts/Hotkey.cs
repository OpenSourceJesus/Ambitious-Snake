using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Extensions;

namespace AmbitiousSnake
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(Button))]
	public class Hotkey : MonoBehaviour, IUpdatable
	{
		public bool PauseWhileUnfocused
		{
			get
			{
				return true;
			}
		}
		public string[] buttonNames;
		public string[] axisNames;
		public State state;
		public Timer holdDownTimer;
		public Button button;
		bool previousAnyKey;
		
		public virtual void Start ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				if (button == null)
					button = GetComponent<Button>();
				return;
			}
#endif
			GameManager.updatables = GameManager.updatables.Add(this);
		}
		
		public void DoUpdate ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
#endif
			if (state == State.Down)
			{
				foreach (string buttonName in buttonNames)
				{
					if (string.IsNullOrEmpty(buttonName) && Input.anyKeyDown)
					{
						Activate ();
						break;
					}
				}
				foreach (string axisName in axisNames)
				{
					// if (InputManager.Instance. != 0 && InputManager.inputter.GetAxisPrev(axisName) == 0)
					// {
					// 	Activate ();
					// 	break;
					// }
				}
			}
			else if (state == State.Up)
			{
				foreach (string buttonName in buttonNames)
				{
					// if ((string.IsNullOrEmpty(buttonName) && ((!Input.anyKey && previousAnyKey) || InputManager.inputter.GetAnyButtonUp())) || (!string.IsNullOrEmpty(buttonName) && InputManager.inputter.GetButtonUp(buttonName)))
					// {
					// 	Activate ();
					// 	break;
					// }	
				}
				foreach (string axisName in axisNames)
				{
					// if (InputManager.inputter.GetAxis(axisName) == 0 && InputManager.inputter.GetAxisPrev(axisName) != 0)
					// {
					// 	Activate ();
					// 	break;
					// }
				}
			}
			else
			{
			}
			// previousAnyKey = Input.anyKey || InputManager.inputter.GetAnyButton();
		}

		public enum State
		{
			Down,
			Held,
			Up
		}

		void Activate ()
		{
			button.onClick.Invoke();
		}

		void OnDestroy ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
#endif
			GameManager.updatables = GameManager.updatables.Remove(this);
		}
	}
}