using UnityEngine;
using System.Collections;

public class MenuBarControllerCustomizable : MonoBehaviour
{
	private bool showLoadFileDialog = false;
	//private bool showFilterSelectDialog = false;
	//private bool showFilterConfigureDialog = false;

	private System.Action<string[], int, string> setArg;//=(a,i,e) =>a[i]=e;
	private System.Action<int, string> curriedSetArg;

	private string voxel_position_filepath = "No file selected.",
		voxel_intensity_filepath = "No file selected.",
		p_values_filepath = "No file selected.",
		terminal_voxels_filepath = "No file selected.",
		arclengths_filepath = "No file selected.";

	public GUISkin customSkin = null;
	
	protected string m_textPath;

	protected FileBrowser m_fileBrowser;
	protected FilterBrowser m_filterBrowser;

	[SerializeField]
	protected Texture2D m_directoryImage,
	m_fileImage;

	public static double screenHeight = Screen.currentResolution.height;
	public static double screenWidth = Screen.currentResolution.width;

	// Update is called once per frame
	void Update()
	{
		switch (MenuBar.menuItemSelected)
		{
			case MenuItemSelections.LOAD_FILE:
				showLoadFileDialog = true;
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
				//showFilterSelectDialog = true;
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

	protected void VoxelPositionFileSelectedCallback(string path)
	{
		m_fileBrowser = null;
		voxel_position_filepath = path;
		showLoadFileDialog = true;
	}

	protected void VoxelIntensityFileSelectedCallback(string path)
	{
		m_fileBrowser = null;
		voxel_intensity_filepath = path;
		showLoadFileDialog = true;
	}

	protected void PFileSelectedCallback(string path)
	{
		m_fileBrowser = null;
		p_values_filepath = path;
		showLoadFileDialog = true;
	}

	protected void TerminalVoxelsFileSelectedCallback(string path)
	{
		m_fileBrowser = null;
		terminal_voxels_filepath = path;
		showLoadFileDialog = true;
	}

	protected void ArclengthsFileSelectedCallback(string path)
	{
		m_fileBrowser = null;
		arclengths_filepath = path;
		showLoadFileDialog = true;
	}

	protected void OnGUI()
	{
		if (showLoadFileDialog)
		{
			GUILayout.BeginArea(
				new Rect(200, 100, 400, 460),
				"Load File",
				GUI.skin.window
				);

			GUILayout.Space(50);
			GUILayout.Label("Select file containing voxel position: ", GUILayout.Width(300));
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("...", GUILayout.Width(20)))
			{
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

			GUILayout.Space(10);
			GUILayout.Label("Select file containing voxel intensity values: ", GUILayout.Width(300));
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("...", GUILayout.Width(20)))
			{
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

			GUILayout.Space(10);
			GUILayout.Label("Select file containing p-values: ", GUILayout.Width(300));
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("...", GUILayout.Width(20)))
			{
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

			GUILayout.Space(10);
			GUILayout.Label("Select file containing terminal voxels: ", GUILayout.Width(300));
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("...", GUILayout.Width(20)))
			{
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

			GUILayout.Space(10);
			GUILayout.Label("Select file containing arclengths: ", GUILayout.Width(300));
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("...", GUILayout.Width(20)))
			{
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
			GUILayout.BeginHorizontal();
			GUILayout.Space(100);
			if (GUILayout.Button("Cancel", GUILayout.Width(60)))
			{
				showLoadFileDialog = false;
			}
			GUILayout.Space(50);
			if (GUILayout.Button("Submit", GUILayout.Width(60)))
			{
				/*SceneRenderer sceneRenderer = (SceneRenderer)Camera.allCameras[0].gameObject.AddComponent(typeof(SceneRenderer));
				sceneRenderer.voxel_position_filepath = voxel_position_filepath;
				sceneRenderer.voxel_intensity_filepath = voxel_intensity_filepath;
				sceneRenderer.p_values_filepath = p_values_filepath;
				sceneRenderer.terminal_voxels_filepath = terminal_voxels_filepath;
				sceneRenderer.arclengths_filepath = arclengths_filepath;*/
				showLoadFileDialog = false;
			}
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}

		if (m_fileBrowser != null)
		{
			if (customSkin != null)
			{
				GUI.skin = customSkin;
				m_fileBrowser.OnGUI();
				GUI.skin = null;
			}
		}
	}
}
