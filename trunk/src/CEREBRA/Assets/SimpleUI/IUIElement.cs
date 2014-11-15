using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SimpleUI
{
	public interface IUIElement
	{
		IUIElement Parent { get; set; }
		string Name {get; set;}
		Vector2 Position { get; set; }
		Vector2 Size { get; set; }
		void OnGUI();
	}
}
