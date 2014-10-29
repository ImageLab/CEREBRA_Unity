using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SimpleUI
{
	class UITextbox: UIElementBase
	{
		private string text;
		public string Text { get { return text; } set { text = value; } }

		public delegate void TextChangeHandler(IUIElement sender, object args);
		public event TextChangeHandler onTextChange;
		public bool FillAllSpace;

		public UITextbox(string text = "", bool fillAllSpace = true)
		{
			Text = text;
			FillAllSpace = fillAllSpace;
		}

		public override void OnGUI()
		{
			string newText;

			if (sizeGiven && positionGiven)
			{
				newText = GUI.TextField(new Rect(Screen.width * Position.x, Screen.height * Position.y, Screen.width * Size.x, Screen.height * Size.y), text);
				if(newText!=text)
				{
					if (onTextChange != null)
					{
						onTextChange(this, newText);
					}
					text = newText;
				}
			}
			else
			{
				if (!FillAllSpace)
				{
					GUIStyle labelStyle = "Textbox";
					Vector2 v2 = labelStyle.CalcSize(new GUIContent(text));
					newText = GUILayout.TextField(text, GUILayout.MaxWidth(v2.x), GUILayout.MaxHeight(v2.y));
					if (newText != text)
					{
						if (onTextChange != null)
						{
							onTextChange(this, newText);
						}
						text = newText;
					}
				}
				else
				{
					newText = GUILayout.TextField(text);
					if (newText != text)
					{
						if (onTextChange != null)
						{
							onTextChange(this, newText);
						}
						text = newText;
					}
				}
			}
		}
	}
}
