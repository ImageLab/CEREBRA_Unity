using UnityEngine;
using System.Collections;

public class Transparent : MonoBehaviour {
	
	
	// Use this for initialization
	void Start () {
		gameObject.SetActive (true);
		//gameObject.renderer.material.color = Color.green;
		GameObject.Find("default_MeshPart0").renderer.material.color = new Color(1f,1f,1f,0.5f);
		GameObject.Find("default_MeshPart1").renderer.material.color = new Color(1f,1f,1f,0.5f);	
		GameObject.Find("default_MeshPart2").renderer.material.color = new Color(1f,1f,1f,0.5f);
		
	}
	
	// Update is called once per frame
	void Update () {
		/*if (Input.GetKeyDown (KeyCode.Space)) {
			GameObject.Find ("default_MeshPart0").renderer.material.color = new Color (1f, 1f, 1f, 0.8f);
			GameObject.Find ("default_MeshPart1").renderer.material.color = new Color (1f, 1f, 1f, 0.8f);	
			GameObject.Find ("default_MeshPart2").renderer.material.color = new Color (1f, 1f, 1f, 0.8f);
		}
		if(Input.GetMouseButtonDown(0))
		{
			//gameObject.SetActive (false);
			GameObject.Find ("default_MeshPart0").renderer.material.color = new Color (1f, 1f, 1f, 0.5f);
			GameObject.Find ("default_MeshPart1").renderer.material.color = new Color (1f, 1f, 1f, 0.5f);	
			GameObject.Find ("default_MeshPart2").renderer.material.color = new Color (1f, 1f, 1f, 0.5f);
			gameObject.SetActive (true);
			
		}*/

		
		
		
	}
}