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
			float numerator = Snake.instance.targetLength.GetValue() - Snake.instance.targetLength.min;
			float denominator = Snake.instance.targetLength.max - Snake.instance.targetLength.min;
			displayText.text = string.Format("Length: {0:F0}%", numerator / denominator * 100);
		}
	}
}