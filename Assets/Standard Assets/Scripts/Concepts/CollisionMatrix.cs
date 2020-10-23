using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CollisionMatrix : System.Object
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
