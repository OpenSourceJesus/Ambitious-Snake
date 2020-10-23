using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class UIToggle : _Selectable
{
	[HideInInspector]
	public Toggle toggle;
	[HideInInspector]
	public List<UIToggleGroup> toggleGroups;
	
	public virtual void Awake ()
	{
		toggle = GetComponent<Toggle>();
		toggle.onValueChanged.Invoke (toggle.isOn);
	}
	
	public virtual void OnToggle ()
	{
		foreach (UIToggleGroup toggleGroup in toggleGroups)
			toggleGroup.OnUpdate ();
	}
	
	public void SetIsOn (bool isOn)
	{
		this.toggle.isOn = isOn;
	}
}
