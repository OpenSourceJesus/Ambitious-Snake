using UnityEngine.UI;
using Extensions;

namespace AmbitiousSnake
{
	public class SnakeMeasurer : SingletonMonoBehaviour<SnakeMeasurer>, IUpdatable
	{
		public bool PauseWhileUnfocused
		{
			get
			{
				return true;
			}
		}
		public Text displayText;

		void OnEnable ()
		{
			GameManager.updatables = GameManager.updatables.Add(this);
		}

		void OnDisable ()
		{
			GameManager.updatables = GameManager.updatables.Remove(this);
		}
		
		public void DoUpdate ()
		{
			float numerator = Snake.instance.targetLength.GetValue() - Snake.instance.targetLength.min;
			float denominator = Snake.instance.targetLength.max - Snake.instance.targetLength.min;
			displayText.text = string.Format("Length: {0:F0}%", numerator / denominator * 100);
		}
	}
}