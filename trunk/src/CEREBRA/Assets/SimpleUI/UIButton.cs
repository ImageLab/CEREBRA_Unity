using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleUI;
using UnityEngine;

namespace SimpleUI
{
    class UIButton : UIElementBase
    {
		public UIButton(string textValue = "", bool fillAllSpace = true)
		{
			Text = textValue;
			FillAllSpace = fillAllSpace;
		}

		public UIButton(GUIContent content, bool fillAllSpace = true)
		{
			Content = content;
			FillAllSpace = fillAllSpace;
		}

        public string text;
		public string Text { get { return text; } set { text = value; content = new GUIContent(value); } }

		private GUIContent content;
		public GUIContent Content { get { return content; } set { content = value; text = value.text; } }

		public bool FillAllSpace;
        public delegate void OnClickHandler(IUIElement sender, EventArgs e);
        public event OnClickHandler onClick;
        public override void OnGUI() {
			if (sizeGiven && positionGiven)
			{
				if (GUI.Button(new Rect(Screen.width * Position.x, Screen.height * Position.y, Screen.width * Size.x, Screen.height * Size.y), content)) 
				{
					if (onClick != null) 
					{
						onClick(this, null);
					}
				}
			}
			else
			{
				if (!FillAllSpace)
				{
					GUIStyle labelStyle = "Button";
					Vector2 v2 = labelStyle.CalcSize(content);
					if (GUILayout.Button(content, GUILayout.MaxWidth(v2.x), GUILayout.MaxHeight(v2.y)))
					{
						if (onClick != null)
						{
							onClick(this, null);
						}
					}
				}
				else
				{
					if (GUILayout.Button(content))
					{
						if (onClick != null)
						{
							onClick(this, null);
						}
					}
				}
			}
        }
    }
}


