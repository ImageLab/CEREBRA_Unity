using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libsimple
{
	public interface IProcessor
	{
		/// <summary>
		/// Returns canonical name for the Processor.
		/// </summary>
		/// <returns>string</returns>
		string GetProcessorName();

		/// <summary>
		/// Returns string representation of the type of Processor.
		/// Currently "input" and "process" is recognized.
		/// </summary>
		/// <returns>string, "input" or "process"</returns>
		string GetProcessorType();

		/// <summary>
		/// This should return an n-by-2 array
		/// [i,0] name of argument
		/// [i,1] short description
		/// </summary>
		/// <returns>string[i,2]</returns>
		string[,] GetArgs();

		/// <summary>
		/// This method should modify current Processor with given values.
		/// Values follow the order given in GetArgs()
		/// </summary>
		/// <param name="args"></param>
		void FromArray(string[] args);

		/// <summary>
		/// This should process the given Packet. All calculations should happen here.
		/// </summary>
		/// <param name="p">Input Packet</param>
		/// <returns>New Packet</returns>
		Packet Process(Packet p);

	}
}
