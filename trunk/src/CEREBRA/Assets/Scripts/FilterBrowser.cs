using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;


public class FilterBrowser {
	
	// Called when the user clicks cancel or select
	public delegate void FinishedCallback(string path);
	// Defaults to working directory
	
	protected string[] m_filters;
	protected GUIContent[] m_filtersGUIContent;
	protected int m_selectedFilter;
	
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
	public FilterBrowser(Rect screenRect, string name, FinishedCallback callback) {
		m_name = name;
		m_screenRect = screenRect;
		m_callback = callback;
		m_filters = libsimple.ProcessorManager.GetRegisteredProcessors();
		List<string> lst = new List<string>();

		foreach(string fname in m_filters) {
			if (libsimple.ProcessorManager.GetProcessorInstance(fname).GetProcessorType() == "process")
				lst.Add(fname);
		}

		m_filters = lst.ToArray();

		BuildContent();
	}

	protected void BuildContent() {
		m_filtersGUIContent = new GUIContent[m_filters.Length];
		for (int i = 0; i < m_filtersGUIContent.Length; ++i)
		{
			m_filtersGUIContent[i] = new GUIContent(m_filters[i]);
		}
		
	}


	public void OnGUI() {
		GUILayout.BeginArea(
			m_screenRect,
			m_name,
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
		m_selectedFilter = GUILayoutx.SelectionList(
			m_selectedFilter,
			m_filtersGUIContent,
			FilterDoubleClickCallback
			);
		GUILayout.EndScrollView();
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Cancel", GUILayout.Width(50))) {
			m_callback(null);
		}
		if (GUILayout.Button("Select", GUILayout.Width(50))) {
			m_callback(m_filters[m_selectedFilter]);
		}
		GUI.enabled = true;
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
		
		if (Event.current.type == EventType.Repaint) {
			BuildContent();
		}
	}
	
	protected void FilterDoubleClickCallback(int i) {
		m_callback(m_filters[i]);
	}
	
	
}