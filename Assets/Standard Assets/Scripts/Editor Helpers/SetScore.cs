using UnityEngine;
using AmbitiousSnake;

[ExecuteAlways]
public class SetScore : MonoBehaviour
{
	public uint score;
	
	public virtual void OnEnable ()
	{
		GameManager.Score = score;
	}
}
