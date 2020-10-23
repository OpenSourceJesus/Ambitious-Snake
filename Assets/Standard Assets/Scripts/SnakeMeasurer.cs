using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace AmbitiousSnake
{
	public class SnakeMeasurer : MonoBehaviour
	{
		public static SnakeMeasurer instance;
		Text displayText;
		
		void Start ()
		{
			instance = this;
			displayText = GetComponent<Text>();
		}
		
		void Update ()
		{
			float numerator = GameManager.GetSingleton<Snake>().targetLength.GetValue() - GameManager.GetSingleton<Snake>().targetLength.min;
			float denominator = GameManager.GetSingleton<Snake>().targetLength.max - GameManager.GetSingleton<Snake>().targetLength.min;
			displayText.text = string.Format("Length: {0:F0}%", numerator / denominator * 100);
		}
	}
}