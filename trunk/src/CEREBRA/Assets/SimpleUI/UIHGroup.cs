using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SimpleUI
{
	class UIHGroup : UIContainer
	{
		public UIHGroup()
		{
			;
		}

		public override void OnGUI()
		{
			base.beforeGUI();
			GUILayout.BeginHorizontal();
			foreach (IUIElement elem in Children)
			{
				elem.OnGUI();
			}
			GUILayout.EndHorizontal();
			base.afterGUI();
		}
	}
}
