using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleUI
{
	class UIMessageBox : UIWindow
	{
		public UIMessageBox(string message)
		{
			this.Size = new UnityEngine.Vector2(0.4f, 0.3f);
			Add(new UILabel(message));
			UIButton okButton = new UIButton("OK");
			okButton.Position = new UnityEngine.Vector2(0.1f * Size.x, 0.8f * Size.y);
			okButton.Size = new UnityEngine.Vector2(0.8f * Size.x, 0.15f * Size.y);
			okButton.onClick += okButton_onClick;
			Add(okButton);
		}

		public UIMessageBox(string title, string message)
		{
			this.Size = new UnityEngine.Vector2(0.4f, 0.3f);
			this.Title = title;
			Add(new UILabel(message));
			UIButton okButton = new UIButton("OK");
			okButton.onClick += okButton_onClick;
			Add(okButton);
		}

		void okButton_onClick(IUIElement sender, EventArgs e)
		{
			RemoveSelf();
		}
	}
}
