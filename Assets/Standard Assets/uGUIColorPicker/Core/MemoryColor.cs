using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;


namespace uCP
{
	public class MemoryColor : MonoBehaviour
	{
		/// <summary>It's for memory objects.</summary>
		public GameObject buttonPrefab;

		/// <summary>Capacity of Memory.</summary>
		public int MaxMemory = 18;

		/// <summary>Color button objects.</summary>
		[NonSerialized] public List<GradationUI> Memories = new List<GradationUI>();
		
		/// <summary>Currently selected memory.</summary>
		[NonSerialized] public GradationUI currentMemory = null;
		
		/// <summary>Parent color picker object.</summary>
		[NonSerialized] public ColorPicker _colorPicker; // Parent. 
		public ColorPicker colorPicker{
			get{
				if(_colorPicker == null)
					_colorPicker = GetComponentInParent<ColorPicker> ();
				return _colorPicker;
			}
		}

		/// <summary>ColorButton function, use for setting the colorpicker's color.</summary>
		public virtual void ColorButton(GradationUI memory)
		{
			// change the currentButton.
			currentMemory = memory;

			// Update the colorpicker.
			colorPicker.color = memory.colors[1];
			colorPicker.UpdateUI();
		}

		/// <summary>Memorize the current color and create a button.</summary>
		public virtual void AddButton()
		{
			// Create a ColorButton.
			CreateNewButton(colorPicker.color);
		}

		/// <summary>Delete the current ColorButton or the last ColorButton.</summary>
		public virtual void DeleteButton()
		{
			// No buttons.
			if(Memories.Count == 0)
				return;

			// Get index of current.
			var index = Memories.IndexOf(currentMemory);

			if(index>=0)
			{
				// Destroy the current memory.
				Destroy(Memories[index].gameObject);
				Memories.RemoveAt(index);

				// Select next memory.
				if(index<Memories.Count && Memories[index]!=null)
					currentMemory = Memories[index];
			}
			else
			{
				// Destroy the last memory.
				var last = Memories.Count-1;
				Destroy(Memories[last].gameObject);
				Memories.RemoveAt(last);
			}
		}
		
		/// <summary>Create a memory with a color.</summary>
		public virtual void CreateNewButton(Color color)
		{
			// Limitation.
			if(Memories.Count>=MaxMemory)
				return;

			// Instantiate a button.
			var obj = Instantiate(buttonPrefab) as GameObject;
			obj.transform.SetParent(transform);

			// Sibling control.
			obj.transform.SetSiblingIndex(Memories.Count);

			// Add the new memory object.
			var grad = obj.GetComponent<GradationUI>();
			Memories.Add (grad);

			// Set color.
			grad.colors[0] = new Color(color.r,color.g,color.b); // SW
			grad.colors[1] = grad.colors[3] =  color; // NW & SE
			grad.colors[2] = new Color(color.r,color.g,color.b,Mathf.Pow(color.a,2)); // NE

			// Register onClick event.
			var button = obj.GetComponent<Button>();
			button.onClick.AddListener(()=>{
				ColorButton(grad);
			});
			
			// Focus the new memory object.
			currentMemory = grad;
		}

		/// <summary>Create memories with colors.</summary>
		public virtual void SetColors(Color[] colors)
		{
			foreach (var c in colors)
				CreateNewButton(c);
		}

		/// <summary>Get the colors.</summary>
		public virtual Color[] GetColors()
		{
			//var list = Memories.ConvertAll(x=>x.colors[1]);
			//return list.ToArray();
			
			return null;
		}

		/// <summary>Clear all memories.</summary>
		public virtual void Clear()
		{
			while(Memories.Count>0)
				DeleteButton();
		}
	}
}