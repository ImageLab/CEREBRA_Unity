using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace MATHelper
{
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct matvar_t
	{
		public UInt64 nbytes;					/**< Number of bytes for the MAT variable */
		public int rank;						/**< Rank (Number of dimensions) of the data */
		public uint data_type;					/**< Data type(MAT_T_*) */
		public int data_size;					/**< Bytes / element for the data */
		public uint class_type;				/**< Class type in Matlab(MAT_C_DOUBLE, etc) */
		public int isComplex;				/**< non-zero if the data is complex, 0 if real */
		public int isGlobal;					/**< non-zero if the variable is global */
		public int isLogical;					/**< non-zero if the variable is logical */
		public UInt64* dims;						/**< Array of lengths for each dimension */
		[MarshalAs(UnmanagedType.LPStr)]
		public string name;					/**< Name of the variable */
		public void* data;						/**< Pointer to the data */
		public int mem_conserve;				/**< 1 if Memory was conserved with data */
		public uint compression;				/**< Variable compression type */
		public void* internal_data;			/**< matio internal data */
	};

	public static class Native
	{
		public enum MATFileAccess
		{
			MAT_ACC_RDONLY = 0,  /**< @brief Read only file access                */
			MAT_ACC_RDWR = 1   /**< @brief Read/Write file access               */
		};

		public enum MATFileType
		{
			MAT_FT_MAT73 = 0x0200,   /**< @brief Matlab version 7.3 file             */
			MAT_FT_MAT5 = 0x0100,   /**< @brief Matlab level-5 file                 */
			MAT_FT_MAT4 = 0x0010    /**< @brief Version 4 file                      */
		};

		public enum MATDataType
		{
			MAT_T_UNKNOWN = 0,    /**< @brief UNKOWN data type                    */
			MAT_T_INT8 = 1,    /**< @brief 8-bit signed integer data type      */
			MAT_T_UINT8 = 2,    /**< @brief 8-bit unsigned integer data type    */
			MAT_T_INT16 = 3,    /**< @brief 16-bit signed integer data type     */
			MAT_T_UINT16 = 4,    /**< @brief 16-bit unsigned integer data type   */
			MAT_T_INT32 = 5,    /**< @brief 32-bit signed integer data type     */
			MAT_T_UINT32 = 6,    /**< @brief 32-bit unsigned integer data type   */
			MAT_T_SINGLE = 7,    /**< @brief IEEE 754 single precision data type */
			MAT_T_DOUBLE = 9,    /**< @brief IEEE 754 double precision data type */
			MAT_T_INT64 = 12,    /**< @brief 64-bit signed integer data type     */
			MAT_T_UINT64 = 13,    /**< @brief 64-bit unsigned integer data type   */
			MAT_T_MATRIX = 14,    /**< @brief matrix data type                    */
			MAT_T_COMPRESSED = 15,    /**< @brief compressed data type                */
			MAT_T_UTF8 = 16,    /**< @brief 8-bit unicode text data type        */
			MAT_T_UTF16 = 17,    /**< @brief 16-bit unicode text data type       */
			MAT_T_UTF32 = 18,    /**< @brief 32-bit unicode text data type       */

			MAT_T_STRING = 20,    /**< @brief String data type                    */
			MAT_T_CELL = 21,    /**< @brief Cell array data type                */
			MAT_T_STRUCT = 22,    /**< @brief Structure data type                 */
			MAT_T_ARRAY = 23,    /**< @brief Array data type                     */
			MAT_T_FUNCTION = 24     /**< @brief Function data type                  */
		};

		public enum MATClassType
		{
			MAT_C_EMPTY = 0, /**< @brief Empty array                           */
			MAT_C_CELL = 1, /**< @brief Matlab cell array class               */
			MAT_C_STRUCT = 2, /**< @brief Matlab structure class                */
			MAT_C_OBJECT = 3, /**< @brief Matlab object class                   */
			MAT_C_CHAR = 4, /**< @brief Matlab character array class          */
			MAT_C_SPARSE = 5, /**< @brief Matlab sparse array class             */
			MAT_C_DOUBLE = 6, /**< @brief Matlab double-precision class         */
			MAT_C_SINGLE = 7, /**< @brief Matlab single-precision class         */
			MAT_C_INT8 = 8, /**< @brief Matlab signed 8-bit integer class     */
			MAT_C_UINT8 = 9, /**< @brief Matlab unsigned 8-bit integer class   */
			MAT_C_INT16 = 10, /**< @brief Matlab signed 16-bit integer class    */
			MAT_C_UINT16 = 11, /**< @brief Matlab unsigned 16-bit integer class  */
			MAT_C_INT32 = 12, /**< @brief Matlab signed 32-bit integer class    */
			MAT_C_UINT32 = 13, /**< @brief Matlab unsigned 32-bit integer class  */
			MAT_C_INT64 = 14, /**< @brief Matlab unsigned 32-bit integer class  */
			MAT_C_UINT64 = 15, /**< @brief Matlab unsigned 32-bit integer class  */
			MAT_C_FUNCTION = 16 /**< @brief Matlab unsigned 32-bit integer class  */
		};

		public enum MATArrFlags
		{
			MAT_F_COMPLEX = 0x0800, /**< @brief Complex bit flag */
			MAT_F_GLOBAL = 0x0400, /**< @brief Global bit flag */
			MAT_F_LOGICAL = 0x0200, /**< @brief Logical bit flag */
			MAT_F_DONT_COPY_DATA = 0x0001  /**< Don't copy data, use keep the pointer */
		};

		public enum MATCompression
		{
			MAT_COMPRESSION_NONE = 0,   /**< @brief No compression */
			MAT_COMPRESSION_ZLIB = 1    /**< @brief zlib compression */
		};

		public enum MATLookup
		{
			MAT_BY_NAME = 1, /**< Lookup by name */
			MAT_BY_INDEX = 2  /**< Lookup by index */
		};

		/*[DllImport("libmatio", CharSet = CharSet.Ansi)]
		extern public static IntPtr Mat_Open([MarshalAs(UnmanagedType.LPStr)]string filename, int mode);

		[DllImport("libmatio", CharSet = CharSet.Ansi)]
		extern public static IntPtr Mat_VarReadInfo(IntPtr matfile, [MarshalAs(UnmanagedType.LPStr)]string varname);
		
		[DllImport("libmatio", CharSet = CharSet.Ansi)]
		extern public static Int32 Mat_VarReadDataAll(IntPtr matfile, IntPtr varptr);
		*/
		[DllImport("libmatio", CharSet = CharSet.Ansi)]
		extern public static UInt64 Mat_SizeOf(int datatype);

		[DllImport("libmatio", CharSet = CharSet.Ansi)]
		extern public static UInt64 Mat_SizeOfClass(int class_type);

		[DllImport("libmatio", CharSet = CharSet.Ansi)]
		extern public static IntPtr Mat_CreateVer([MarshalAs(UnmanagedType.LPStr)]string matname, [MarshalAs(UnmanagedType.LPStr)]string hdr_str, int mat_file_ver);

		[DllImport("libmatio", CharSet = CharSet.Ansi)]
		extern public static int Mat_Close(IntPtr mat);

		[DllImport("libmatio", CharSet = CharSet.Ansi)]
		extern public static IntPtr Mat_Open([MarshalAs(UnmanagedType.LPStr)]string matname, int mode);

		[DllImport("libmatio", CharSet = CharSet.Ansi)]
		[return: MarshalAs(UnmanagedType.LPStr)]
		extern public static string Mat_GetFilename(IntPtr matfp);

		[DllImport("libmatio", CharSet = CharSet.Ansi)]
		extern public static Int32 Mat_GetVersion(IntPtr matfp);

		[DllImport("libmatio", CharSet = CharSet.Ansi)]
		extern public static int Mat_Rewind(IntPtr mat);

		[DllImport("libmatio", CharSet = CharSet.Ansi)]
		extern public static IntPtr Mat_VarCalloc();

		[DllImport("libmatio", CharSet = CharSet.Ansi)]
		extern unsafe public static IntPtr Mat_VarCreate([MarshalAs(UnmanagedType.LPStr)]string name, Int32 class_type, Int32 data_type, int rank, UInt64* dims, void* data, int opt);

		[DllImport("libmatio", CharSet = CharSet.Ansi)]
		extern unsafe public static IntPtr Mat_VarCreateStruct([MarshalAs(UnmanagedType.LPStr)]string name, int rank, UInt64* dims, [MarshalAs(UnmanagedType.LPArray)]string[] fields, UInt32 nfields);

		[DllImport("libmatio", CharSet = CharSet.Ansi)]
		extern public static int Mat_VarDelete(IntPtr mat, [MarshalAs(UnmanagedType.LPStr)]string name);

		[DllImport("libmatio", CharSet = CharSet.Ansi)]
		extern public static IntPtr Mat_VarDuplicate(IntPtr inp, int opt);

		[DllImport("libmatio", CharSet = CharSet.Ansi)]
		extern public static void Mat_VarFree(IntPtr matvar);

		[DllImport("libmatio", CharSet = CharSet.Ansi)]
		extern public static IntPtr Mat_VarGetCell(IntPtr matvar, int index);

		// TODO: fix this!?
		/*[DllImport("libmatio", CharSet = CharSet.Ansi)]
		extern public static matvar_t **Mat_VarGetCells(IntPtr matvar,int *start,int *stride,
							  int *edge);

		[DllImport("libmatio", CharSet = CharSet.Ansi)]
		extern public static matvar_t **Mat_VarGetCellsLinear(IntPtr matvar,int start,int stride,
							  int edge);*/

		[DllImport("libmatio", CharSet = CharSet.Ansi)]
		extern public static UInt64 Mat_VarGetSize(IntPtr matvar);

		[DllImport("libmatio", CharSet = CharSet.Ansi)]
		extern public static UInt32 Mat_VarGetNumberOfFields(IntPtr matvar);

		[DllImport("libmatio", CharSet = CharSet.Ansi)]
		extern public static int Mat_VarAddStructField(IntPtr matvar, [MarshalAs(UnmanagedType.LPStr)]string fieldname);

		[DllImport("libmatio", CharSet = CharSet.Ansi)]
		[return: MarshalAs(UnmanagedType.LPStr)]
		extern public static string Mat_VarGetStructFieldnames(IntPtr matvar);

		[DllImport("libmatio", CharSet = CharSet.Ansi)]
		extern public static IntPtr Mat_VarGetStructFieldByIndex(IntPtr matvar,
							  UInt64 field_index, UInt64 index);

		[DllImport("libmatio", CharSet = CharSet.Ansi)]
		extern public static IntPtr Mat_VarGetStructFieldByName(IntPtr matvar,
							  [MarshalAs(UnmanagedType.LPStr)]string field_name, UInt64 index);

		[DllImport("libmatio", CharSet = CharSet.Ansi)]
		extern unsafe public static IntPtr Mat_VarGetStructField(IntPtr matvar, void* name_or_index,
							  int opt, int index);

		[DllImport("libmatio", CharSet = CharSet.Ansi)]
		extern unsafe public static IntPtr Mat_VarGetStructs(IntPtr matvar, int* start, int* stride,
							  int* edge, int copy_fields);

		[DllImport("libmatio", CharSet = CharSet.Ansi)]
		extern public static IntPtr Mat_VarGetStructsLinear(IntPtr matvar, int start, int stride,
							  int edge, int copy_fields);

		[DllImport("libmatio", CharSet = CharSet.Ansi)]
		extern public static void Mat_VarPrint(IntPtr matvar, int printdata);

		[DllImport("libmatio", CharSet = CharSet.Ansi)]
		extern public static IntPtr Mat_VarRead(IntPtr mat, [MarshalAs(UnmanagedType.LPStr)]string name);

		[DllImport("libmatio", CharSet = CharSet.Ansi)]
		extern unsafe public static int Mat_VarReadData(IntPtr mat, IntPtr matvar, void* data,
							  int* start, int* stride, int* edge);

		[DllImport("libmatio", CharSet = CharSet.Ansi)]
		extern public static int Mat_VarReadDataAll(IntPtr mat, IntPtr matvar);

		[DllImport("libmatio", CharSet = CharSet.Ansi)]
		extern unsafe public static int Mat_VarReadDataLinear(IntPtr mat, IntPtr matvar, void* data,
							  int start, int stride, int edge);

		[DllImport("libmatio", CharSet = CharSet.Ansi)]
		extern public static IntPtr Mat_VarReadInfo(IntPtr mat, [MarshalAs(UnmanagedType.LPStr)]string name);

		[DllImport("libmatio", CharSet = CharSet.Ansi)]
		extern public static IntPtr Mat_VarReadNext(IntPtr mat);

		[DllImport("libmatio", CharSet = CharSet.Ansi)]
		extern public static IntPtr Mat_VarReadNextInfo(IntPtr mat);

		[DllImport("libmatio", CharSet = CharSet.Ansi)]
		extern public static IntPtr Mat_VarSetCell(IntPtr matvar, int index, IntPtr cell);

		[DllImport("libmatio", CharSet = CharSet.Ansi)]
		extern public static IntPtr Mat_VarSetStructFieldByIndex(IntPtr matvar,
							  UInt64 field_index, UInt64 index, IntPtr field);

		[DllImport("libmatio", CharSet = CharSet.Ansi)]
		extern public static IntPtr Mat_VarSetStructFieldByName(IntPtr matvar,
							  [MarshalAs(UnmanagedType.LPStr)]string field_name, UInt64 index, IntPtr field);

		[DllImport("libmatio", CharSet = CharSet.Ansi)]
		extern public static int Mat_VarWrite(IntPtr mat, IntPtr matvar,
							  Int32 compress);

		[DllImport("libmatio", CharSet = CharSet.Ansi)]
		extern public static int Mat_VarWriteInfo(IntPtr mat, IntPtr matvar);

		[DllImport("libmatio", CharSet = CharSet.Ansi)]
		extern unsafe public static int Mat_VarWriteData(IntPtr mat, IntPtr matvar, void* data,
							  int* start, int* stride, int* edge);

		/* Other functions */

		[DllImport("libmatio", CharSet = CharSet.Ansi)]
		extern unsafe public static int Mat_CalcSingleSubscript(int rank, int* dims, int* subs);

		[DllImport("libmatio", CharSet = CharSet.Ansi)]
		extern unsafe public static int* Mat_CalcSubscripts(int rank, int* dims, int index);

	}
}
