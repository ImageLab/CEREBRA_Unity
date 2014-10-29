using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SimpleUI
{
	class UIFileBrowser : UIWindow
	{
		public delegate void FileSelectedHandler(IUIElement sender, object args);
		public event FileSelectedHandler onFileSelect;


		private UIButton cancelButton;
		private UIButton selectButton;
		private UIBreadcrumb directorySelector;
		private UIList fileListSelect;

		private string currentDirectory;

		public UIFileBrowser(Vector2 size, string directory = null)
		{
			Size = size;
			if (directory == null)
			{
				directory = System.IO.Directory.GetCurrentDirectory();
			}

			currentDirectory = directory;

			this.Title = "Select a file or folder";

			directorySelector = new UIBreadcrumb();
			directorySelector.Divider = ">";
			directorySelector.onLevelChange += directorySelector_onLevelChange;

			fileListSelect = new UIList(new Vector2(0.05f * size.x, 0.2f * size.y), new Vector2(0.9f * size.x, 0.8f * size.y));
			fileListSelect.onDoubleClick += fileListSelect_onDoubleClick;

			cancelButton = new UIButton("Cancel");
			cancelButton.onClick += cancelButton_onClick;
			
			selectButton = new UIButton("Select");
			selectButton.onClick += selectButton_onClick;

			rebuildBreadcrumb();
			rebuildFileList();

			UIHGroup hg = new UIHGroup();

			hg.Add(cancelButton);
			hg.Add(selectButton);

			Add(directorySelector);
			Add(fileListSelect);
			Add(hg);
			
		}

		void directorySelector_onLevelChange(IUIElement sender, object args)
		{
			currentDirectory = String.Join("\\", directorySelector.Data.ToArray());
			if (directorySelector.Data.Count == 1)
				currentDirectory += "\\";
			rebuildFileList();
		}

		void fileListSelect_onDoubleClick(IUIElement sender, object args)
		{
			string sel=fileListSelect.SelectedItem;
			if (sel.StartsWith("/"))
			{
				currentDirectory = System.IO.Path.Combine(currentDirectory, sel.Substring(1));
				Debug.Log(currentDirectory);
				rebuildBreadcrumb();
				rebuildFileList();
			}
		}

		void selectButton_onClick(IUIElement sender, EventArgs e)
		{
			string filename = fileListSelect.SelectedItem;

			if (filename.StartsWith("/"))
			{
				filename = filename.Substring(1);
			}

			if (onFileSelect != null)
			{
				onFileSelect(this, System.IO.Path.Combine(currentDirectory,filename)); // here be filename
			}
			this.RemoveSelf();
		}

		void cancelButton_onClick(IUIElement sender, EventArgs e)
		{
			if (onFileSelect != null)
			{
				onFileSelect(this, null);
			}
			this.RemoveSelf();
		}

		private void rebuildFileList() {
			fileListSelect.Data = new List<string>();
			string[] files = System.IO.Directory.GetDirectories(currentDirectory);
			foreach (string dir in files)
			{
				fileListSelect.Data.Add("/" + System.IO.Path.GetFileName(dir));
			}

			files = System.IO.Directory.GetFiles(currentDirectory);
			foreach (string file in files)
			{
				fileListSelect.Data.Add(System.IO.Path.GetFileName(file));
			}
		}

		private void rebuildBreadcrumb()
		{
			directorySelector.Data = currentDirectory.Split(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar).ToList();
		}
	}
}
