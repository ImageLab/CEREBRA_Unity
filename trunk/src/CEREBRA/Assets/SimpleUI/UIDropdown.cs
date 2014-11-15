using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SimpleUI
{
	class UIDropdown : UIElementBase
	{
		private bool showingList;

		public List<string> Data { get; set; }
		public int SelectedIndex { get; set; }
		public string SelectedItem { get { return (SelectedIndex>=0 ? Data[SelectedIndex] : null); } }

		public UIDropdown(Vector2 position, Vector2 size)
		{
			SelectedIndex = -1;
			Position = position;
			Size = size;
		}

		public override void OnGUI()
		{
			string showingItem = ((Data != null && Data.Count > 0 && SelectedIndex>=0) ? SelectedItem : "(choose one)");

			Rect textRect=new Rect(Screen.width * Position.x, Screen.height * Position.y, Screen.width * Size.x-Screen.height * Size.y, Screen.height * Size.y);
			Rect buttonRect=new Rect(Screen.width * Position.x+Screen.width*Size.x-Screen.height * Size.y, Screen.height * Position.y, Screen.height * Size.y, Screen.height * Size.y);
			Rect dropdownRect = new Rect(Screen.width * Position.x, Screen.height * Position.y + Screen.height * Size.y, Screen.width * Size.x, Screen.height * Size.y * ((Data != null && Data.Count>0) ? Data.Count : 1));
			

			GUI.TextField(textRect, showingItem);
			if (GUI.Button(buttonRect, new GUIContent(showingList ? "\u25B2" : "\u25BC")))
			{
				showingList = !showingList;
			}

			if (showingList)
			{
				if(Event.current.type == EventType.MouseDown && !dropdownRect.Contains(Event.current.mousePosition))
				{
					showingList = false;
					Event.current.Use();
					return;
				}
				GUI.Box(dropdownRect, "");
				GUILayout.BeginArea(dropdownRect);
				//GUI.BeginGroup(dropdownRect);
				if (Data != null && Data.Count > 0)
				{
					for (int i = 0; i < Data.Count; i++)
					{
						if (GUILayout.Button(Data[i]))
						{
							SelectedIndex = i;
							showingList = false;
						}
					}
				}
				//GUI.EndGroup();
				GUILayout.EndArea();
			}
		}
	}
}
