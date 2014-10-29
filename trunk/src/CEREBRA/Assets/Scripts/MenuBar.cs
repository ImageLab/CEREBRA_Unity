using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuBar : MonoBehaviour
{
	private bool clicked_file = false,
				 clicked_edit = false,
				 clicked_view = false,
				 clicked_help = false;

	public GUISkin customSkin;

	public static MenuItemSelections menuItemSelected = MenuItemSelections.NONE;

	private float layerDepth = 0.0f;
	private float transparency = 0.0f;
	private float voxelSize = 0.0f;
	private int lastSelectedFilter = 0;
	private Vector2 m_scrollPosition;
	public static List<string> filterNameList = new List<string>();
	public static List<string[]> filterConfigList = new List<string[]>();

	float buttonAlignHorizontal = (float)(MenuBarController.screenWidth * 0.825);
	float buttonAlignVertical = (float)(MenuBarController.screenHeight * 0.100);
	float verticalSpace = (float)(MenuBarController.screenHeight * 0.030);
	float horizontalSpace = (float)(MenuBarController.screenWidth * 0.050);
	float zoomBoxWidth = (float)(MenuBarController.screenWidth * 0.050);
	float zoomBoxHeight = (float)(MenuBarController.screenHeight * 0.115);
	float smallButtonWidth = (float)(MenuBarController.screenWidth * 0.020);
	float smallButtonHeight = (float)(MenuBarController.screenHeight * 0.037);
	float mediumButtonWidth = (float)(MenuBarController.screenWidth * 0.050);
	float mediumButtonHeight = (float)(MenuBarController.screenHeight * 0.020);
	float sliderWidth = (float)(MenuBarController.screenWidth * 0.130);
	float sliderHeight = (float)(MenuBarController.screenHeight * 0.018);
	float filterBoxWidth = (float)(MenuBarController.screenWidth * 0.182);
	float filterBoxHeight = (float)(MenuBarController.screenHeight * 0.231);
	float space = 0;

	private FilterBrowser m_filterBrowser;
	private FilterConfiguration m_filterConfiguration;

	private float LabelSlider(Rect screenRect, float sliderValue, float sliderMaxValue, string labelText)
	{
		GUI.Label(screenRect, labelText);

		// &lt;- Push the Slider to the end of the Label
		screenRect.x += horizontalSpace;
		screenRect.y += (float)(MenuBarController.screenHeight * 0.0028);

		sliderValue = GUI.HorizontalSlider(screenRect, sliderValue, 0.0f, sliderMaxValue);
		return sliderValue;
	}

	private void onFilterSelected(string f)
	{
		UnityEngine.Debug.Log(f);

		if (f != null)
		{
			filterNameList.Add(f);
			libsimple.IProcessor proc = libsimple.ProcessorManager.GetProcessorInstance(f);
			if (proc.GetArgs().GetLength(0) > 0)
			{
				m_filterConfiguration = new FilterConfiguration(new Rect(100, 80, 600, 500), proc, onFilterConfigurationDone);
			}
		}

		m_filterBrowser = null;
	}

	private void onFilterConfigurationDone(string[] conf)
	{
		filterConfigList.Add(conf);
		m_filterConfiguration = null;
	}


	private void OnGUI()
	{
		if (m_filterBrowser != null) {
			GUI.skin = customSkin;
			m_filterBrowser.OnGUI();
			GUI.skin = null;
		}

		if (m_filterConfiguration != null)
		{
			GUI.skin = customSkin;
			m_filterConfiguration.OnGUI();
			GUI.skin = null;
		}

		float zoomBoxButtonPlacement = (float)(zoomBoxWidth * 0.3);

		GUI.Box(new Rect(buttonAlignHorizontal, buttonAlignVertical, zoomBoxWidth, zoomBoxHeight), "Zoom");
		space = verticalSpace;
		if (GUI.Button(new Rect(buttonAlignHorizontal + zoomBoxButtonPlacement, buttonAlignVertical + space, smallButtonWidth, smallButtonHeight), "+"))
		{

		}
		space = (float)(smallButtonHeight * 2);
		if (GUI.Button(new Rect(buttonAlignHorizontal + zoomBoxButtonPlacement, buttonAlignVertical + space, smallButtonWidth, smallButtonHeight), "-"))
		{

		}
		space += verticalSpace * 2;
		layerDepth = LabelSlider(new Rect(buttonAlignHorizontal - horizontalSpace, buttonAlignVertical + space, sliderWidth, sliderHeight), layerDepth, 100.0f, "Layer Depth");
		space += verticalSpace;
		transparency = LabelSlider(new Rect(buttonAlignHorizontal - horizontalSpace, buttonAlignVertical + space, sliderWidth, sliderHeight), transparency, 100.0f, "Transparency");
		space += verticalSpace;
		voxelSize = LabelSlider(new Rect(buttonAlignHorizontal - horizontalSpace, buttonAlignVertical + space, sliderWidth, sliderHeight), voxelSize, 100.0f, "Voxel Size");
		space += verticalSpace;

		Rect m_filterRect = new Rect(buttonAlignHorizontal - horizontalSpace, buttonAlignVertical + space, filterBoxWidth, filterBoxHeight);

		space += verticalSpace + filterBoxHeight;
		GUILayout.BeginArea(
			m_filterRect,
			"Filters",
			GUI.skin.window
			);
		m_scrollPosition = GUILayout.BeginScrollView(
			m_scrollPosition,
			false,
			true,
			GUI.skin.horizontalScrollbar,
			GUI.skin.verticalScrollbar,
			GUI.skin.box
			);

		GUI.enabled = true;
		GUI.skin = customSkin;
		lastSelectedFilter = GUILayoutx.SelectionList(lastSelectedFilter, filterNameList.ToArray());
		GUI.skin = null;
		GUILayout.EndScrollView();
		GUILayout.EndArea();

		if (GUI.Button(new Rect(buttonAlignHorizontal + zoomBoxButtonPlacement, buttonAlignVertical + space, smallButtonWidth, smallButtonHeight), "+"))
		{
			m_filterBrowser = new FilterBrowser(
					new Rect(100, 80, 600, 500),
					"Select Filter",
					onFilterSelected
					);
			menuItemSelected = MenuItemSelections.FILTERS;
		}

		space += verticalSpace + filterBoxHeight;
		GUI.Label(new Rect(buttonAlignHorizontal - horizontalSpace, buttonAlignVertical + space, mediumButtonWidth, mediumButtonHeight), "Region");
		if (GUI.Button(new Rect(buttonAlignHorizontal, buttonAlignVertical + space, mediumButtonWidth, mediumButtonHeight), "Whole Brain | >")) // This will be a dropdown button
		{

		}
		space += verticalSpace;
		GUI.Label(new Rect(buttonAlignHorizontal - horizontalSpace, buttonAlignVertical + space, mediumButtonWidth * 2, mediumButtonHeight), "Color Blind Mode");
		if (GUI.Button(new Rect(buttonAlignHorizontal + smallButtonWidth, buttonAlignVertical + space, mediumButtonWidth, mediumButtonHeight), "On")) // This will be a dropdown button
		{

		}
		if (GUI.Button(new Rect(0, 0, mediumButtonWidth, mediumButtonHeight), "File"))
		{
			clicked_file = !clicked_file;

			if (clicked_file)
			{
				clicked_edit = false;
				clicked_view = false;
				clicked_help = false;
			}
		}

		if (GUI.Button(new Rect(mediumButtonWidth, 0, mediumButtonWidth, mediumButtonHeight), "Edit"))
		{
			clicked_edit = !clicked_edit;

			if (clicked_edit)
			{
				clicked_file = false;
				clicked_view = false;
				clicked_help = false;
			}
		}

		if (GUI.Button(new Rect(mediumButtonWidth * 2, 0, mediumButtonWidth, mediumButtonHeight), "View"))
		{
			clicked_view = !clicked_view;

			if (clicked_view)
			{
				clicked_file = false;
				clicked_edit = false;
				clicked_help = false;
			}
		}

		if (GUI.Button(new Rect(mediumButtonWidth * 3, 0, mediumButtonWidth, mediumButtonHeight), "Help"))
		{
			clicked_help = !clicked_help;

			if (clicked_help)
			{
				clicked_file = false;
				clicked_edit = false;
				clicked_view = false;
			}
		}

		if (clicked_file)
		{
			if (GUI.Button(new Rect(0, mediumButtonHeight, mediumButtonWidth, mediumButtonHeight), "Load File"))
			{
				clicked_file = false;
				menuItemSelected = MenuItemSelections.LOAD_FILE;
			}

			if (GUI.Button(new Rect(0, mediumButtonHeight * 2, mediumButtonWidth, mediumButtonHeight), "Open Recent"))
			{
				clicked_file = false;
				menuItemSelected = MenuItemSelections.OPEN_RECENT;
			}

			if (GUI.Button(new Rect(0, mediumButtonHeight * 3, mediumButtonWidth, mediumButtonHeight), "Save Image"))
			{
				clicked_file = false;
				menuItemSelected = MenuItemSelections.SAVE_IMAGE;
			}

			if (GUI.Button(new Rect(0, mediumButtonHeight * 4, mediumButtonWidth, mediumButtonHeight), "Save Movie"))
			{
				clicked_file = false;
				menuItemSelected = MenuItemSelections.SAVE_MOVIE;
			}

			if (GUI.Button(new Rect(0, mediumButtonHeight * 5, mediumButtonWidth, mediumButtonHeight), "Exit"))
			{
				clicked_file = false;
				menuItemSelected = MenuItemSelections.EXIT;
			}
		}

		if (clicked_edit)
		{
			clicked_file = false;
			clicked_view = false;
			clicked_help = false;

			if (GUI.Button(new Rect(mediumButtonWidth, mediumButtonHeight, mediumButtonWidth, mediumButtonHeight), "Options"))
			{
				clicked_edit = false;
				menuItemSelected = MenuItemSelections.OPTIONS;
			}

			if (GUI.Button(new Rect(mediumButtonWidth, mediumButtonHeight * 2, mediumButtonWidth, mediumButtonHeight), "Filters"))
			{
				clicked_edit = false;
				menuItemSelected = MenuItemSelections.FILTERS;
			}
		}

		if (clicked_view)
		{
			clicked_file = false;
			clicked_edit = false;
			clicked_help = false;

			if (GUI.Button(new Rect(mediumButtonWidth * 2, mediumButtonHeight, mediumButtonWidth, mediumButtonHeight), "Clear View"))
			{
				clicked_view = false;
				menuItemSelected = MenuItemSelections.CLEAR_VIEW;
			}

			if (GUI.Button(new Rect(mediumButtonWidth * 2, mediumButtonHeight * 2, mediumButtonWidth, mediumButtonHeight), "Side-By-Side"))
			{
				clicked_view = false;
				menuItemSelected = MenuItemSelections.VIEW_SIDE_BY_SIDE;
			}
		}

		if (clicked_help)
		{
			clicked_file = false;
			clicked_edit = false;
			clicked_view = false;

			if (GUI.Button(new Rect(mediumButtonWidth * 3, mediumButtonHeight, mediumButtonWidth, mediumButtonHeight), "Manual"))
			{
				clicked_help = false;
				menuItemSelected = MenuItemSelections.MANUAL;
			}

			if (GUI.Button(new Rect(mediumButtonWidth * 3, mediumButtonHeight * 2, mediumButtonWidth, mediumButtonHeight), "About"))
			{
				clicked_help = false;
				menuItemSelected = MenuItemSelections.ABOUT_BRAIN_VIEWER;
			}
		}

	}
}