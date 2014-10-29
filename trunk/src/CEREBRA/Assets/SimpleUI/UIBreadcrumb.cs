using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SimpleUI
{
	class UIBreadcrumb : UIElementBase
	{
		private string _divider="";
		public delegate void LevelChangedHandler(IUIElement sender, object args);
		public event LevelChangedHandler onLevelChange;
		public string Divider { get { return _divider; } set { _divider = value; dirty = true; } }
		private UIHGroup hgroup;
		private UIButton[] buttons;
		private bool dirty=true;

		private List<string> _data;

		public List<string> Data { get { return _data; } set { _data = value; dirty = true; } }

		private void updateButtons() {
			if (hgroup == null)
			{
				hgroup = new UIHGroup();
			}
			else
			{
				hgroup.RemoveAll();
			}
			if(Data!=null) {
				for (int i = 0; i < Data.Count; i++)
				{
					int button_index = i+1; // note: we have to define a local variable. 
											// if we don't, lambda expression at line 40 does not work. (i -> Data.Count)
					if (i != 0 && _divider!="")
					{
						hgroup.Add(new UILabel(_divider,false));
					}
					UIButton btn = new UIButton(Data[i], false);
					btn.onClick += ((s, e) => { this.SetLevel(button_index); if (onLevelChange != null) onLevelChange(this, null); });
					hgroup.Add(btn);
				}
			}
			dirty = false;
		}

		public void AddLevel(string item)
		{
			if (Data == null)
			{
				Data = new List<string>();
			}
			Data.Add(item);
			dirty = true;
		}

		public int GetLevel()
		{
			return (Data != null ? Data.Count : 0);
		}

		public void SetLevel(int level)
		{
			if (Data != null)
			{
				Data = Data.Take(level).ToList();
			}
			dirty = true;
		}

		public override void OnGUI()
		{
			if (dirty)
				updateButtons();
			
			hgroup.OnGUI();
			
			if (dirty)
				updateButtons();
		}
	}
}
