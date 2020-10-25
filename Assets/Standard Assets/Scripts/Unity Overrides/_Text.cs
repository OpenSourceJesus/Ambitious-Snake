using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class UIText : MonoBehaviour
{
	Text text;
	
	void Start ()
	{
		text = GetComponent<Text>();
	}
	
	public void Set (float value)
	{
		Set ("" + value);
	}
	
	public void Set (int value)
	{
		Set ("" + value);
	}
	
	public void Set (string value)
	{
		text.text = value;
	}
}
