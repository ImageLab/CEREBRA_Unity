using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libsimple
{
	/// <summary>
	/// IInputProcessor inherits IProcessor, adds CanReadFrom method
	/// </summary>
	public interface IInputProcessor : IProcessor
	{
		/// <summary>
		/// Returns whether thi plugin can load given file/directory.
		/// </summary>
		/// <param name="filename">path to file/directory</param>
		/// <returns>bool</returns>
		bool CanReadFrom(string filename);
	}
}
