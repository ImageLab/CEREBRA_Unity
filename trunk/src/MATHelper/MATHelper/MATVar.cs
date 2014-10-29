using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace MATHelper
{
	public class MATVar
	{
		private IntPtr handle;
		private MATFile ownerFile;
		private bool dirty;
		private matvar_t inner;
		private long[] _Dims;
		private bool dontFree;

		private void ensureLoaded()
		{
			if (dirty)
			{
				inner = (matvar_t)Marshal.PtrToStructure(handle, typeof(matvar_t));
				dirty = false;
				_Dims = new long[inner.rank];
				unsafe
				{
					for (int i = 0; i < inner.rank; i++)
					{
						_Dims[i] = (long)inner.dims[i];
					}
				}
			}
		}

		public MATVar(IntPtr varhandle, MATFile owner, bool dontFree=false)
		{
			handle = varhandle;
			ownerFile = owner;
			this.dontFree = dontFree;
			dirty = true;
		}

		~MATVar()
		{
			if(!this.dontFree)
				Native.Mat_VarFree(handle);
		}

		public string Name
		{
			get
			{
				ensureLoaded();
				return inner.name;
			}
			set
			{
				inner.name = value;
			}
		}

		public int Rank
		{
			get
			{
				ensureLoaded();
				return inner.rank;
			}
		}

		public long[] Dims
		{
			get
			{
				ensureLoaded();
				return _Dims;
			}
		}

		public matvar_t GetInner()
		{
			return this.inner;
		}

		/*public dynamic GetData()
		{
			Native.Mat_VarReadDataAll(fhandle, handle);
			dirty = true;
			ensureLoaded();

			dynamic arr = Array.CreateInstance(typeof(double), Dims);
			//Buffer.BlockCopy(inner.data, 0, arr, 0, 100);

			return arr;
		}*/

		public double[,] GetData2D()
		{
			if (Rank != 2)
			{
				throw new ArgumentOutOfRangeException("Rank", "is not 2.");
			}
			else
			{
				double[,] arr = new double[Dims[0], Dims[1]];
				unsafe
				{
					//Console.WriteLine("Entered unsafe area");
					if (inner.data == null)
					{
						//Console.WriteLine("Trying to read all data");
						Native.Mat_VarReadDataAll(ownerFile.GetHandle(), handle);
						//Console.WriteLine("Read all data");
						dirty = true;
						ensureLoaded();
					}

					double* dp = (double*)inner.data;
					//Console.WriteLine((UInt64)dp);
					for (int i = 0; i < Dims[0]; i++)
					{
						for (int j = 0; j < Dims[1]; j++)
						{
							arr[i, j] = dp[i + Dims[0] * j];
						}
						//Console.Write(i+" ");
					}
				}

				return arr;
			}
		}

		public MATVar[,] GetAsCell2D()
		{
			if (inner.class_type == (uint)Native.MATClassType.MAT_C_CELL && Rank == 2)
			{
				MATVar[,] arr = new MATVar[Dims[0], Dims[1]];

				unsafe
				{
					if (inner.data == null)
					{
						//Console.WriteLine("Loading data...");
						Native.Mat_VarReadDataAll(ownerFile.GetHandle(), handle);
						dirty = true;
						ensureLoaded();
					}

					long* dp = (long*)inner.data;
					for (int i = 0; i < Dims[0]; i++)
					{

						for (int j = 0; j < Dims[1]; j++)
						{
							arr[i, j] = new MATVar(new IntPtr(dp[i + Dims[0] * j]), ownerFile);
						}
					}
				}

				return arr;
			}
			else
			{
				Console.WriteLine("Not compatible");
				Console.WriteLine(inner.data_type);
				Console.WriteLine(inner.class_type);
			}

			return null;
		}

		public double[] GetDataAs1D()
		{
			if (Rank != 2)
			{
				throw new ArgumentOutOfRangeException("Rank", "is not 2.");
			}
			else
			{
				double[] arr = new double[Dims[0] * Dims[1]];
				unsafe
				{
					if (inner.data == null)
					{
						Native.Mat_VarReadDataAll(ownerFile.GetHandle(), handle);
						dirty = true;
						ensureLoaded();
					}

					double* dp = (double*)inner.data;
					int indexer = 0;
					for (int i = 0; i < Dims[0]; i++)
					{
						for (int j = 0; j < Dims[1]; j++)
						{
							arr[indexer++] = dp[i + Dims[0] * j];
						}
					}
				}

				return arr;
			}
		}

		public MATVar[] GetAsCell1D()
		{
			if (inner.class_type == (uint)Native.MATClassType.MAT_C_CELL && Rank == 2)
			{
				MATVar[] arr = new MATVar[Dims[0] * Dims[1]];

				unsafe
				{
					if (inner.data == null)
					{
						//Console.WriteLine("Loading data...");
						Native.Mat_VarReadDataAll(ownerFile.GetHandle(), handle);
						dirty = true;
						ensureLoaded();
					}

					long* dp = (long*)inner.data;
					int indexer = 0;
					for (int i = 0; i < Dims[0]; i++)
					{

						for (int j = 0; j < Dims[1]; j++)
						{
							arr[indexer++] = new MATVar(new IntPtr(dp[i + Dims[0] * j]), ownerFile,true);
						}
					}
				}

				return arr;
			}
			else
			{
				Console.WriteLine("Not compatible");
				Console.WriteLine(inner.data_type);
				Console.WriteLine(inner.class_type);
			}

			return null;
		}
	}
}
