using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SimpleUI
{
	class UILabel : UIElementBase
	{
		public string Text { get; set; }
		public string Tooltip { get; set; }
		public bool FillAllSpace;
		
		
		public UILabel(string text="",bool fillAllSpace=true) {
			Text = text;
			FillAllSpace = fillAllSpace;
			//UnityEngine.Debug.Log(Text+" from constructor");
		}

		public UILabel(string text, string tooltip, bool fillAllSpace = true)
		{
			Text = text;
			FillAllSpace = fillAllSpace;
			Tooltip = tooltip;
			//UnityEngine.Debug.Log(Text+" from constructor");
		}

		public override void OnGUI()
		{
			if (positionGiven && sizeGiven)
			{
				GUI.Label(new Rect(Screen.width * Position.x, Screen.height * Position.y, Screen.width * Size.x, Screen.height * Size.y), new GUIContent(Text, Tooltip));
			}
			else
			{
				if (!FillAllSpace)
				{
					GUIStyle labelStyle = "Label";
					Vector2 v2=labelStyle.CalcSize(new GUIContent(Text, Tooltip));
					//Debug.Log(v2);
					GUILayout.Label(new GUIContent(Text), GUILayout.MaxWidth(v2.x), GUILayout.MaxHeight(v2.y));
				}
				else
				{
					GUILayout.Label(new GUIContent(Text, Tooltip));
				}
			}
		}
	}
}
