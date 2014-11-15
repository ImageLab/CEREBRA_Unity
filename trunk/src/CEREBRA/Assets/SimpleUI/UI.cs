using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SimpleUI
{
	class UI : UIContainer // shouldn't we inherit this from UIContainer?
	{
		public UI() {
		
		}

		public override void OnGUI() {
			base.beforeGUI();
			foreach (IUIElement elem in Children)
			{
				//Debug.Log(elem.Name);
				elem.OnGUI();
			}
			base.afterGUI();
		}
	}
}
