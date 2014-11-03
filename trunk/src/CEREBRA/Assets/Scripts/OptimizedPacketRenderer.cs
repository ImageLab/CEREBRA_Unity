using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OptimizedPacketRenderer : MonoBehaviour {
	int zoom = 60;
	float smooth = 5;

	public float moveSpeed = 10f;
	public Texture ScaleTexture;
	//private float speed = 1.0f;
    public libsimple.Packet packetToRender;

    float userMax;
    float userMin;
    float voxelMin;
    float voxelMax;
	

	void generateVoxelGeometry(float size=1f, bool resizeByIntensity = false) 
	{

		if ( packetToRender != null)
		{

            if (packetToRender.hideVoxels == true) return;

			int lenVoxels=packetToRender.vXYZ.Length;

			if (lenVoxels > 8000)
				lenVoxels = 8000;

			int numVertices = lenVoxels * 8;
			int numTris = lenVoxels * 36;

			Vector3[] vertices = new Vector3[numVertices];
			Vector2[] uvs = new Vector2[numVertices]; // texture indexes
			int[] tris = new int[numTris];

			int i = 0;

			int numVoxels=packetToRender.vXYZ.Length;

			for (int j = 0; j < numVoxels; j++, i++)
			{
				if (i >= 8000) {
					GameObject g = new GameObject("VoxelNode_"+(j/8000).ToString());
					g.transform.parent = GameObject.Find("TargetGameObject").transform;
					Mesh me = g.AddComponent<MeshFilter>().mesh;
					g.AddComponent<MeshRenderer>();
					g.renderer.material.mainTexture = ScaleTexture;

					me.Clear();

					me.vertices = vertices;
					me.uv = uvs;
					me.triangles = tris;
					me.RecalculateNormals();
					me.RecalculateBounds();

					me.Optimize();

					lenVoxels=numVoxels-j;
					if (lenVoxels > 8000)
						lenVoxels = 8000;

					numVertices = lenVoxels * 8;
					numTris = lenVoxels * 36;

					vertices = new Vector3[numVertices];
					uvs = new Vector2[numVertices]; // texture indexes
					tris = new int[numTris];

					i = 0;
				}
                //DENEME
               
                float temp = size * 0.5f * Mathf.Clamp01((float)packetToRender.Intensities[0, j]);
                float newsize =userMax - ( (0.5f * (voxelMax - temp)) / (voxelMax - voxelMin) );
                float spread = resizeByIntensity ? newsize : size * 0.5f;
                //float spread = size * 0.5f;
                
                // \DENEME
				//ORIGINAL : 
                //float spread = resizeByIntensity ? size * 0.5f * Mathf.Clamp01((float)packetToRender.Intensities[0, j]) : size * 0.5f;

                
				//UnityEngine.Debug.Log("x: " + packetToRender.vXYZ[j].x + ", y: " + packetToRender.vXYZ[j].y + ", z: " + packetToRender.vXYZ[j].z);

				// push vertices
				vertices[i * 8 + 0] = new Vector3((float)packetToRender.vXYZ[j].x - spread, (float)packetToRender.vXYZ[j].y - spread, (float)packetToRender.vXYZ[j].z + spread);
				vertices[i * 8 + 1] = new Vector3((float)packetToRender.vXYZ[j].x + spread, (float)packetToRender.vXYZ[j].y - spread, (float)packetToRender.vXYZ[j].z + spread);
				vertices[i * 8 + 2] = new Vector3((float)packetToRender.vXYZ[j].x + spread, (float)packetToRender.vXYZ[j].y - spread, (float)packetToRender.vXYZ[j].z - spread);
				vertices[i * 8 + 3] = new Vector3((float)packetToRender.vXYZ[j].x - spread, (float)packetToRender.vXYZ[j].y - spread, (float)packetToRender.vXYZ[j].z - spread);
				vertices[i * 8 + 4] = new Vector3((float)packetToRender.vXYZ[j].x - spread, (float)packetToRender.vXYZ[j].y + spread, (float)packetToRender.vXYZ[j].z + spread);
				vertices[i * 8 + 5] = new Vector3((float)packetToRender.vXYZ[j].x + spread, (float)packetToRender.vXYZ[j].y + spread, (float)packetToRender.vXYZ[j].z + spread);
				vertices[i * 8 + 6] = new Vector3((float)packetToRender.vXYZ[j].x + spread, (float)packetToRender.vXYZ[j].y + spread, (float)packetToRender.vXYZ[j].z - spread);
				vertices[i * 8 + 7] = new Vector3((float)packetToRender.vXYZ[j].x - spread, (float)packetToRender.vXYZ[j].y + spread, (float)packetToRender.vXYZ[j].z - spread);

				// push texture coordinates
				uvs[i * 8 + 0] = new Vector2(0.5f, Mathf.Clamp01((float)packetToRender.Intensities[0, i]));
				uvs[i * 8 + 1] = uvs[i * 8 + 0];
				uvs[i * 8 + 2] = uvs[i * 8 + 0];
				uvs[i * 8 + 3] = uvs[i * 8 + 0];
				uvs[i * 8 + 4] = uvs[i * 8 + 0];
				uvs[i * 8 + 5] = uvs[i * 8 + 0];
				uvs[i * 8 + 6] = uvs[i * 8 + 0];
				uvs[i * 8 + 7] = uvs[i * 8 + 0];

				// push triangles
				// bottom
				tris[i * 36 + 0] = i * 8 + 3; tris[i * 36 + 1] = i * 8 + 1; tris[i * 36 + 2] = i * 8 + 0;
				tris[i * 36 + 3] = i * 8 + 3; tris[i * 36 + 4] = i * 8 + 2; tris[i * 36 + 5] = i * 8 + 1;
				// left
				tris[i * 36 + 6] = i * 8 + 3; tris[i * 36 + 7] = i * 8 + 4; tris[i * 36 + 8] = i * 8 + 7;
				tris[i * 36 + 9] = i * 8 + 3; tris[i * 36 + 10] = i * 8 + 0; tris[i * 36 + 11] = i * 8 + 4;
				// front
				tris[i * 36 + 12] = i * 8 + 0; tris[i * 36 + 13] = i * 8 + 5; tris[i * 36 + 14] = i * 8 + 4;
				tris[i * 36 + 15] = i * 8 + 0; tris[i * 36 + 16] = i * 8 + 1; tris[i * 36 + 17] = i * 8 + 5;
				// back
				tris[i * 36 + 18] = i * 8 + 2; tris[i * 36 + 19] = i * 8 + 7; tris[i * 36 + 20] = i * 8 + 6;
				tris[i * 36 + 21] = i * 8 + 2; tris[i * 36 + 22] = i * 8 + 3; tris[i * 36 + 23] = i * 8 + 7;
				// right
				tris[i * 36 + 24] = i * 8 + 1; tris[i * 36 + 25] = i * 8 + 6; tris[i * 36 + 26] = i * 8 + 5;
				tris[i * 36 + 27] = i * 8 + 1; tris[i * 36 + 28] = i * 8 + 2; tris[i * 36 + 29] = i * 8 + 6;
				// top
				tris[i * 36 + 30] = i * 8 + 4; tris[i * 36 + 31] = i * 8 + 6; tris[i * 36 + 32] = i * 8 + 7;
				tris[i * 36 + 33] = i * 8 + 4; tris[i * 36 + 34] = i * 8 + 5; tris[i * 36 + 35] = i * 8 + 6;
			}

			GameObject go = new GameObject("VoxelNode_final");
			go.transform.parent = GameObject.Find("TargetGameObject").transform;
			Mesh m = go.AddComponent<MeshFilter>().mesh;
			go.AddComponent<MeshRenderer>();
			go.renderer.material.mainTexture = ScaleTexture;

			m.Clear();

            if( !packetToRender.hideVoxels)
			    m.vertices = vertices;
			m.uv = uvs;
			m.triangles = tris;
			m.RecalculateNormals();
			m.RecalculateBounds();

			m.Optimize();

			//GameObject.Find("TargetGameObject").renderer.material.SetColor(0,new Color(1, 1, 1, 0.7f));
		}
        UnityEngine.Debug.Log("optimized");
	}

	void generateConvexHullGeometry()
	{
		libsimple.Packet.Point3D[][] points = (libsimple.Packet.Point3D[][])packetToRender.getExtra("convexHullVertices");
		int[][][] faces = (int[][][])packetToRender.getExtra("convexHullFaces");

		for (int i = 0; i < points.Length; i++)
		{
			int numTris = 0;
			for (int j = 0; j < faces[i].Length; j++)
			{
				numTris += faces[i][j].Length - 2;
			}
			numTris *= 3;

			Vector3[] vertices = new Vector3[points[i].Length];
			for (int j = 0; j < points[i].Length; j++)
			{
				vertices[j] = new Vector3(points[i][j].x, points[i][j].y, points[i][j].z);
			}

			Vector2[] uvs = new Vector2[points[i].Length];
			for (int j = 0; j < points[i].Length; j++)
			{
				uvs[j] = new Vector2(0.5f, 1.0f / points.Length * i);
			}

			int[] tris = new int[numTris];
			int trisIndex = 0;
			for (int j = 0; j < faces[i].Length; j++)
			{
				for (int k = 2; k < faces[i][j].Length; k++)
				{
					tris[trisIndex] = faces[i][j][0];
					tris[trisIndex + 1] = faces[i][j][1];
					tris[trisIndex + 2] = faces[i][j][k];
					trisIndex += 3;
				}
			}

			GameObject g = new GameObject("ConvexHullNode_" + i.ToString());
			g.transform.parent = GameObject.Find("TargetGameObject").transform;
			Mesh me = g.AddComponent<MeshFilter>().mesh;
			g.AddComponent<MeshRenderer>();
			g.renderer.material.shader = Shader.Find("Transparent/Diffuse");
			Color c = g.renderer.material.color;
			c.a = 0.5f;
			g.renderer.material.color = c;
			g.renderer.material.mainTexture = ScaleTexture;

			me.Clear();

			me.vertices = vertices;
			me.uv = uvs;
			me.triangles = tris;
			me.RecalculateNormals();
			me.RecalculateBounds();

			me.Optimize();
		}
	}

	// Use this for initialization
	void Start()
	{
        userMax = (float)packetToRender.VoxelSizeRange[1];
        userMin = (float)packetToRender.VoxelSizeRange[0];

        voxelMin = (float)packetToRender.Intensities[0, 0];
        voxelMax = (float)packetToRender.Intensities[0, 0];

        for (int o = 0; o < packetToRender.vXYZ.Length; o++)
        {
            if ((float)packetToRender.Intensities[0, o] < voxelMin) voxelMin = (float)packetToRender.Intensities[0, o];
            if ((float)packetToRender.Intensities[0, o] > voxelMax) voxelMax = (float)packetToRender.Intensities[0, o];
        }
        
		generateVoxelGeometry(0.4f,true);

		List<Vector3> voxelPositions = new List<Vector3>();
		for (int i = 0; i < packetToRender.vXYZ.Length; i++)
		{
			voxelPositions.Add(new Vector3((float)packetToRender.vXYZ[i].x, (float)packetToRender.vXYZ[i].y, (float)packetToRender.vXYZ[i].z));
		}

		if (packetToRender.getExtra("useConvexHullRenderer") != null)
			generateConvexHullGeometry();
		
		if (packetToRender.Edges != null) {
			generateEdgeGeometry(voxelPositions);
		}
	}

	private void generateEdgeGeometry(List<Vector3> voxelPositions)
	{
		int i = 0, j = 0;
		int nextSplitNode = 0;
		int splitSize = 0;
		float sqrt3 = (float)Mathf.Sqrt(3);
		float arrowDistanceToActualEdge = 0.03f;
		float triangleBase = 0.07f;
		float triangleBaseDistanceToVoxel = 0.1f;

		while (nextSplitNode < packetToRender.vXYZ.Length) {
			splitSize += packetToRender.Edges[0, nextSplitNode].Length;
			if (splitSize >= 16000)
			{
				splitSize -= packetToRender.Edges[0, nextSplitNode].Length;
				break;
			}
			else {
				nextSplitNode++;
			}
		}

		int numVertices = splitSize * 4;

		Vector3[] vertices = new Vector3[numVertices];
		Vector2[] uvs = new Vector2[numVertices]; // texture indexes
		int[] tris = new int[numVertices*3];

		for (; i < packetToRender.vXYZ.Length; i++) {
			if (i == nextSplitNode) {
				GameObject g = new GameObject("EdgeNodeAt_" + nextSplitNode.ToString());
				g.transform.parent = GameObject.Find("TargetGameObject").transform;
				Mesh me = g.AddComponent<MeshFilter>().mesh;
				g.AddComponent<MeshRenderer>();
				g.renderer.material.mainTexture = ScaleTexture;

				me.Clear();

				me.vertices = vertices;
				me.uv = uvs;
				me.triangles = tris;
				me.RecalculateNormals();
				me.RecalculateBounds();

				me.Optimize();

				splitSize = 0; // i am an idiot

				while (nextSplitNode < packetToRender.vXYZ.Length)
				{
					splitSize += packetToRender.Edges[0, nextSplitNode].Length;
					if (splitSize >= 16000)
					{
						splitSize -= packetToRender.Edges[0, nextSplitNode].Length;
						break;
					}
					else
					{
						nextSplitNode++;
					}
				}

				numVertices = splitSize * 4;

				vertices = new Vector3[numVertices];
				uvs = new Vector2[numVertices]; // texture indexes
				tris = new int[numVertices * 3];

				j = 0;
			}

			for (int k = 0; k < packetToRender.Edges[0, i].Length; k++, j++) 
			{
				int otherVoxel = packetToRender.Edges[0, i][k].Key;

				// this ": ?" parts select upper and lower spined pyramids.
				// top
				vertices[j * 4 + 0] = new Vector3(0f, (otherVoxel > i ? -(triangleBase/2*sqrt3+arrowDistanceToActualEdge) : triangleBase/2*sqrt3+arrowDistanceToActualEdge), triangleBaseDistanceToVoxel);
				// left
				vertices[j * 4 + 1] = new Vector3((otherVoxel > i ? -triangleBase/2 : triangleBase/2), (otherVoxel > i ? -arrowDistanceToActualEdge : arrowDistanceToActualEdge), triangleBaseDistanceToVoxel);
				// right
				vertices[j * 4 + 2] = new Vector3((otherVoxel > i ? triangleBase/2 : -triangleBase/2), (otherVoxel > i ? -arrowDistanceToActualEdge : arrowDistanceToActualEdge), triangleBaseDistanceToVoxel);
				// pointing
				vertices[j * 4 + 3] = new Vector3(0f, (otherVoxel > i ? -(triangleBase / 2 / sqrt3 + arrowDistanceToActualEdge) : (triangleBase / 2 / sqrt3 + arrowDistanceToActualEdge)), (voxelPositions[otherVoxel] - voxelPositions[i]).magnitude - triangleBaseDistanceToVoxel);

				// rotate vertices here
				Quaternion rot = Quaternion.FromToRotation(new Vector3(0f, 0f, 1f), (voxelPositions[otherVoxel] - voxelPositions[i]).normalized);
				vertices[j * 4 + 0] = rot * vertices[j * 4 + 0] + voxelPositions[i];
				vertices[j * 4 + 1] = rot * vertices[j * 4 + 1] + voxelPositions[i];
				vertices[j * 4 + 2] = rot * vertices[j * 4 + 2] + voxelPositions[i];
				vertices[j * 4 + 3] = rot * vertices[j * 4 + 3] + voxelPositions[i];

				uvs[j * 4 + 0] = new Vector2(0.5f, Mathf.Clamp01((float)packetToRender.Edges[0, i][k].Value));
				uvs[j * 4 + 1] = uvs[j * 4 + 0];
				uvs[j * 4 + 2] = uvs[j * 4 + 0];
				uvs[j * 4 + 3] = uvs[j * 4 + 0];

				// push triangles
				// this triangles can be in wrong order. i double checked but i'm not sure.
				tris[j * 12 + 0] = j * 4 + 0; tris[j * 12 + 1] = j * 4 + 2; tris[j * 12 + 2] = j * 4 + 1;
				
				tris[j * 12 + 3] = j * 4 + 0; tris[j * 12 + 4] = j * 4 + 1; tris[j * 12 + 5] = j * 4 + 3;
				
				tris[j * 12 + 6] = j * 4 + 0; tris[j * 12 + 7] = j * 4 + 3; tris[j * 12 + 8] = j * 4 + 2;
				
				tris[j * 12 + 9] = j * 4 + 3; tris[j * 12 + 10] = j * 4 + 1; tris[j * 12 + 11] = j * 4 + 2;
			}
		}

		GameObject go = new GameObject("EdgeNode_final");
		go.transform.parent = GameObject.Find("TargetGameObject").transform;
		Mesh m = go.AddComponent<MeshFilter>().mesh;
		go.AddComponent<MeshRenderer>();
		go.renderer.material.mainTexture = ScaleTexture;

		m.Clear();

		m.vertices = vertices;
		m.uv = uvs;
		m.triangles = tris;
		m.RecalculateNormals();
		m.RecalculateBounds();

		m.Optimize();
	}

    public static string ScreenShotName(int width, int height)
    {
        return string.Format("{0}/screenshots/screen_{1}x{2}_{3}.png",
                             Application.dataPath,
                             width, height,
                             System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
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

        if (Input.GetKeyDown("c"))
        {

            Application.CaptureScreenshot( ScreenShotName( 1024, 768);
            RenderTexture rt = new RenderTexture(camera.targetTexture.width, camera.targetTexture.height, 24);
            camera.targetTexture = rt;
            Texture2D screenShot = new Texture2D(camera.targetTexture.width, camera.targetTexture.height, TextureFormat.RGB24, false);
            camera.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, camera.targetTexture.width, camera.targetTexture.height), 0, 0);
            camera.targetTexture = null;
            RenderTexture.active = null; // JC: added to avoid errors
            Destroy(rt);
            byte[] bytes = screenShot.EncodeToPNG();
            string filename = ScreenShotName(camera.targetTexture.width, camera.targetTexture.height);
            System.IO.File.WriteAllBytes(filename, bytes);
            Debug.Log(string.Format("Took screenshot to: {0}", filename)); ;
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
