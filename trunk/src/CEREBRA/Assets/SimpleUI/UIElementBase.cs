using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SimpleUI
{
	/// <summary>
	/// This class implements all the boilerplate code for IUIElement interface.
	/// Instead of impementing IUIElement, you should just extend this class.
	/// </summary>
	public class UIElementBase : IUIElement
	{
		public IUIElement Parent { get; set; }
		public virtual void OnGUI() { Debug.Log("override etmeyi unuttun panpaaa!!<3"); }
		public string Name { get; set; }

		public virtual void RemoveSelf() {
			UIContainer cnt = (UIContainer)this.Parent;
			cnt.Remove(this);
		}

		private Vector2 _position;
		public bool positionGiven = false;
		public Vector2 Position
		{
			get
			{
				return _position;
			}
			set
			{
				_position = value;
				positionGiven = true;
			}
		}

		private Vector2 _size;
		public bool sizeGiven = false;
		public Vector2 Size
		{
			get
			{
				return _size;
			}
			set
			{
				_size = value;
				sizeGiven = true;
			}
		}
	}
}
