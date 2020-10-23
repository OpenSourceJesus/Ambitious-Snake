using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AmbitiousSnake
{
	[ExecuteInEditMode]
	public class MouseAndGamepadSwitch : MonoBehaviour
	{
		public GameObject[] toggleGos = new GameObject[0];
		bool wasMouse = true;
		static bool previousUsingGamepad;
		
		void OnEnable ()
		{
			if (toggleGos.Length == 0)
			{
				Transform trs = GetComponent<Transform>();
				List<GameObject> children = new List<GameObject>();
				for (int i = 0; i < trs.childCount; i ++)
					children.Add(trs.GetChild(i).gameObject);
				toggleGos = children.ToArray();
			}
			previousUsingGamepad = InputManager.UsingGamepad;
			InputSystem.onDeviceChange += OnDeviceChanged;
		}
		
		void OnDisable ()
		{
			InputSystem.onDeviceChange -= OnDeviceChanged;
		}
		
		void ToggleGos ()
		{
			foreach (GameObject go in toggleGos)
				go.SetActive(!go.activeSelf);
		}

		void OnDeviceChanged (InputDevice device, InputDeviceChange change)
		{
			bool usingGamepad = InputManager.UsingGamepad;
			if (usingGamepad != previousUsingGamepad)
			{
				ToggleGos ();
				previousUsingGamepad = usingGamepad;
			}
		}
	}
}