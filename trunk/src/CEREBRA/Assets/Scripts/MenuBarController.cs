using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuBarController : MonoBehaviour 
{
	private bool showLoadFileDialog = false;
	private string voxel_position_filepath = "No file selected.",
		voxel_intensity_filepath = "No file selected.",
		p_values_filepath = "No file selected.",
		terminal_voxels_filepath = "No file selected.",
		arclengths_filepath = "No file selected.";

	public GUISkin customSkin = null;
	public Texture ScaleTexture;

	protected string m_textPath;
	
	protected FileBrowser m_fileBrowser;
	
	[SerializeField]
	protected Texture2D	m_directoryImage, m_fileImage;

    public static double screenHeight = Screen.currentResolution.height;
    public static double screenWidth = Screen.currentResolution.width;

	// Update is called once per frame
	void Update () {
		switch(MenuBar.menuItemSelected)
		{
			case MenuItemSelections.LOAD_FILE:
				m_fileBrowser = new FileBrowser(
						new Rect(100, 80, 600, 500),
						"Select File",
						directorySelected
						);
				m_fileBrowser.BrowserType = FileBrowserType.Directory;
				m_fileBrowser.DirectoryImage = m_directoryImage;
				m_fileBrowser.FileImage = m_fileImage;
				showLoadFileDialog = false;
				//showLoadFileDialog = true;
				break;
			case MenuItemSelections.OPEN_RECENT:
				break;
			case MenuItemSelections.SAVE_IMAGE:
				break;
			case MenuItemSelections.SAVE_MOVIE:
				break;
			case MenuItemSelections.EXIT:
				break;
			case MenuItemSelections.OPTIONS:
				break;
			case MenuItemSelections.FILTERS:
				break;
			case MenuItemSelections.CLEAR_VIEW:
				break;
			case MenuItemSelections.VIEW_SIDE_BY_SIDE:
				break;
			case MenuItemSelections.MANUAL:
				break;
			case MenuItemSelections.ABOUT_BRAIN_VIEWER:
				break;
		}
        //print(Screen.currentResolution.height);
		MenuBar.menuItemSelected = MenuItemSelections.NONE;
	}

	protected void VoxelPositionFileSelectedCallback(string path) {
		m_fileBrowser = null;
		voxel_position_filepath = path;
		showLoadFileDialog = true;
	}

	protected void VoxelIntensityFileSelectedCallback(string path) {
		m_fileBrowser = null;
		voxel_intensity_filepath = path;
		showLoadFileDialog = true;
	}

	protected void PFileSelectedCallback(string path) {
		m_fileBrowser = null;
		p_values_filepath = path;
		showLoadFileDialog = true;
	}

	protected void TerminalVoxelsFileSelectedCallback(string path) {
		m_fileBrowser = null;
		terminal_voxels_filepath = path;
		showLoadFileDialog = true;
	}

	protected void ArclengthsFileSelectedCallback(string path) {
		m_fileBrowser = null;
		arclengths_filepath = path;
		showLoadFileDialog = true;
	}

	protected void directorySelected(string path) {
		if (path != null)
		{
			string currDir = System.IO.Directory.GetCurrentDirectory();
			//UnityEngine.Debug.Log(currDir);
			System.IO.Directory.SetCurrentDirectory("./processors");
			//UnityEngine.Debug.Log(System.IO.Directory.GetCurrentDirectory());
			libsimple.IProcessor[] available;
			try
			{
				available = libsimple.ProcessorManager.GetReadersFor(path);
			}
			catch(System.Exception e) {
				System.IO.Directory.SetCurrentDirectory(currDir);
				UnityEngine.Debug.LogError(e.ToString());
				return;
			}

			UnityEngine.Debug.Log("Available openers...");

			foreach (libsimple.IProcessor proc in available)
			{
				UnityEngine.Debug.Log(proc.GetProcessorName());
			}

			UnityEngine.Debug.Log("Available openers listed.");

			if (available.Length > 0) {
				List<string> command = new List<string>();
				List<string[]> pipeline = new List<string[]>();
				libsimple.IProcessor proc = available[0];
				command.Add(proc.GetProcessorName());
				command.Add(path);

				pipeline.Add(command.ToArray());

				for (int i = 0; i < MenuBar.filterNameList.Count; i++) {
					command.Clear();
					command.Add(MenuBar.filterNameList[i]);
					UnityEngine.Debug.Log(MenuBar.filterNameList[i]);
					for (int j = 0; j<MenuBar.filterConfigList[i].Length; j++) {
						command.Add(MenuBar.filterConfigList[i][j]);
						UnityEngine.Debug.Log(MenuBar.filterConfigList[i][j]);
					}
					UnityEngine.Debug.Log("--");
					pipeline.Add(command.ToArray());
				}

				libsimple.Pipeline p = new libsimple.Pipeline();

				string[][] configs = pipeline.ToArray();

				for (int i = 0; i < configs.Length; i++) {
					for (int j = 0; j < configs[i].Length; j++) {
						UnityEngine.Debug.Log("" + i + "," + j + " - " + configs[i][j]);
					}
				}

					p.FromArray(pipeline.ToArray());

				libsimple.Packet pckt = p.Run();
				/*
				PacketRenderer renderer = (PacketRenderer)Camera.allCameras[0].gameObject.AddComponent(typeof(PacketRenderer));
				renderer.packetToRender = pckt;
				*/

				OptimizedPacketRenderer renderer = (OptimizedPacketRenderer)Camera.allCameras[0].gameObject.AddComponent(typeof(OptimizedPacketRenderer));
				renderer.packetToRender = pckt;
				//renderer.ScaleTexture = ScaleTexture;
			}
			System.IO.Directory.SetCurrentDirectory(currDir);
		}
		m_fileBrowser = null;
	}

	protected void OnGUI () {
		if (showLoadFileDialog)
		{
			GUILayout.BeginArea(
				new Rect(200, 100, 400, 460),
				"Load File",
				GUI.skin.window
				);

			GUILayout.Space (50);
			GUILayout.Label("Select file containing voxel position: ", GUILayout.Width(300));
			GUILayout.BeginHorizontal();
				if (GUILayout.Button("...", GUILayout.Width(20))) {
					m_fileBrowser = new FileBrowser(
						new Rect(100, 80, 600, 500),
						"Select File",
						VoxelPositionFileSelectedCallback
						);
					m_fileBrowser.SelectionPattern = "*.txt";
					m_fileBrowser.DirectoryImage = m_directoryImage;
					m_fileBrowser.FileImage = m_fileImage;
					showLoadFileDialog = false;
				}
				GUILayout.Label(voxel_position_filepath, GUILayout.Width(300));
			GUILayout.EndHorizontal();

			GUILayout.Space (10);
			GUILayout.Label("Select file containing voxel intensity values: ", GUILayout.Width(300));
			GUILayout.BeginHorizontal();
				if (GUILayout.Button("...", GUILayout.Width(20))) {
					m_fileBrowser = new FileBrowser(
						new Rect(100, 80, 600, 500),
						"Select File",
						VoxelIntensityFileSelectedCallback
						);
					m_fileBrowser.SelectionPattern = "*.txt";
					m_fileBrowser.DirectoryImage = m_directoryImage;
					m_fileBrowser.FileImage = m_fileImage;
					showLoadFileDialog = false;
				}
				GUILayout.Label(voxel_intensity_filepath, GUILayout.Width(300));
			GUILayout.EndHorizontal();

			GUILayout.Space (10);
			GUILayout.Label("Select file containing p-values: ", GUILayout.Width(300));
			GUILayout.BeginHorizontal();
				if (GUILayout.Button("...", GUILayout.Width(20))) {
					m_fileBrowser = new FileBrowser(
						new Rect(100, 80, 600, 500),
						"Select File",
						PFileSelectedCallback
						);
					m_fileBrowser.SelectionPattern = "*.txt";
					m_fileBrowser.DirectoryImage = m_directoryImage;
					m_fileBrowser.FileImage = m_fileImage;
					showLoadFileDialog = false;
				}
				GUILayout.Label(p_values_filepath, GUILayout.Width(300));
			GUILayout.EndHorizontal();

			GUILayout.Space (10);
			GUILayout.Label("Select file containing terminal voxels: ", GUILayout.Width(300));
			GUILayout.BeginHorizontal();
				if (GUILayout.Button("...", GUILayout.Width(20))) {
					m_fileBrowser = new FileBrowser(
						new Rect(100, 80, 600, 500),
						"Select File",
						TerminalVoxelsFileSelectedCallback
						);
					m_fileBrowser.SelectionPattern = "*.txt";
					m_fileBrowser.DirectoryImage = m_directoryImage;
					m_fileBrowser.FileImage = m_fileImage;
					showLoadFileDialog = false;
				}
				GUILayout.Label(terminal_voxels_filepath, GUILayout.Width(300));
			GUILayout.EndHorizontal();

			GUILayout.Space (10);
			GUILayout.Label("Select file containing arclengths: ", GUILayout.Width(300));
			GUILayout.BeginHorizontal();
				if (GUILayout.Button("...", GUILayout.Width(20))) {
					m_fileBrowser = new FileBrowser(
						new Rect(100, 80, 600, 500),
						"Select File",
						ArclengthsFileSelectedCallback
						);
					m_fileBrowser.SelectionPattern = "*.txt";
					m_fileBrowser.DirectoryImage = m_directoryImage;
					m_fileBrowser.FileImage = m_fileImage;
					showLoadFileDialog = false;
				}
				GUILayout.Label(arclengths_filepath, GUILayout.Width(300));
			GUILayout.EndHorizontal();

			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal ();
			GUILayout.Space (100);
			if (GUILayout.Button("Cancel", GUILayout.Width(60))) {
				showLoadFileDialog = false;
			}
			GUILayout.Space (50);
			if (GUILayout.Button("Submit", GUILayout.Width(60))) {
				/*SceneRenderer sceneRenderer = (SceneRenderer) Camera.allCameras[0].gameObject.AddComponent(typeof(SceneRenderer));
				sceneRenderer.voxel_position_filepath = voxel_position_filepath;
				sceneRenderer.voxel_intensity_filepath = voxel_intensity_filepath;
				sceneRenderer.p_values_filepath = p_values_filepath;
				sceneRenderer.terminal_voxels_filepath = terminal_voxels_filepath;
				sceneRenderer.arclengths_filepath = arclengths_filepath;*/
				showLoadFileDialog = false;
			}
			GUILayout.EndHorizontal ();
			GUILayout.EndArea();
		}

		if (m_fileBrowser != null) {
			if (customSkin != null) 
			{
				GUI.skin = customSkin;
				m_fileBrowser.OnGUI();
				GUI.skin = null;
			}
		} 
	}
}
