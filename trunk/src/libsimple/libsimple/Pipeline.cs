using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace libsimple
{
	public class Pipeline
	{
		private List<IProcessor> processors;

		/// <summary>
		/// default constructor
		/// </summary>
		public Pipeline()
		{
			processors = new List<IProcessor>();
		}

		/// <summary>
		/// Adds a Processor with given information
		/// </summary>
		/// <param name="args">args[0] should contain Processor name. args[1..n] is for parameters</param>
		public void AddProcessor(string[] args)
		{
			processors.Add(ProcessorManager.FromArray(args));
		}

		private string[] parseLine()
		{
			return null;
		}

		/// <summary>
		/// Used when loading a Pipeline from file
		/// </summary>
		/// <param name="args">array of lines in the file. each line contains a processor</param>
		public void FromFileStringArray(string[] args)
		{
			// TODO: implement this
			throw new NotImplementedException();
		}

		/// <summary>
		/// Used to construct a Pipeline from UI.
		/// </summary>
		/// <param name="args">args[n] contain Processors. args[i][0] contain name and rest is parameters. see IProcessor.FromArray</param>
		public void FromArray(string[][] args)
		{
			processors.Clear();
			for (int i = 0; i < args.Length; i++)
			{
				processors.Add(ProcessorManager.FromArray(args[i]));
			}
		}

		/// <summary>
		/// Runs given Processor chain
		/// </summary>
		/// <returns>a Packet instance for Unity3D to draw</returns>
		public Packet Run()
		{
			Packet data = new Packet();
			foreach (IProcessor processor in processors)
			{
				data = processor.Process(data);
			}
            data.EdgeMapX = new int[data.vXYZ.Length];
            data.EdgeMapY = new int[data.vXYZ.Length];
            data.EdgeMapZ = new int[data.vXYZ.Length];
            for (int i = 0; i < data.vXYZ.Length; i++)
            {
                data.EdgeMapX[i] = i;
                data.EdgeMapY[i] = i;
                data.EdgeMapZ[i] = i;
            }
            return data;
		}
	}
}
