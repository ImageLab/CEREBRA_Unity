using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;

namespace libsimple
{
	// this class is defined static, so that it doesn't need to be initialized manually or created
	static public class ProcessorManager
	{
		// in C#, one can use Type class and Activator class to dynamically generate an instance
		static private Dictionary<string, Type> processors = new Dictionary<string, Type>();

		static ProcessorManager()
		{
			;
		}

		static public bool QueryProcessor(string name)
		{
			return processors.ContainsKey(name);
		}

		static public string[] GetRegisteredProcessors()
		{
			return processors.Keys.ToArray();
		}

		static public IProcessor GetProcessorInstance(string name)
		{
			return (IProcessor)Activator.CreateInstance(processors[name]);
		}

		static public IProcessor[] GetReadersFor(string filename)
		{
			List<IProcessor> canRead = new List<IProcessor>();
			string[] args = new string[] { filename };

			foreach (Type tProc in processors.Values)
			{
				IProcessor processor = (IProcessor)Activator.CreateInstance(tProc);
				if (processor.GetProcessorType() == "input")
				{
					IInputProcessor inputProcessor = (IInputProcessor)processor;
					if (inputProcessor.CanReadFrom(filename))
					{
						inputProcessor.FromArray(args);
						canRead.Add(inputProcessor);
					}
				}
			}

			return canRead.ToArray();
		}

		static public bool Register(IProcessor p)
		{
			if (processors.ContainsKey(p.GetProcessorName()))
			{
				return false;
			}
			else
			{
				processors[p.GetProcessorName()] = p.GetType();
				return true;
			}
		}

		static public IProcessor FromArray(string[] values)
		{
			Debug.Assert(values.GetLength(0) > 0, "Processor construction request with no name.");
			Debug.Assert(processors.ContainsKey(values[0]), "Given Processor name(" + values[0] + ") is not registered.");

			// copy parameters
			string[] args = new string[values.GetLength(0) - 1];

			for (int i = 1; i < values.GetLength(0); i++)
			{
				args[i - 1] = values[i];
			}

			// create Processor using type information
			IProcessor proc = (IProcessor)Activator.CreateInstance(processors[values[0]]);

			// initialize Processor
			proc.FromArray(args);

			return proc;
		}
	}
}
