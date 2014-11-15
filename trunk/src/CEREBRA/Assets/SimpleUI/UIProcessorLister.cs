using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SimpleUI
{
	class UIProcessorLister : UIWindow
	{
		public delegate void ProcessorCreateHandler(string selectedProcessor);
		public event ProcessorCreateHandler onProcessorCreate;
		public string[] processorsToList;
		private UIList processorList;
		private UIButton selectButton;
		private UIButton cancelButton;

		public UIProcessorLister(string message, string[] processorsToList, Vector2 size)
		{
			Title = "Select a processor";
			this.processorsToList = processorsToList;
			this.Size = size;

			processorList = new UIList(new Vector2(0.0f, 0.0f), new Vector2(1f, 0.8f));
			processorList.Data = processorsToList.ToList();
			processorList.onDoubleClick += processorList_onDoubleClick;

			UIHGroup hg = new UIHGroup();
			cancelButton = new UIButton("Cancel");
			cancelButton.onClick += cancelButton_onClick;

			selectButton = new UIButton("Select");
			selectButton.onClick += selectButton_onClick;

			hg.Add(cancelButton);
			hg.Add(selectButton);

			Add(new UILabel(message));
			Add(processorList);
			Add(hg);
		}

		public UIProcessorLister(string[] processorsToList, Vector2 size)
		{
			Title = "Select a processor";
			if (processorsToList == null)
			{
				processorsToList = libsimple.ProcessorManager.GetRegisteredProcessors();
			}

			this.processorsToList = processorsToList;
			this.Size = size;

			processorList = new UIList(new Vector2(0.05f * size.x, 0.1f * size.y), new Vector2(0.9f * size.x, 0.8f * size.y));
			processorList.Data = processorsToList.ToList();
			processorList.onDoubleClick += processorList_onDoubleClick;

			UIHGroup hg = new UIHGroup();
			cancelButton = new UIButton("Cancel");
			cancelButton.onClick += cancelButton_onClick;

			selectButton = new UIButton("Select");
			selectButton.onClick += selectButton_onClick;

			hg.Add(cancelButton);
			hg.Add(selectButton);

			Add(processorList);
			Add(hg);
		}

		void selectButton_onClick(IUIElement sender, EventArgs e)
		{
			if (onProcessorCreate != null)
				onProcessorCreate(processorList.SelectedItem);

			this.RemoveSelf();
		}

		void cancelButton_onClick(IUIElement sender, EventArgs e)
		{
			//if (onProcessorCreate != null)
			//	onProcessorCreate(null);

			this.RemoveSelf();
		}

		void processorList_onDoubleClick(IUIElement sender, object args)
		{
			if (onProcessorCreate != null)
				onProcessorCreate(processorList.SelectedItem);

			this.RemoveSelf();
		}
	}
}
