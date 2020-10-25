using UnityEngine.UI;

public class _ToggleGroup : _Toggle
{
	public ToggleGroupType type;
	public _Toggle[] subToggles;
	
	public override void Awake ()
	{
		foreach (_Toggle subToggle in subToggles)
			subToggle.toggleGroups.Add(this);
		toggle = GetComponent<Toggle>();
		OnUpdate ();
	}
	
	public void OnUpdate ()
	{
		bool turnOn = true;
		foreach (_Toggle subToggle in subToggles)
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
					foreach (_Toggle subToggle in subToggles)
						subToggle.toggle.isOn = true;
					break;
				case ToggleGroupType.None:
					foreach (_Toggle subToggle in subToggles)
						subToggle.toggle.isOn = false;
					break;
				case ToggleGroupType.Same:
					foreach (_Toggle subToggle in subToggles)
						subToggle.toggle.isOn = toggle.isOn;
					break;
				case ToggleGroupType.Opposite:
					foreach (_Toggle subToggle in subToggles)
						subToggle.toggle.isOn = !toggle.isOn;
					break;
			}
		}
		else
		{
			bool turnOn = true;
			foreach (_Toggle subToggle in subToggles)
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
