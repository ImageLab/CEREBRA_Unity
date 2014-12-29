using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MATLoader
{
	public class PValueMATLoader : libsimple.IInputProcessor
	{
		private string filename;

		public string GetProcessorName()
		{
			return "MAT File Loader (P-Values)";
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

				bool hasIntensities = varlist.Exists((e) => e.Name == "data" || e.Name == "Y_detrend" || e.Name == "tr_data" || e.Name == "trData");
				bool hasXYZ = varlist.Exists((e) => e.Name == "vXYZ" || e.Name == "XYZ");
				bool hasPValues = varlist.Exists((e) => e.Name == "ind_tr");

				return hasPValues && hasXYZ && (!hasIntensities);
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
            MATHelper.MATVar MNIMatrix = varlist.Find((e) => e.Name == "mni_coordinates");
            double[,] matrix = MNIMatrix.GetData2D();
            Console.WriteLine("Debug : size of the matrix pval");
			//Console.WriteLine(pos.GetLength(1));

			double x = 0, y = 0, z = 0;
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
            for (int i = 0; i < 4; i++)
            {		
                pck.MNITransitionMatrix[i, 0] = matrix[i, 0];		
                pck.MNITransitionMatrix[i, 1] = matrix[i, 1];		
                pck.MNITransitionMatrix[i, 2] = matrix[i, 2];		
                pck.MNITransitionMatrix[i, 3] = matrix[i, 3];		
            }
			// free pos
			pos = null;

			for (int i = 0; i < pck.vXYZ.Length; i++)
			{
				pck.vXYZ[i].x -= (float)x;
				pck.vXYZ[i].y -= (float)y;
				pck.vXYZ[i].z -= (float)z;
			}

			MATHelper.MATVar edgeCounts = varlist.Find((e) => e.Name.Contains("ind_tr"));

			double[] countsDouble = edgeCounts.GetDataAs1D();
			int[] counts = countsDouble.Select((e) => (int)(e + 0.5)).ToArray();

			pck.Intensities = new double[1, counts.Length];

			for (int i = 0; i < counts.Length; i++)
			{
				pck.Intensities[0, i] = counts[i];
			}

			return pck;
		}
	}
}
