using libsimple;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorUnit
{
	class RotateBrain : IProcessor
	{
		private double angleAroundX;
		private double angleAroundY;
		private double angleAroundZ;

		private string[,] arguments;

		/// <summary>
		/// Returns canonical name for the Processor.
		/// </summary>
		/// <returns>string</returns>
		string IProcessor.GetProcessorName()
		{
			return "Rotate Brain";
		}

		/// <summary>
		/// Returns string representation of the type of Processor.
		/// Currently "input" and "process" is recognized.
		/// </summary>
		/// <returns>string, "input" or "process"</returns>
		string IProcessor.GetProcessorType()
		{
			return "process";
		}

		void IProcessor.FromArray(string[] args)
		{
			angleAroundX = Convert.ToDouble(args[0]);
			angleAroundY = Convert.ToDouble(args[1]);
			angleAroundZ = Convert.ToDouble(args[2]);

		}
		/// <summary>
		/// This should return an n-by-2 array
		/// [i,0] name of argument
		/// [i,1] short description
		/// </summary>
		/// <returns>string[i,2]</returns>
		string[,] IProcessor.GetArgs()
		{
			arguments = new string[3, 2];

			arguments[0, 0] = "Rotation around x axis (in degrees): ";// arguments[0, 1] = "degrees around x.";//
			arguments[1, 0] = "Rotation around y axis (in degrees): ";// arguments[1, 1] = "degrees around y.";//
			arguments[2, 0] = "Rotation around z axis (in degrees): ";// arguments[2, 1] = "degrees around z.";//
			return arguments;
		}


		/// <summary>
		/// This should process the given Packet. All calculations should happen here.
		/// </summary>
		/// <param name="p">Input Packet</param>
		/// <returns>New Packet</returns>
		Packet IProcessor.Process(Packet p)
		{
			Packet newPacket = p.Copy();

			double xChange = angleAroundX * Math.PI / 180, yChange = angleAroundY * Math.PI / 180, zChange = angleAroundZ * Math.PI / 180;

			for (int i = 0; i < newPacket.vXYZ.Length; i++)
			{

				//x rotation
				if (xChange != 0)
				{
					float y_old = (float)newPacket.vXYZ[i].y, z_old = (float)newPacket.vXYZ[i].z;
					newPacket.vXYZ[i].x = newPacket.vXYZ[i].x;
					newPacket.vXYZ[i].y = y_old * (float)System.Math.Cos(xChange) - z_old * (float)System.Math.Sin(xChange);
					newPacket.vXYZ[i].z = y_old * (float)System.Math.Sin(xChange) + z_old * (float)System.Math.Cos(xChange);
				}

				//y rotation
				if (yChange != 0)
				{
					float x_old = (float)newPacket.vXYZ[i].x, z_old = (float)newPacket.vXYZ[i].z;
					newPacket.vXYZ[i].x = x_old * (float)System.Math.Cos(yChange) + z_old * (float)System.Math.Sin(yChange);
					newPacket.vXYZ[i].y = newPacket.vXYZ[i].y;
					newPacket.vXYZ[i].z = -1 * x_old * (float)System.Math.Sin(yChange) + z_old * (float)System.Math.Cos(yChange);
				}

				//z rotation
				if (zChange != 0)
				{
					float y_old = (float)newPacket.vXYZ[i].y, x_old = (float)newPacket.vXYZ[i].x;
					newPacket.vXYZ[i].x = x_old * (float)System.Math.Cos(zChange) - y_old * (float)System.Math.Sin(zChange);
					newPacket.vXYZ[i].y = x_old * (float)System.Math.Sin(zChange) + y_old * (float)System.Math.Cos(zChange);
					newPacket.vXYZ[i].z = newPacket.vXYZ[i].z;
				}
			}

			return newPacket;
		}
	}
}
