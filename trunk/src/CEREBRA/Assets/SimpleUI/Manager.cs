using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SimpleUI
{
	class Manager : MonoBehaviour
	{
		public GUISkin customSkin = null;
		private UI _currentUI = null;
		public UI Current { 
			get {
				return _currentUI;
			}
			set {
				_currentUI = value;
				_currentUI.Size = new Vector2(1, 1);
			}

		}

		void Start() { 
		}

		void OnGUI() {
			GUI.skin = customSkin;
			if (Current != null)
			{
				//Debug.Log("drawing current...");
				Current.OnGUI();
			}
			else {
				//Debug.Log("current is null");
			}
		}

		/*static Rect FromPositionSize(Vector2 pos, Vector2 size) 
		{
			return new Rect()
		}*/

	}
}
