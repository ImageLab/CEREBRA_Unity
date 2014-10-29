using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleUI
{
	class UIContainer : UIElementBase
	{
		public List<IUIElement> Children;

		private bool drawing = false;
		private List<IUIElement> toBeRemoved;
		private List<IUIElement> toBeAdded;

		protected void beforeGUI()
		{
			drawing = true;
		}

		protected void afterGUI()
		{
			drawing = false;
			foreach (IUIElement elem in toBeRemoved)
			{
				Remove(elem);
			}
			toBeRemoved.Clear();

			foreach (IUIElement elem in toBeAdded)
			{
				Add(elem);
			}
			toBeAdded.Clear();
		}

		public UIContainer()
		{
			Children = new List<IUIElement>();
			toBeRemoved = new List<IUIElement>();
			toBeAdded = new List<IUIElement>();
		}

		public void Add(IUIElement elem)
		{
			if (drawing)
			{
				toBeAdded.Add(elem);
			}
			else
			{
				elem.Parent = this;
				Children.Add(elem);
			}
		}

		public void Remove(IUIElement elem)
		{
			if (drawing) 
			{
				//UnityEngine.Debug.Log("Adding to be deleted: " + elem.ToString());
				toBeRemoved.Add(elem);
			}
			else
			{
				//UnityEngine.Debug.Log("deleting: " + elem.ToString());
				elem.Parent = null;
				Children.Remove(elem);
			}
		}

		public void RemoveAll()
		{
			if (drawing)
			{
				toBeRemoved.AddRange(Children);
			}
			else
			{
				foreach (IUIElement elem in Children)
				{
					elem.Parent = null;
				}

				Children.Clear();
			}
		}

		public override void RemoveSelf()
		{
			RemoveAll();
			UIContainer cnt = (UIContainer)this.Parent;
			cnt.Remove(this);
		}
	}
}
