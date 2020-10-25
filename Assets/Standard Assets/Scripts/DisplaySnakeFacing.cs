using UnityEngine;
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
		public Transform trs;

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
			trs.position = Snake.instance.GetHeadPosition();
			trs.right = Snake.instance.facingVector;
		}
	}
}