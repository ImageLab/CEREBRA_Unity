﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libsimple
{
	/// <summary>
	/// This class encapsulates brain data.
	/// </summary>
	/// <remarks>
	/// You should only be using this class while writing Processors.
	/// </remarks>
	public class Packet
	{
		/* Side Note:
		 * In C#, one can declare a 2D array in 2 ways. 
		 * First way:	<type>[,]
		 *		this creates a normal C++ 2d array, <type>[][]
		 * Second way:	<type>[][]
		 *		this creates what is known as a *jagged array*. this is
		 *		an array or arrays. C++ equivalent of vector<vector<type> >
		 */

		/// <summary>
		/// Array of Node intensities. Accessed as Intensities[time,node], both 0-based.
		/// </summary>
		public double[,] Intensities;

        /// <summary>
        /// Array of max and min voxel size values.
        /// </summary>
        public double[] VoxelSizeRange;

        public bool potatoProcess;

        /// <summary>
        /// Matrix to apply transformation to MNI Space.
        /// </summary>
        public double[,] MNITransitionMatrix;

		/// <summary>
		/// Array of edges. Edges[time,origin_node][i].Key is the node that origin_node
		/// is connected. Edges[time,origin_node][i].Value is the edge weight.
		/// </summary>
		public KeyValuePair<int, double>[,][] Edges;

        /// <summary>
        /// Array of mapped Edges according to sorted packet by X coordinate.
        /// </summary>
        public int[] EdgeMapX;

        /// <summary>
        /// Array of mapped Edges according to sorted packet by Y coordinate.
        /// </summary>
        public int[] EdgeMapY;

        /// <summary>
        /// Array of mapped Edges according to sorted packet by Z coordinate.
        /// </summary>
        public int[] EdgeMapZ;

		/// <summary>
		/// This is Vector3f in Unity. I(baha) didn't want to link against Unity.
		/// </summary>
		public struct Point3D
		{
			public float x, y, z;

			public Point3D(double _x, double _y, double _z)
			{
				x = (float)_x;
				y = (float)_y;
				z = (float)_z;

			}

			public Point3D(float _x, float _y, float _z)
			{
				x = _x;
				y = _y;
				z = _z;

			}
		}

		/// <summary>
		/// Positions in space.
		/// </summary>
		public Point3D[] vXYZ;

		private Dictionary<string, object> extras;

        public bool hideVoxels;

		/// <summary>
		/// Lets you define an extra data for this Packet object. For example, input file
		/// name can be passed this way.
		/// </summary>
		/// <remarks>
		/// These extras are copied when using Copy or SoftCopy. 
		/// </remarks>
		public void setExtra(string name, object data)
		{
			if (data != null)
				extras[name] = data;
			else
				extras.Remove(name);
		}

		/// <summary>
		/// Lets you define an extra data for this Packet object. For example, input file
		/// name can be passed this way.
		/// </summary>
		/// <remarks>
		/// These extras are copied when using Copy or SoftCopy. 
		/// </remarks>
		public object getExtra(string name, object def = null)
		{
			if (extras.ContainsKey(name))
				return extras[name];
			else
				return def;
		}

		/// <summary>
		/// This method works like getExtra, except it removes the extra requested.
		/// </summary>
		public object useExtra(string name, object def = null)
		{
			if (extras.ContainsKey(name))
			{
				object o = extras[name];
				extras.Remove(name);
				return o;
			}
			else
				return def;
		}

		public Packet()
		{
			Intensities = null;
            VoxelSizeRange = new double[2];
            VoxelSizeRange[0] = 0.3;
            VoxelSizeRange[1] = 0.7;
            MNITransitionMatrix = new double [4,4];
			Edges = null;
            EdgeMapX = null;
            EdgeMapY = null;
            EdgeMapZ = null;
			vXYZ = null;
			extras = new Dictionary<string, object>();
            hideVoxels = false;
		}

		/// <summary>
		/// Method for copying a packet easily. This is a deep copy,
		/// i.e. copy object has different pointers to internal data.
		/// </summary>
		/// <returns>Packet</returns>
		public Packet Copy()
		{
			Packet p = new Packet();

            // Copy extras
			p.extras = new Dictionary<string, object>(extras);

            // Copy intensities

            if (Intensities != null)
			{
				int l0 = Intensities.GetLength(0);
				int l1 = Intensities.GetLength(1);
				p.Intensities = new double[l0, l1];
				for (int i = 0; i < l0; i++)
				{
					for (int j = 0; j < l1; j++)
					{
                        p.Intensities[i, j] = Intensities[i, j];
					}
				}
			}

            // Copy VoxelSizeRange
            if (VoxelSizeRange != null)
            {
                p.VoxelSizeRange[0] = VoxelSizeRange[0];
                p.VoxelSizeRange[1] = VoxelSizeRange[1];
            }

            // Copy EdgeMapX
            if (EdgeMapX != null)
            {
                int l0 = EdgeMapX.GetLength(0);
                p.EdgeMapX = new int[l0];
                for (int i = 0; i < l0; i++)
                {
                    p.EdgeMapX[i] = EdgeMapX[i];
                }
            }

            // Copy EdgeMapY
            if (EdgeMapY != null)
            {
                int l0 = EdgeMapY.GetLength(0);
                p.EdgeMapY = new int[l0];
                for (int i = 0; i < l0; i++)
                {
                    p.EdgeMapY[i] = EdgeMapY[i];
                }
            }

            // Copy EdgeMapZ
            if (EdgeMapZ != null)
            {
                int l0 = EdgeMapZ.GetLength(0);
                p.EdgeMapZ = new int[l0];
                for (int i = 0; i < l0; i++)
                {
                    p.EdgeMapZ[i] = EdgeMapZ[i];
                }
            }

            // Copy edges
			if (Edges != null)
			{
				int l0 = Edges.GetLength(0);
				int l1 = Edges.GetLength(1);
				p.Edges = new KeyValuePair<int, double>[l0, l1][];
				for (int i = 0; i < l0; i++)
				{
					for (int j = 0; j < l1; j++)
					{
						int l2 = Edges[i, j].Length;
						p.Edges[i, j] = new KeyValuePair<int, double>[l2];

						for (int k = 0; k < l2; k++)
						{
							p.Edges[i, j][k] = new KeyValuePair<int, double>(Edges[i, j][k].Key, Edges[i, j][k].Value);
						}
					}
				}
			}

            // Copy voxel coordinates
			if (vXYZ != null)
			{
				int l0 = vXYZ.GetLength(0);
				p.vXYZ = new Point3D[l0];
				for (int i = 0; i < l0; i++)
				{
					p.vXYZ[i] = new Point3D(vXYZ[i].x, vXYZ[i].y, vXYZ[i].z);
				}
			}
            if (MNITransitionMatrix != null)
            {
                
                p.MNITransitionMatrix = new double[4,4];
                
                for (int i = 0; i < 4; i++)
                {

                    p.MNITransitionMatrix[i, 0] = MNITransitionMatrix[i, 0];
                    p.MNITransitionMatrix[i, 1] = MNITransitionMatrix[i, 1];
                    p.MNITransitionMatrix[i, 2] = MNITransitionMatrix[i, 2];
                    p.MNITransitionMatrix[i, 3] = MNITransitionMatrix[i, 3];
                }
            }

            p.hideVoxels = hideVoxels;

			return p;
		}

		/// <summary>
		/// Method for creating a package with only extras copied.
		/// </summary>
		/// <returns>Packet</returns>
		public Packet SoftCopy()
		{
			Packet p = new Packet();

			p.extras = new Dictionary<string, object>(extras);

			return p;
		}
	}
}
