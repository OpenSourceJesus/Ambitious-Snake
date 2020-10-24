using UnityEngine;

namespace Extensions
{
	[ExecuteAlways]
	public class CenterObjectInBounds : MonoBehaviour
	{
		public bool update;
		public bool useChildren;
		public Renderer[] renderers;
		
		void Update ()
		{
			if (!update)
				return;
			update = false;
			if (useChildren)
				renderers = GetComponentsInChildren<Renderer>();
			Bounds[] boundsInstances = new Bounds[renderers.Length];
			if (!useChildren)
			{
				for (int i = 0; i < renderers.Length; i ++)
					boundsInstances[i] = renderers[i].bounds;
			}
			else
			{
				for (int i = 0; i < renderers.Length; i ++)
				{
					renderers[i].transform.SetParent(null);
					boundsInstances[i] = renderers[i].bounds;
				}
			}
			Bounds combinedBounds = BoundsExtensions.Combine(boundsInstances);
			transform.position = combinedBounds.center;
			if (useChildren)
			{
				for (int i = 0; i < renderers.Length; i ++)
					renderers[i].transform.SetParent(transform);
			}
		}
	}
}