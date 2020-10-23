using UnityEngine;
using System.Collections;

public class SortRenderer : MonoBehaviour
{
	public new Renderer renderer;
	public bool useLayerName;
	public string sortingLayerName;
	public int sortingLayerId;
	[Range(-32768, -32767)]
	public int sortingOrder;
	
	public virtual void Start ()
	{
		if (useLayerName)
			renderer.sortingLayerName = sortingLayerName;
		else
			renderer.sortingLayerID = sortingLayerId;
		renderer.sortingOrder = sortingOrder;
	}
}
