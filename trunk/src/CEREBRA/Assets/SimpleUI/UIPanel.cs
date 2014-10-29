using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SimpleUI
{
	class UIPanel : UIContainer
	{

		public UIPanel(Vector2 pos, Vector2 size)
		{
			Position = pos;
			Size = size;
		}
		public override void OnGUI()
		{
			base.beforeGUI();
			Rect rect = new Rect(Screen.width * Position.x, Screen.height * Position.y, Screen.width * Size.x, Screen.height * Size.y);

			GUI.Box(rect, "");
			GUI.BeginGroup(rect);

			foreach(IUIElement elem in Children)
			{
				elem.OnGUI();
			}
			GUI.EndGroup();
			base.afterGUI();
		}
	}
}
