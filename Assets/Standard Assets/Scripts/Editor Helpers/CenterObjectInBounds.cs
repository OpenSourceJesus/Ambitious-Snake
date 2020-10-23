using UnityEngine;
using System.Collections.Generic;

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
			if (this.useChildren)
				this.renderers = this.GetComponentsInChildren<Renderer>();
			Bounds[] boundsInstances = new Bounds[this.renderers.Length];
			if (!this.useChildren)
			{
				for (int i = 0; i < this.renderers.Length; i ++)
					boundsInstances[i] = this.renderers[i].bounds;
			}
			else
			{
				for (int i = 0; i < this.renderers.Length; i ++)
				{
					this.renderers[i].transform.SetParent(null);
					boundsInstances[i] = this.renderers[i].bounds;
				}
			}
			Bounds combinedBounds = BoundsExtensions.CombineBounds(boundsInstances);
			transform.position = combinedBounds.center;
			if (this.useChildren)
			{
				for (int i = 0; i < this.renderers.Length; i ++)
					this.renderers[i].transform.SetParent(transform);
			}
		}
	}
}