using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libsimple;

namespace ConvexHullProcessor
{
    public class ConvexHullProcessor : IProcessor
    {
        private string[,] arguments;

		public string GetProcessorName() {
			return "Convex Hull Finder";
		}

		public string[,] GetArgs()
		{
			arguments = new string[1, 2];
            arguments[0, 0] = "This filter connects the outer voxels in the model unless there is no region information. If the region information exists, then it connects the outer voxels in every region seperately.";
            return arguments;
		}

		public string GetProcessorType()
		{
			return "process";
		}

		public void FromArray(string[] arguments)
		{
			return;
		}

		private Tuple<libsimple.Packet.Point3D[], int[][]> findConvexHullFor(libsimple.Packet.Point3D[] inVertices)
		{
			Packet.Point3D[] points;
			int[][] vertices;

			DefaultVertex[] dv = new DefaultVertex[inVertices.Length];

			for (int i = 0; i < inVertices.Length; i++)
			{
				dv[i] = new DefaultVertex();
				dv[i].Position = new double[3] { inVertices[i].x, inVertices[i].y, inVertices[i].z };
			}

			var convexHull = ConvexHull.Create(dv);

			if (convexHull == null)
			{
				return Tuple.Create<libsimple.Packet.Point3D[], int[][]>(null, null); ;
			}

			List<DefaultVertex> dv2 = convexHull.Points.ToList();

			points = new Packet.Point3D[dv2.Count];

			for (int i = 0; i < dv2.Count; i++)
			{
				points[i] = new Packet.Point3D(dv2[i].Position[0], dv2[i].Position[1], dv2[i].Position[2]);
			}

			DefaultConvexFace<DefaultVertex>[] faces = convexHull.Faces.ToArray();

			vertices = new int[faces.Length][];

			for (int i = 0; i < vertices.Length; i++)
			{
				vertices[i] = new int[faces[i].Vertices.Length];
				for (int j = 0; j < faces[i].Vertices.Length; j++)
					vertices[i][j] = dv2.IndexOf(faces[i].Vertices[j]);
			}

			return Tuple.Create(points, vertices);
		}

		public Packet Process(Packet incoming)
		{
			Packet outgoing = incoming.Copy();
			Packet.Point3D[][] points;
			int[][][] vertices;
			if (incoming.getExtra("inputGroup") == null)
			{
				var convexHullResult = findConvexHullFor(incoming.vXYZ);
				points= new Packet.Point3D[1][];
				vertices = new int[1][][];
				points[0] = convexHullResult.First;
				vertices[0] = convexHullResult.Second;
			}
			else
			{
				int[] inputGroups = ((int[])incoming.getExtra("inputGroup"));

				int numInputGroups = inputGroups.Max()+1;

				points = new Packet.Point3D[numInputGroups][];
				vertices = new int[numInputGroups][][];

				for (int i = 0; i < numInputGroups; i++)
				{
					List<Packet.Point3D> groupPoints = new List<Packet.Point3D>();

					for (int j = 0; j < inputGroups.Length; j++)
					{
						if (inputGroups[j] == i)
						{
							groupPoints.Add(incoming.vXYZ[j]);
						}
					}

					var convexHullResult = findConvexHullFor(groupPoints.ToArray());
					points[i] = convexHullResult.First;
					vertices[i] = convexHullResult.Second;
				}
			}

			outgoing.setExtra("convexHullVertices", points);
			outgoing.setExtra("convexHullFaces", vertices);
			outgoing.setExtra("useConvexHullRenderer", true);

			return outgoing;
		}
    }
}
