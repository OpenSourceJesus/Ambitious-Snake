#if UNITY_EDITOR
using UnityEngine;

[ExecuteInEditMode]
public class SetRandomUniqueId : MonoBehaviour
{
	void Awake ()
	{
		GetComponent<IIdentifiable>().UniqueId = Random.Range(int.MinValue, int.MaxValue);
		DestroyImmediate(this);
	}
}
#endif