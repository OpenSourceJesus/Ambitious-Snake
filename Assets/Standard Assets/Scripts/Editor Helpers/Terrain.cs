using UnityEngine;
using Ferr;
using Extensions;

[ExecuteAlways]
public class Terrain : MonoBehaviour
{
    public Ferr2DT_PathTerrain terrain;

    public virtual void Start ()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            if (terrain == null)
                terrain = GetComponent<Ferr2DT_PathTerrain>();
            return;
        }
#endif
    }

	public virtual void Update ()
    {
        for (int i = 0; i < terrain.pathData.Count; i ++)
            terrain.pathData[i] = terrain.pathData[i].Snap(Vector2.one * 25);
	}
}
