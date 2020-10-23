using UnityEngine.UI;

namespace uCP
{
	public class DropperGraphicRaycaster : GraphicRaycaster
	{
		#if !UNITY_4_6
		public int _priority = 1;
		public override int sortOrderPriority {
			get { return _priority; }
		}
		#endif
	}
}