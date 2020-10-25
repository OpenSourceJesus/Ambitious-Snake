using UnityEngine;
// using AmbitiousSnake;
// using Extensions;

public class Laser : MonoBehaviour, IUpdatable
{
	public bool PauseWhileUnfocused
	{
		get
		{
			return true;
		}
	}
	public LineRenderer line;
	public LayerMask whatBlocksMe;
	public Transform trs;
	public RaycastHit2D hitBlocker;
	const int maxLength = int.MaxValue;

	// void OnEnable ()
	// {
	// 	GameManager.updatables = GameManager.updatables.Add(this);
	// }
	
	public void DoUpdate ()
	{
		hitBlocker = Physics2D.Raycast(trs.position, trs.up, maxLength, whatBlocksMe);
		if (hitBlocker.collider != null)
			line.SetPosition(1, trs.worldToLocalMatrix.MultiplyPoint(hitBlocker.point));
		else
			line.SetPosition(1, trs.up * maxLength);
	}

	// void OnDisable ()
	// {
	// 	GameManager.updatables = GameManager.updatables.Remove(this);
	// }
}
