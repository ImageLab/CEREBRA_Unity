using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libsimple;

namespace ProcessorUnit
{
	class TranslateBrain : IProcessor
	{
		private double translateOnX;
		private double translateOnY;
		private double translateOnZ;

		private string[,] arguments;

		/// <summary>
		/// Returns canonical name for the Processor.
		/// </summary>
		/// <returns>string</returns>
		string IProcessor.GetProcessorName()
		{
			return "Translate Brain";
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
			translateOnX = Convert.ToDouble(args[0]);
			translateOnY = Convert.ToDouble(args[1]);
			translateOnZ = Convert.ToDouble(args[2]);
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

			arguments[0, 0] = "transition units around x axis: ";// arguments[0, 1] = "units around x axis.";//
			arguments[1, 0] = "transition units around y axis: ";// arguments[1, 1] = "units around y axis.";//
			arguments[2, 0] = "transition units around z axis: ";// arguments[2, 1] = "units around z axis.";//
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


			float xChange = (float)translateOnX, yChange = (float)translateOnY, zChange = (float)translateOnZ;
			for (int i = 0; i < newPacket.vXYZ.Length; i++)
			{
				//x translation
				newPacket.vXYZ[i].x = newPacket.vXYZ[i].x + xChange;
				//y translation
				newPacket.vXYZ[i].y = newPacket.vXYZ[i].y + yChange;
				//z translation
				newPacket.vXYZ[i].z = newPacket.vXYZ[i].z + zChange;

			}

			return newPacket;
		}
	}
}
