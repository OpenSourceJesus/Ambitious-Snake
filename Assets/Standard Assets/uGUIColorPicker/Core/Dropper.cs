using UnityEngine;
using UnityEngine.UI;
using System;


namespace uCP
{
	[RequireComponent(typeof(Button))]
	public class Dropper : _Selectable
	{
		/// <summary>Dropper canvas prefab.</summary>
		public GameObject dropperCanvas;

		/// <summary>The color when active.</summary>
		public Color ActiveColor = new Color(96f,96f,96f);
		[NonSerialized] public Color NormalColor;

		/// <summary>This object's button.</summary>
		[NonSerialized] public Button DropperButton;
		
		/// <summary>This object's button.</summary>
		[NonSerialized] public Graphic graph;
		
		/// <summary>Parent color picker object.</summary>
		[NonSerialized] public ColorPicker _colorPicker; // Parent. 
		public ColorPicker colorPicker{
			get{
				if(_colorPicker == null)
					_colorPicker = GetComponentInParent<ColorPicker> ();
				return _colorPicker;
			}
		}

		protected virtual void Awake()
		{
			graph = GetComponent<Graphic>();
			NormalColor = graph.color;

			DropperButton = GetComponent<Button>();
			DropperButton.onClick.AddListener(OnDropperButton);
		}
		#region events
		/// <summary>Dropper's button event.</summary>
		public virtual void OnDropperButton()
		{
			graph.color = ActiveColor;

			var obj = Instantiate(dropperCanvas) as GameObject;
			var DGetColor = obj.GetComponent<DropperGetColor>();
			Debug.Log(DGetColor);
			DGetColor.OnGetColor.AddListener(OnGetColorCallback);
		}

		/// <summary>Dropper canvas event callback.</summary>
		public virtual void OnGetColorCallback(Color c)
		{
			graph.color = NormalColor;

			colorPicker.color = c;
			colorPicker.UpdateUI();
		}
		#endregion
	}
}
