using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SimpleUI
{
    class UIList : UIElementBase
    {
        Vector2 scrollPosition = new Vector2(0, 0);
		public List<string> Data { get; set; }
		public int SelectedIndex { get; set; }
		public string SelectedItem { get { return (SelectedIndex >= 0 ? Data[SelectedIndex] : null); } }

		public delegate void DoubleClickHandler(IUIElement sender, object args);
		public event DoubleClickHandler onDoubleClick;

		public delegate void SelectionChangeHandler(IUIElement sender, object args);
		public event SelectionChangeHandler onSelectionChange;

        public UIList(Vector2 pos, Vector2 size)
        {
            Position = pos;
            Size = size;
        }


        public override void OnGUI()
        {
            Rect rect = new Rect(Screen.width * Position.x, Screen.height * Position.y, Screen.width * Size.x, Screen.height * Size.y);

			GUIStyle itemStyle = "Label";
			Rect lastRect=new Rect();

            GUI.Box(rect, "");

            GUILayout.BeginArea(rect);
            
            scrollPosition = GUILayout.BeginScrollView(scrollPosition); // burayı nepcaz ki :/
			if (Data != null)
			{
				for (int i = 0; i < Data.Count; i++)
				{
					string item = Data[i];
					Rect thisItemRect = GUILayoutUtility.GetRect(new GUIContent(item), itemStyle);
					bool hover = thisItemRect.Contains(Event.current.mousePosition);
					if (Event.current.type == EventType.MouseDown)
					{
						if (hover)
						{
							if (Event.current.clickCount == 1)
							{
								if (SelectedIndex != i && onSelectionChange != null)
								{
									SelectedIndex = i;
									onSelectionChange(this, null);
								}
								SelectedIndex = i;
								Event.current.Use();
							}
							else if (Event.current.clickCount == 2)
							{
								SelectedIndex = i;
								Event.current.Use();
								if (onDoubleClick != null)
								{
									onDoubleClick(this, null);
								}
							}
						}
					}
					else if (Event.current.type == EventType.Repaint)
					{
						if (hover || i == SelectedIndex)
						{
							GUI.Box(thisItemRect, "");
						}
						GUI.Label(thisItemRect, item);
						//Debug.Log(thisItemRect);
						lastRect = thisItemRect;
					}
				}
			}
			
            GUILayout.EndScrollView();            
			GUILayout.EndArea();

			if (rect.yMax - lastRect.yMax > 0)
			{
				GUILayout.Space(rect.yMax - lastRect.yMax);
			}
			else
			{
				GUILayout.Space(0);
			}
        }
    }
}
