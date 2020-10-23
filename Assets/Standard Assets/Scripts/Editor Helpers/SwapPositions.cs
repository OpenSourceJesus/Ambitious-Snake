using UnityEngine;

[ExecuteAlways]
public class SwapPositions : MonoBehaviour
{
	void Start ()
    {
        Vector3 oldPos = transform.position;
        Vector3 newPos = new Vector3();
	    SwapPositions swapWith = null;
	    foreach (SwapPositions sp in FindObjectsOfType<SwapPositions>())
        {
            if (sp != this)
            {
	            swapWith = sp;
                newPos = sp.transform.position;
            }
        }
        transform.position = newPos;
        swapWith.transform.position = oldPos;
	    DestroyImmediate(swapWith);
        DestroyImmediate(this);
	}
}
