using UnityEngine;

public class TextFileFinder : MonoBehaviour {

	public GUISkin customSkin = null;

	protected string m_textPath;
	
	protected FileBrowser m_fileBrowser;
	
	[SerializeField]
	protected Texture2D	m_directoryImage,
	m_fileImage;
	
	protected void OnGUI () {

		if (m_fileBrowser != null) {
			if (customSkin != null) 
			{
				GUI.skin = customSkin;
				m_fileBrowser.OnGUI();
				GUI.skin = null;
			}
		} else {
			OnGUIMain();
		}
	}
	
	protected void OnGUIMain() {
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("Text File", GUILayout.Width(100));
		GUILayout.FlexibleSpace();
		GUILayout.Label(m_textPath ?? "none selected");
		if (GUILayout.Button("...", GUILayout.ExpandWidth(false))) {
			m_fileBrowser = new FileBrowser(
				new Rect(100, 100, 600, 500),
				"Choose Text File",
				FileSelectedCallback
				);
			m_fileBrowser.SelectionPattern = "*.txt";
			m_fileBrowser.DirectoryImage = m_directoryImage;
			m_fileBrowser.FileImage = m_fileImage;
		}
		GUILayout.EndHorizontal();
	}
	
	protected void FileSelectedCallback(string path) {
		m_fileBrowser = null;
		m_textPath = path;
	}
}