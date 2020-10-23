using Extensions;

namespace AmbitiousSnake
{
	public class DisplaySnakeFacing : NotPartOfLevelEditor, IUpdatable
	{
		public bool PauseWhileUnfocused
		{
			get
			{
				return true;
			}
		}

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
			transform.position = GetComponentInParent<Snake>().GetHeadPosition();
			transform.right = GetComponentInParent<Snake>().facingVector;
		}
	}
}