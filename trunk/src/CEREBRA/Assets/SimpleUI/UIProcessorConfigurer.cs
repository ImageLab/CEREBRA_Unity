using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SimpleUI
{
	class UIProcessorConfigurer : UIWindow
	{
		public delegate void ProcessorConfiguredHandler(libsimple.IProcessor proc, string[] args);
		public event ProcessorConfiguredHandler onProcessorConfigured;

		private libsimple.IProcessor processor;
		private UIButton cancelButton;
		private UIButton confirmButton;
		private UITextbox[] parameters;

		void buildGUI(string[] prevConfig)
		{
			string[,] args = processor.GetArgs();

			if (prevConfig == null)
			{
				prevConfig = new string[args.GetLength(0)];
				for (int i = 0; i < prevConfig.Length; i++)
				{
					prevConfig[i] = "";
				}
			}

			parameters = new UITextbox[args.GetLength(0)];

			for (int i = 0; i < prevConfig.Length; i++)
			{
				UIHGroup hg = new UIHGroup();
				hg.Add(new UILabel(args[i, 0], args[i, 1]));
				parameters[i] = new UITextbox(prevConfig[i]);
				hg.Add(parameters[i]);
				Add(hg);
			}

			UIHGroup horg = new UIHGroup();
			horg.Position = new Vector2(0.0f, 0.9f);
			horg.Size = new Vector2(1.0f, 0.1f);
			
			cancelButton = new UIButton("Cancel");
			cancelButton.onClick += cancelButton_onClick;
			cancelButton.Position = new Vector2(0.05f * Size.x, 0.85f * Size.y);
			cancelButton.Size = new Vector2(0.4f * Size.x, 0.1f * Size.y);
			
			confirmButton = new UIButton("Confirm");
			confirmButton.onClick += confirmButton_onClick;
			confirmButton.Size = new Vector2(0.4f * Size.x, 0.1f * Size.y);
			confirmButton.Position = new Vector2(0.55f * Size.x, 0.85f * Size.y);

			Add(cancelButton);
			Add(confirmButton);

			Add(horg);
		}

		void confirmButton_onClick(IUIElement sender, EventArgs e)
		{
			if (onProcessorConfigured != null)
			{
				string[] args= new string[parameters.Length];

				for (int i = 0; i < parameters.Length; i++)
				{
					args[i] = parameters[i].Text;
				}

				onProcessorConfigured(processor, args);
			}
			this.RemoveSelf();
		}

		void cancelButton_onClick(IUIElement sender, EventArgs e)
		{
			this.RemoveSelf();
		}

		public UIProcessorConfigurer(libsimple.IProcessor proc,Vector2 size)
		{
			Title = "Configure " + proc.GetProcessorName();
			processor = proc;
			this.Size = size;
			buildGUI(null);
			this.Size = size;
		}

		public UIProcessorConfigurer(libsimple.IProcessor proc, string[] prevConfig, Vector2 size)
		{
			Title = "Configure " + proc.GetProcessorName();
			processor = proc;
			this.Size = size;
			buildGUI(prevConfig);
			this.Size = size;
		}
	}
}
