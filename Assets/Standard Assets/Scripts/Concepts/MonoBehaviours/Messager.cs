using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Messager : MonoBehaviour
{
	public static Messager instance;
	public Color defaultColor;
	public float defaultTimePerChar;
	Text text;
	
	void Start ()
	{
		instance = this;
		this.text = GetComponent<Text>();
		this.text.enabled = false;
	}
	
	public void Message (string text, Color color, float timePerChar)
	{
		this.text.text = text;
		this.text.color = color;
		StartCoroutine(Display (text.Length * timePerChar));
	}
	
	public void Message (string text, Color color)
	{
		Message (text, color, this.defaultTimePerChar);
	}
	
	public void Message (string text, float timePerChar)
	{
		Message (text, this.defaultColor, timePerChar);
	}
	
	public void Message (string text)
	{
		Message (text, this.defaultColor, this.defaultTimePerChar);
	}
	
	IEnumerator Display (float time)
	{
		this.text.enabled = true;
		yield return new WaitForSeconds(time);
		this.text.enabled = false;
	}
}
