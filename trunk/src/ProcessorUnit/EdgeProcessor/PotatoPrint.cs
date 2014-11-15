using libsimple;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ProcessorUnit
{
    class PotatoPrint:IProcessor
    {
        private string[,] arguments;
        private string sliceAxis;
        private double sliceNumber;

        /// <summary>
        /// Returns canonical name for the Processor.
        /// </summary>
        /// <returns></returns>
        string IProcessor.GetProcessorName()
        {
            return "Potato Print";
        }

        /// <summary>
        /// Returns string representation of the type of Processor.
        /// Currently "input" and "process" is recognized.
        /// </summary>
        /// <returns>string, "process"</returns>
        string IProcessor.GetProcessorType()
        {
            return "process";
        }
        string[,] IProcessor.GetArgs()
        {
            arguments = new string[2, 2];

            arguments[0, 0] = "Slice Axis: ";// arguments[0, 1] = "degrees around x.";//
            arguments[1, 0] = "Slice Number: ";// arguments[1, 1] = "degrees around y.";//

            return arguments;
        }
        void IProcessor.FromArray(string[] args)
        {
            sliceAxis = args[0];
            sliceNumber = Convert.ToInt16(args[1]);
        }
        Packet IProcessor.Process(Packet p)
        {
            libsimple.Packet newPacket = p.Copy();

            int[] ranks = new int[p.vXYZ.Length];
            int[] ranksY = new int[p.vXYZ.Length];
            int[] ranksZ = new int[p.vXYZ.Length];
            float minX, maxX;
            float minY, maxY;
            float minZ, maxZ;
            minX = p.vXYZ[0].x;
            maxX = p.vXYZ[0].x;
            minY = p.vXYZ[0].y;
            maxY = p.vXYZ[0].y;
            minZ = p.vXYZ[0].z;
            maxZ = p.vXYZ[0].z;
            for (int i = 0; i < p.vXYZ.Length; i++) { ranks[i] = -1; ranksY[i] = -1; ranksZ[i] = -1; }
            for (int i = 0; i < p.vXYZ.Length; i++)
            {
                if (p.vXYZ[i].x < minX) minX = p.vXYZ[i].x;
                if (p.vXYZ[i].x > maxX) maxX = p.vXYZ[i].x;

                if (p.vXYZ[i].y < minY) minY = p.vXYZ[i].y;
                if (p.vXYZ[i].y > maxY) maxY = p.vXYZ[i].y;

                if (p.vXYZ[i].z < minZ) minZ = p.vXYZ[i].z;
                if (p.vXYZ[i].z > maxZ) maxZ = p.vXYZ[i].z;
            }

            /*avgX = (maxX + minX) / 2;
            avgY = (maxY + minY) / 2;
            avgZ = (maxZ + minZ) / 2;*/
            for (int i = 0; i < p.vXYZ.Length; i++)
            {
                    ranks[i] = (int)( p.vXYZ[i].x-minX);
                //--y
                    ranksY[i] = (int)(p.vXYZ[i].y-minY);

                    ranksZ[i] = (int)(p.vXYZ[i].z-minZ);
            }

            p.setExtra("layerRanks", ranks);
            p.setExtra("layerRanksY", ranksY);
            p.setExtra("layerRanksZ", ranksZ);

            if (sliceAxis == "X") 
            {
             

                int[] keyMap = new int[newPacket.vXYZ.Length];

                List<libsimple.Packet.Point3D> tmp = new List<libsimple.Packet.Point3D>();

                for (int i = 0, j = 0; i < newPacket.vXYZ.Length; i++)
                {

                    if (ranks[i] == sliceNumber)
                    {

                        tmp.Add(newPacket.vXYZ[i]);
                        keyMap[i] = j;
                        j++;
                    }
                    else
                    {
                        keyMap[i] = -1;
                    }
                }
                newPacket.vXYZ = new libsimple.Packet.Point3D[tmp.Count];
                for (int o = 0; o < tmp.Count; o++) newPacket.vXYZ[o] = tmp[o];

                if (p.Edges != null)
                {

                    newPacket.Edges = new KeyValuePair<int, double>[newPacket.Edges.GetLength(0), tmp.Count][];
                    for (int i = 0; i < newPacket.Edges.GetLength(0); i++)
                    {
                        for (int j = 0, k = 0; j < p.Edges.GetLength(1); j++)
                        {
                            if (ranks[j] > sliceNumber || ranks[j]<sliceNumber) continue;
                            List<KeyValuePair<int, double>> tempEdges = new List<KeyValuePair<int, double>>(p.Edges[i, j]);
                            tempEdges.RemoveAll(x => (ranks[x.Key] > sliceNumber || ranks[x.Key] < sliceNumber));

                            for (int l = 0; l < tempEdges.Count; l++)
                            {
                                tempEdges[l] = new KeyValuePair<int, double>(keyMap[tempEdges[l].Key], tempEdges[l].Value);
                            }

                            newPacket.Edges[i, k] = new KeyValuePair<int, double>[tempEdges.Count];
                            newPacket.Edges[i, k] = tempEdges.ToArray();
                            k++;
                        }
                    }
                }
            }
            else if (sliceAxis == "Y")
            {
                int[] keyMap = new int[newPacket.vXYZ.Length];

                List<libsimple.Packet.Point3D> tmp = new List<libsimple.Packet.Point3D>();

                for (int i = 0, j = 0; i < newPacket.vXYZ.Length; i++)
                {

                    if (ranksY[i] == sliceNumber)
                    {

                        tmp.Add(newPacket.vXYZ[i]);
                        keyMap[i] = j;
                        j++;
                    }
                    else
                    {
                        keyMap[i] = -1;
                    }
                }
                newPacket.vXYZ = new libsimple.Packet.Point3D[tmp.Count];
                for (int o = 0; o < tmp.Count; o++) newPacket.vXYZ[o] = tmp[o];

                if (p.Edges != null)
                {

                    newPacket.Edges = new KeyValuePair<int, double>[newPacket.Edges.GetLength(0), tmp.Count][];
                    for (int i = 0; i < newPacket.Edges.GetLength(0); i++)
                    {
                        for (int j = 0, k = 0; j < p.Edges.GetLength(1); j++)
                        {
                            if (ranksY[j] > sliceNumber || ranksY[j] < sliceNumber) continue;
                            List<KeyValuePair<int, double>> tempEdges = new List<KeyValuePair<int, double>>(p.Edges[i, j]);
                            tempEdges.RemoveAll(y => (ranksY[y.Key] > sliceNumber || ranksY[y.Key] < sliceNumber));

                            for (int l = 0; l < tempEdges.Count; l++)
                            {
                                tempEdges[l] = new KeyValuePair<int, double>(keyMap[tempEdges[l].Key], tempEdges[l].Value);
                            }

                            newPacket.Edges[i, k] = new KeyValuePair<int, double>[tempEdges.Count];
                            newPacket.Edges[i, k] = tempEdges.ToArray();
                            k++;
                        }
                    }
                }
            }
            else if (sliceAxis == "Z")
            {
                int[] keyMap = new int[newPacket.vXYZ.Length];

                List<libsimple.Packet.Point3D> tmp = new List<libsimple.Packet.Point3D>();

                for (int i = 0, j = 0; i < newPacket.vXYZ.Length; i++)
                {

                    if (ranksZ[i] == sliceNumber)
                    {

                        tmp.Add(newPacket.vXYZ[i]);
                        keyMap[i] = j;
                        j++;
                    }
                    else
                    {
                        keyMap[i] = -1;
                    }
                }
                newPacket.vXYZ = new libsimple.Packet.Point3D[tmp.Count];
                for (int o = 0; o < tmp.Count; o++) newPacket.vXYZ[o] = tmp[o];

                if (p.Edges != null)
                {

                    newPacket.Edges = new KeyValuePair<int, double>[newPacket.Edges.GetLength(0), tmp.Count][];
                    for (int i = 0; i < newPacket.Edges.GetLength(0); i++)
                    {
                        for (int j = 0, k = 0; j < p.Edges.GetLength(1); j++)
                        {
                            if (ranksZ[j] > sliceNumber || ranksZ[j] < sliceNumber) continue;
                            List<KeyValuePair<int, double>> tempEdges = new List<KeyValuePair<int, double>>(p.Edges[i, j]);
                            tempEdges.RemoveAll(z => (ranksZ[z.Key] > sliceNumber || ranksZ[z.Key] < sliceNumber));

                            for (int l = 0; l < tempEdges.Count; l++)
                            {
                                tempEdges[l] = new KeyValuePair<int, double>(keyMap[tempEdges[l].Key], tempEdges[l].Value);
                            }

                            newPacket.Edges[i, k] = new KeyValuePair<int, double>[tempEdges.Count];
                            newPacket.Edges[i, k] = tempEdges.ToArray();
                            k++;
                        }
                    }
                }
            }
            return newPacket;
        }
    }
}
