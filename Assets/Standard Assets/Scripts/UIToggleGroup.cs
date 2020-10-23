using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIToggleGroup : UIToggle
{
	public ToggleGroupType type;
	public UIToggle[] subToggles;
	
	public override void Awake ()
	{
		foreach (UIToggle subToggle in subToggles)
			subToggle.toggleGroups.Add(this);
		toggle = GetComponent<Toggle>();
		OnUpdate ();
	}
	
	public void OnUpdate ()
	{
		bool turnOn = true;
		foreach (UIToggle subToggle in subToggles)
		{
			if (subToggle.toggle != null)
			{
				switch (type)
				{
					case ToggleGroupType.All:
						if (!subToggle.toggle.isOn)
							turnOn = false;
						break;
					case ToggleGroupType.None:
						if (subToggle.toggle.isOn)
							turnOn = false;
						break;
				}
			}
		}
		toggle.isOn = turnOn;
	}
	
	public override void OnToggle ()
	{
		base.OnToggle ();
		if (toggle.isOn)
		{
			switch (type)
			{
				case ToggleGroupType.All:
					foreach (UIToggle subToggle in subToggles)
						subToggle.toggle.isOn = true;
					break;
				case ToggleGroupType.None:
					foreach (UIToggle subToggle in subToggles)
						subToggle.toggle.isOn = false;
					break;
				case ToggleGroupType.Same:
					foreach (UIToggle subToggle in subToggles)
						subToggle.toggle.isOn = toggle.isOn;
					break;
				case ToggleGroupType.Opposite:
					foreach (UIToggle subToggle in subToggles)
						subToggle.toggle.isOn = !toggle.isOn;
					break;
			}
		}
		else
		{
			bool turnOn = true;
			foreach (UIToggle subToggle in subToggles)
			{
				switch (type)
				{
					case ToggleGroupType.All:
						if (!subToggle.toggle.isOn)
							turnOn = false;
						break;
					case ToggleGroupType.None:
						if (subToggle.toggle.isOn)
							turnOn = false;
						break;
				}
			}
			if (turnOn)
				toggle.isOn = turnOn;
		}
	}
}

public enum ToggleGroupType
{
	All = 0,
	None = 1,
	Same = 2,
	Opposite = 3
}
