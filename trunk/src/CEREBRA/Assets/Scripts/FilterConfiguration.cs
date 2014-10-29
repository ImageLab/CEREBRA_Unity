using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;


public class FilterConfiguration {
	
	// Called when the user clicks cancel or select
	public delegate void FinishedCallback(string[] values);
	// Defaults to working directory
	
	protected string[,] m_filters;
	protected GUIContent[] m_filtersGUIContent;
	protected int m_selectedFilter;
	protected string[] m_givenArgs;
	
	protected GUIStyle CentredText {
		get {
			if (m_centredText == null) {
				m_centredText = new GUIStyle(GUI.skin.label);
				m_centredText.alignment = TextAnchor.MiddleLeft;
				m_centredText.fixedHeight = GUI.skin.button.fixedHeight;
			}
			return m_centredText;
		}
	}
	protected GUIStyle m_centredText;
	
	protected string m_name;
	protected Rect m_screenRect;
	
	protected Vector2 m_scrollPosition;
	
	protected FinishedCallback m_callback;
	
	// Browsers need at least a rect, name and callback
	public FilterConfiguration(Rect screenRect, libsimple.IProcessor proc ,FinishedCallback callback) {
		m_name = proc.GetProcessorName() + " configuration";
		m_screenRect = screenRect;
		m_callback = callback;
		m_filters = proc.GetArgs();
		m_givenArgs = new string[m_filters.GetLength(0)];

		for (int i = 0; i < m_givenArgs.Length; i++) {
			m_givenArgs[i] = "";
		}

		BuildContent();
	}

	protected void BuildContent() {
		m_filtersGUIContent = new GUIContent[m_filters.GetLength(0)];
		for (int i = 0; i < m_filtersGUIContent.Length; ++i)
		{
			m_filtersGUIContent[i] = new GUIContent(m_filters[i,0],m_filters[i,1]);
		}
		
	}


	public void OnGUI() {
		GUILayout.BeginArea(
			m_screenRect,
			m_name,
			GUI.skin.window
			);

		GUI.enabled = true;
		GUILayout.Space(20);

		m_scrollPosition = GUILayout.BeginScrollView(
			m_scrollPosition,
			false,
			true,
			GUI.skin.horizontalScrollbar,
			GUI.skin.verticalScrollbar,
			GUI.skin.box
			);

		for (int i = 0; i < m_filters.GetLength(0); i++)
		{

			GUILayout.Label(m_filtersGUIContent[i], GUILayout.Width(300));
			GUILayout.BeginHorizontal();
			m_givenArgs[i] = GUILayout.TextField(m_givenArgs[i],GUILayout.Width(300));
			GUILayout.EndHorizontal();

			GUILayout.Space(10);
		}
			
		GUILayout.EndScrollView();
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Cancel", GUILayout.Width(50))) {
			m_callback(null);
		}
		if (GUILayout.Button("Select", GUILayout.Width(50))) {
			m_callback(m_givenArgs);
		}
		GUI.enabled = true;
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
		
		if (Event.current.type == EventType.Repaint) {
			BuildContent();
		}
	}	
}