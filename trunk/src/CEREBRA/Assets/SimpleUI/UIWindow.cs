using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SimpleUI
{
	class UIWindow : UIContainer
	{
		/*public delegate void ResizeHandler(IUIElement sender, object args);
		public event ResizeHandler onResize;*/

		public int ID;

		public string Title { get; set; }

		private static int IDCounter = 0;
		//private static Dictionary<int, List<IUIElement> > allWindows;

		public UIWindow()
		{
			positionGiven = false;
			ID = IDCounter;
			IDCounter++;
		}

		public void DrawWindowContent(int wid) {
			base.beforeGUI();
			foreach (IUIElement elem in Children)
			{
				//UnityEngine.Debug.Log("Drawing " + elem.ToString());
				elem.OnGUI();
			}
			GUI.DragWindow();
			base.afterGUI();
		}

		public override void OnGUI() {
			if (!sizeGiven)
			{
				throw new ArgumentNullException("this.Size");
			}

			if (!positionGiven)
			{
				Position = new Vector2((1.0f - Size.x) / 2.0f, (1.0f - Size.y) / 2.0f);
			}

			Rect new_rect=GUILayout.Window(ID,new Rect(Screen.width*Position.x,Screen.height*Position.y,Screen.width*Size.x,Screen.height*Size.y),DrawWindowContent,Title);
			Position = new Vector2(new_rect.x/Screen.width, new_rect.y/Screen.height);
		}
	}
}
