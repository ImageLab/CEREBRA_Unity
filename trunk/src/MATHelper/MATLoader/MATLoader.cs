using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MATLoader
{
    public class MATLoader: libsimple.IInputProcessor
    {
		private string filename;

		public string GetProcessorName()
		{
			return "MAT File Loader";
		}

		public string GetProcessorType()
		{
			return "input";
		}

		public string[,] GetArgs()
		{
			return new string[,] { { "filename", "[INTERNAL]" } };
		}

		public bool CanReadFrom(string directory)
		{
			if (System.IO.Directory.Exists(directory)) // check for directory
			{
				List<MATHelper.MATVar> varlist = new List<MATHelper.MATVar>();

				// read all .mat files, make a list of variables
				foreach (string file in System.IO.Directory.GetFiles(directory))
				{
					if (file.ToLowerInvariant().EndsWith(".mat"))
					{

						MATHelper.MATFile matfile = new MATHelper.MATFile(file);

						if (matfile.good())
						{
							MATHelper.MATVar v = matfile.GetNextVar();

							while (v != null)
							{
								varlist.Add(v);
								v = matfile.GetNextVar();
							}
						}
					}
				}

				bool hasEdges = varlist.Exists((e) => e.Name == "combined_patterns_tr" || e.Name == "Y_detrend" || e.Name == "tr_data" || e.Name == "trData");
				bool hasIntensities = varlist.Exists((e) => e.Name == "data" || e.Name == "Y_detrend" || e.Name == "tr_data" || e.Name == "trData");
				bool hasXYZ = varlist.Exists((e) => e.Name == "vXYZ" || e.Name == "XYZ");

				return hasEdges && hasIntensities && hasXYZ;
			}
			else
			{
				return false;
			}
		}

		public void FromArray(string[] args)
		{
			filename = args[0];
		}

		public libsimple.Packet Process(libsimple.Packet p)
		{
			libsimple.Packet pck = new libsimple.Packet();

			List<MATHelper.MATVar> varlist = new List<MATHelper.MATVar>();


			foreach (string file in System.IO.Directory.GetFiles(filename))
			{
				if (file.ToLowerInvariant().EndsWith(".mat"))
				{

					MATHelper.MATFile matfile = new MATHelper.MATFile(file);

					if (matfile.good())
					{
						MATHelper.MATVar v = matfile.GetNextVar();

						while (v != null)
						{
							varlist.Add(v);
							v = matfile.GetNextVar();
						}
					}
				}
			}

			MATHelper.MATVar vXYZ = varlist.Find((e) => e.Name == "vXYZ" || e.Name == "XYZ");
			double[,] pos = vXYZ.GetData2D();
            Console.WriteLine("Debug : size of the matrix matloader");
			//Console.WriteLine(pos.GetLength(1));

			double x=0, y=0, z=0;
			if (vXYZ.Dims[0] == 3)
			{
				pck.vXYZ = new libsimple.Packet.Point3D[vXYZ.Dims[1]];
				for (int i = 0; i < vXYZ.Dims[1]; i++)
				{
					x += pos[0, i];
					y += pos[1, i];
					z += pos[2, i];
					pck.vXYZ[i] = new libsimple.Packet.Point3D(pos[0, i], pos[1, i], pos[2, i]);
				}
			}
			else
			{
				pck.vXYZ = new libsimple.Packet.Point3D[vXYZ.Dims[0]];
				for (int i = 0; i < vXYZ.Dims[0]; i++)
				{
					x += pos[i, 0];
					y += pos[i, 1];
					z += pos[i, 2];
					pck.vXYZ[i] = new libsimple.Packet.Point3D(pos[i, 0], pos[i, 1], pos[i, 2]);
				}
			}
			x /= pck.vXYZ.Length; y /= pck.vXYZ.Length; z /= pck.vXYZ.Length;

			// free pos
			pos = null;

			for (int i = 0; i < pck.vXYZ.Length; i++)
			{
				pck.vXYZ[i].x -= (float)x;
				pck.vXYZ[i].y -= (float)y;
				pck.vXYZ[i].z -= (float)z;
			}

			MATHelper.MATVar intensities = varlist.Find((e) => e.Name == "tr_data" || e.Name == "trData");

			if (intensities == null)
			{
				intensities = varlist.Find((e) => e.Name == "data");
			}

			if (intensities == null)
			{
				intensities = varlist.Find((e) => e.Name == "Y_detrend");
			}

			if (intensities == null) {
				//Console.WriteLine("nasil yaaa");
			}

//			Console.WriteLine("intensities");

			if (intensities.Dims[1] == vXYZ.Dims[1])
			{
				pck.Intensities = intensities.GetData2D();
			}
			else
			{
				double[,] ints = intensities.GetData2D();
				int l0 = (int)intensities.Dims[1];
				int l1 = (int)intensities.Dims[0];
				pck.Intensities = new double[l0, l1];

				for (int i = 0; i < l0; i++)
				{
					for (int j = 0; j < l1; j++)
					{
						pck.Intensities[i, j] = ints[j, i];
					}
				}
			}
			intensities = null;

			//Console.WriteLine("intensities done");

			MATHelper.MATVar edges = varlist.Find((e) => e.Name == "combined_patterns_tr");
			MATHelper.MATVar edgeIndices = varlist.Find((e) => e.Name.StartsWith("all_neighbor_indices"));
			MATHelper.MATVar edgeCounts = varlist.Find((e) => e.Name.Contains("ind_tr"));

			//Console.WriteLine(edges != null);
			//Console.WriteLine(edges.Dims[0] + " " + edges.Dims[1]);

			double[,] edgeData = edges.GetData2D();
			//Console.WriteLine("edgeData");
			double[] countsDouble = edgeCounts.GetDataAs1D();
			//Console.WriteLine("countsDouble");
			double[] edgeDestination = null;
			int[] counts = countsDouble.Select((e) => (int)(e+0.5)).ToArray();

			//Console.WriteLine("edges found");

			/* <RANT>
			 * 
			 * i feel like talking.
			 * 
			 * here, we have a little problem. input files use two different formats to keep edges.
			 * i don't quite understand why people do this. so just because someone thought that it
			 * would be cool to save edge destinations as jagged arrays in one file and a continuous 
			 * stream in another, i'm here writing two methods to read edge data.
			 * 
			 * </RANT>
			 */
			MATHelper.MATVar[] nodeData = new MATHelper.MATVar[1]; 
			if (edgeIndices.GetInner().class_type == (uint)MATHelper.Native.MATClassType.MAT_C_CELL)
			{
				edgeDestination = new double[edges.Dims[1]];
				int indexer = 0;
				nodeData = edgeIndices.GetAsCell1D();

				for (int i = 0; i < nodeData.Length; i++)
				{
					double[] edgesToAdd = nodeData[i].GetDataAs1D();
					for (int j = 0; j < edgesToAdd.Length; j++)
					{
						edgeDestination[indexer++] = edgesToAdd[j];
					}
				}
			}
			else
			{
				edgeDestination = edgeIndices.GetDataAs1D();
			}
			int baseIndexer=0;
			int timeSlice=(int)edges.Dims[0];
			
			
			pck.Edges = new KeyValuePair<int, double>[timeSlice, vXYZ.Dims[1]][];


			for (int i = 0; i < vXYZ.Dims[1]; i++) {
				for (int k = 0; k < timeSlice; k++)	{
					pck.Edges[k, i] = new KeyValuePair<int, double>[counts[i]];
					for (int j = 0; j < counts[i]; j++) {
						//Console.WriteLine((int)edgeDestination[baseIndexer + j]);
						pck.Edges[k, i][j] = new KeyValuePair<int, double>(((int)edgeDestination[baseIndexer + j]-1), edgeData[k, baseIndexer + j]);
					}
					//Console.WriteLine();
					//Console.WriteLine(nodeData[i].Dims[0] + " " + nodeData[i].Dims[1]);
					//Console.Read();
				}
				baseIndexer += counts[i];
			}



			return pck;
		}
	}
}
