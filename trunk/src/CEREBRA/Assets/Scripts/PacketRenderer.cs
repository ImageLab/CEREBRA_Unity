using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PacketRenderer : MonoBehaviour
{
	int zoom = 60;
	float smooth = 5;

	public float moveSpeed = 10f;
	//private float speed = 1.0f;

	public libsimple.Packet packetToRender;

	// Use this for initialization
	void Start()
	{
		List<Vector3> voxelPositions = new List<Vector3>();
		for (int i = 0; i < packetToRender.vXYZ.Length; i++ )
		{
			GameObject voxelObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			voxelObject.transform.position = new Vector3((float)packetToRender.vXYZ[i].x, (float)packetToRender.vXYZ[i].y, (float)packetToRender.vXYZ[i].z);
			voxelPositions.Add(voxelObject.transform.position);
			
			voxelObject.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
			voxelObject.isStatic = true;
		}
		UnityEngine.Debug.Log(packetToRender.vXYZ.Length);
		UnityEngine.Debug.Log(packetToRender.Edges.GetLength(1));

		for (int i = 0; i < packetToRender.vXYZ.Length; i++)
		{
			for (int j = 0; j < packetToRender.Edges[0, i].Length; j++)
			{
				/*GameObject line = new GameObject();
				var lr = line.AddComponent<LineRenderer>();
				lr.SetPosition(0, voxelPositions[i]);
				lr.SetPosition(1, voxelPositions[packetToRender.Edges[0, i][j].Key]);
				//lr.SetColors(... , ...);
				lr.SetWidth(0.1f, 0.1f);*/
				GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

				//s += packetToRender.Edges[0, i][j].Key + " ";

				Vector3 between = voxelPositions[i] - voxelPositions[packetToRender.Edges[0, i][j].Key];
				float distance = between.magnitude;
				cube.transform.localScale = new Vector3(0.05f, 0.05f, distance);
				cube.transform.position = voxelPositions[i] - (between / 2.0f);
				cube.transform.LookAt(voxelPositions[packetToRender.Edges[0, i][j].Key]);
				//cube.renderer.material.color = new Color(1.0f,1.0f,0.0f,1.0f);
				cube.isStatic = true;
			}
			UnityEngine.Debug.Log(packetToRender.vXYZ[i].x + ":" + packetToRender.vXYZ[i].y + ":" + packetToRender.vXYZ[i].z);
			//s = "";
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKey("w"))
		{
			transform.Translate(-Vector3.up * moveSpeed * Time.deltaTime);
		}
		if (Input.GetKey("a"))
		{
			transform.Translate(new Vector3(1.0f, 0.0f, 0.0f) * moveSpeed * Time.deltaTime);
		}
		if (Input.GetKey("s"))
		{
			transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
		}
		if (Input.GetKey("d"))
		{
			transform.Translate(new Vector3(-1.0f, -0.0f, -0.0f) * moveSpeed * Time.deltaTime);
		}

		if (Input.GetKeyDown("z"))
		{
			zoom = 60;
		}

		if (Input.GetAxis("Mouse ScrollWheel") < 0) // forward
		{
			zoom++;
			if (zoom > 100)
				zoom = 100;
		}
		if (Input.GetAxis("Mouse ScrollWheel") > 0) // back
		{
			zoom--;
			if (zoom < 0)
				zoom = 0;
		}

		camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, zoom, Time.deltaTime * smooth);

	}
}
