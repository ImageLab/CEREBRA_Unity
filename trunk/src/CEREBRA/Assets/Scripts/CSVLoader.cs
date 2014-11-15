using UnityEngine;
using libsimple;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class CSVLoader : libsimple.IInputProcessor {
	private string directoryName;
	private libsimple.Packet glblPckt;

	public bool CanReadFrom(string filename)
	{
		List<string> files = System.IO.Directory.GetFiles(filename).ToList();

		return files.Exists((e) => e.EndsWith("voxels.txt")) && files.Exists((e) => e.EndsWith("intensity.txt"))
			&& files.Exists((e) => e.EndsWith("neighborhood.txt")) && files.Exists((e) => e.EndsWith("neighbors.txt"))
			&& files.Exists((e) => e.EndsWith("arclengths.txt"));
	}

	public void FromArray(string[] args)
	{
		directoryName = args[0];
	}

	public string[,] GetArgs()
	{
		return new string[, ] { { "filename", "[INTERNAL]" } };
	}

	public string GetProcessorName()
	{
		return "CSV Loader";
	}

	public string GetProcessorType()
	{
		return "input";
	}

	private void importVoxelsFromTextFile(string position_filepath,
										 string intensity_filepath)
	{
		List<Vector3> posList = new List<Vector3>();
		List<double> intensityList= new List<double>();

		Vector3 centerOfMass = new Vector3(0.0f, 0.0f, 0.0f);

		// TODO: Some error-check while reading file would be nice
		using (FileStream position_fs = File.Open(position_filepath,
												  FileMode.Open,
												  FileAccess.Read,
												  FileShare.Read))
		using (BufferedStream position_bs = new BufferedStream(position_fs))
		using (StreamReader position_sr = new StreamReader(position_bs))
		using (FileStream intensity_fs = File.Open(intensity_filepath,
												   FileMode.Open,
												   FileAccess.Read,
												   FileShare.Read))
		using (BufferedStream intensity_bs = new BufferedStream(intensity_fs))
		using (StreamReader intensity_sr = new StreamReader(intensity_bs))
		{
			string position_line, intensity_line;
			while ((position_line = position_sr.ReadLine()) != null
				  && (intensity_line = intensity_sr.ReadLine()) != null)
			{
				string[] bits = position_line.Split(' ');
				int x = int.Parse(bits[0]);
				int y = int.Parse(bits[1]);
				int z = int.Parse(bits[2]);
				float intensity = float.Parse(intensity_line);

				posList.Add(new Vector3(x, y, z));
				intensityList.Add(intensity);

				centerOfMass += new Vector3(x, y, z);
			}

			centerOfMass = centerOfMass / intensityList.Count;

			glblPckt.vXYZ = new libsimple.Packet.Point3D[posList.Count];
			glblPckt.Intensities = new double[1, posList.Count];
			for (int i = 0; i < posList.Count; i++) {
				glblPckt.vXYZ[i] = new libsimple.Packet.Point3D(posList[i].x - centerOfMass.x, posList[i].y - centerOfMass.y, posList[i].z - centerOfMass.z);
				glblPckt.Intensities[0, i] = intensityList[i];
			}

			//alignCenterWithOrigin(centerOfMass);
		}
	}

	private void importArcsFromTextFile(string p_filepath,
									   string terminal_voxels_filepath,
									   string arclengths_filepath)
	{
		// TODO: Some error-check while reading file would be nice
		using (FileStream p_fs = File.Open(p_filepath,
										   FileMode.Open,
										   FileAccess.Read,
										   FileShare.Read))
		using (BufferedStream p_bs = new BufferedStream(p_fs))
		using (StreamReader p_sr = new StreamReader(p_bs))
		using (FileStream terminal_voxels_fs = File.Open(terminal_voxels_filepath,
														 FileMode.Open,
														 FileAccess.Read,
														 FileShare.Read))
		using (BufferedStream terminal_voxels_bs = new BufferedStream(terminal_voxels_fs))
		using (StreamReader terminal_voxels_sr = new StreamReader(terminal_voxels_bs))
		using (FileStream arclengths_fs = File.Open(arclengths_filepath,
													FileMode.Open,
													FileAccess.Read,
													FileShare.Read))
		using (BufferedStream arclengths_bs = new BufferedStream(arclengths_fs))
		using (StreamReader arclengths_sr = new StreamReader(arclengths_bs))
		{
			glblPckt.Edges = new KeyValuePair<int, double>[1, glblPckt.vXYZ.Length][];
			string p_line, terminal_voxels_line, arclengths_line;
			for (int lineNumber = 0;
				 (p_line = p_sr.ReadLine()) != null
				  && (terminal_voxels_line = terminal_voxels_sr.ReadLine()) != null;
				 lineNumber++)
			{
				int p = int.Parse(p_line);
				string[] terminal_voxels = terminal_voxels_line.Split(' ');
				glblPckt.Edges[0, lineNumber] = new KeyValuePair<int, double>[p];

				for (int i = 0; i < p; i++)
				{
					arclengths_line = arclengths_sr.ReadLine();
					int terminal_voxel = int.Parse(terminal_voxels[i]) - 1;
					glblPckt.Edges[0, lineNumber][i] = new KeyValuePair<int, double>(terminal_voxel, float.Parse(arclengths_line));
				}
			}
		}
	}

	public libsimple.Packet Process(libsimple.Packet p)
	{
		glblPckt = new libsimple.Packet();

		importVoxelsFromTextFile(Path.Combine(directoryName, "voxels.txt"), Path.Combine(directoryName, "intensity.txt"));
		importArcsFromTextFile(Path.Combine(directoryName, "neighborhood.txt"), Path.Combine(directoryName, "neighbors.txt"), Path.Combine(directoryName, "arclengths.txt"));

		return glblPckt;
	}
}
