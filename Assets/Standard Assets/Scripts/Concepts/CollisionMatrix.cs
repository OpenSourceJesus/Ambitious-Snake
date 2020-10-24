using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class CollisionMatrix
{
	public List<LayerCollisionMask> layerCollisions;
	
	public LayerMask GetLayerCollisionMask (int layer)
	{
		foreach (LayerCollisionMask layerCollision in layerCollisions)
		{
			if (layerCollision.layer == layer)
				return layerCollision.collisionMask;
		}
		return LayerMask.GetMask();
	}
}
