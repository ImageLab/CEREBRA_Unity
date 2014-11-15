using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MATHelper
{
	public class MATFile
	{
		private IntPtr handle;

		public MATFile(string s)
		{
			handle = Native.Mat_Open(s, 0);
		}

		~MATFile()
		{
			if (handle.ToInt64() != 0)
			{
				Native.Mat_Close(handle);
			}
		}

		public MATFile()
		{
			handle = IntPtr.Zero;
		}

		public bool Open(string s)
		{
			handle = Native.Mat_Open(s, 0);
			return this.good();
		}

		public bool good()
		{
			return handle.ToInt64() != 0;
		}

		public string GetFilename()
		{
			if (this.good())
				return Native.Mat_GetFilename(handle);
			else
				throw new System.ArgumentNullException("this", "instance not initialized");
		}

		public int Rewind()
		{
			if (this.good())
				return Native.Mat_Rewind(handle);
			else
				throw new System.ArgumentNullException("this", "instance not initialized");
		}

		public bool Contains(string name)
		{
			if (this.good())
				return Native.Mat_VarReadInfo(handle, name).ToInt64() != 0;
			else
				throw new System.ArgumentNullException("this", "instance not initialized");
		}

		public MATVar GetVar(string name)
		{
			return new MATVar(Native.Mat_VarReadInfo(handle, name), this);
		}

		public MATVar GetNextVar()
		{
			IntPtr p = Native.Mat_VarReadNextInfo(handle);

			if (p.ToInt64() == 0)
				return null;
			else
				return new MATVar(p, this);
		}

		public IntPtr GetHandle() {
			return this.handle;
		}
	}
}
