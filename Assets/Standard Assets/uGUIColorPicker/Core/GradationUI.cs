using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;


namespace uCP
{
	/// <summary>Gradation UI.</summary>
	[AddComponentMenu("uCP/Gradation UI")]
	public partial class GradationUI : BaseMeshEffect
	{
		/// <summary>If using alpha channel. "true"</summary>
		public bool useAlpha;

		/// <summary>Colors for vertices.</summary>
		public Color[] colors = new Color[4]{Color.black,Color.white,Color.red,Color.black};

		/// <summary>Paint colors on vertices.</summary>
		public override void ModifyMesh(VertexHelper help)
		{
			if (!IsActive() || help == null)
				return;

			var rect = graphic.GetPixelAdjustedRect();
			var v = UIVertex.simpleVert;
			for(int i=0 ; i<help.currentVertCount ; i++)
			{
				help.PopulateUIVertex (ref v,i);

				var NPosition = Rect.PointToNormalized(rect,v.position);
				
				v.color = (Color32)Color.Lerp(Color.Lerp(colors[0],colors[1],NPosition.y),Color.Lerp(colors[3],colors[2],NPosition.y),NPosition.x);
				
				if(!useAlpha)
					v.color.a = byte.MaxValue;
				
				help.SetUIVertex(v,i);
			}
		}

		/// <summary>Update colors.</summary>
	    public virtual void UpdateColors ()
	    {
			graphic.SetVerticesDirty ();
	    }
	}
}