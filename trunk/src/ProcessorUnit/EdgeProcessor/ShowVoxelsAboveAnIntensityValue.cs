using libsimple;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorUnit
{
    class ShowVoxelsAboveAnIntensityValue : IProcessor
    {
        private string[,] arguments;
        private int percentage_from;
        private int percentage_to;

        /// <summary>
        /// Returns canonical name for the Processor.
        /// </summary>
        /// <returns>string</returns>
        string IProcessor.GetProcessorName()
        {

            return "Hide Voxels Under an Intensity Value";
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

        /// <summary>
        /// This should return an n-by-2 array
        /// [i,0] name of argument
        /// [i,1] short description
        /// </summary>
        /// <returns>string[i,2]</returns>
        string[,] IProcessor.GetArgs()
        {

            arguments = new string[2, 2];
            arguments[0, 0] = "Enter an intensity percentage (%): ";
            arguments[1, 0] = "Enter an intensity percentage (%): ";
            //arguments[2, 1] = "All the voxels which are under the specified intensity percentage will be hidden.";
            return arguments;
        }

        /// <summary>
        /// This method should modify current Processor with given values.
        /// Values follow the order given in GetArgs()
        /// </summary>
        /// <param name="args"></param>
        void IProcessor.FromArray(string[] args)
        {

            if (args.GetLength(0) != 0)
            {

                percentage_from = Convert.ToInt32(args[0]);
                percentage_to = Convert.ToInt32(args[1]);
            }
        }

        /// <summary>
        /// This should process the given Packet. All calculations should happen here.
        /// </summary>
        /// <param name="p">Input Packet</param>
        /// <returns>New Packet</returns>
        Packet IProcessor.Process(Packet p)
        {

            {
                libsimple.Packet newPacket = p.Copy();

                int[] ranks = new int[p.vXYZ.Length];
                int[] ranksY = new int[p.vXYZ.Length];
                int[] ranksZ = new int[p.vXYZ.Length];

                int[] keyMap = new int[newPacket.vXYZ.Length];

                List<libsimple.Packet.Point3D> tmp = new List<libsimple.Packet.Point3D>();

                for (int i = 0, j = 0; i < newPacket.vXYZ.Length; i++)
                {

                    if (p.Intensities[0, i] >= percentage_from && p.Intensities[0, i] <= percentage_to)
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
                            if (p.Intensities[0, j] < percentage_from || p.Intensities[0, j] > percentage_to) continue;
                            List<KeyValuePair<int, double>> tempEdges = new List<KeyValuePair<int, double>>(p.Edges[i, j]);
                            tempEdges.RemoveAll(x => (p.Intensities[0, x.Key] < percentage_from || p.Intensities[0, x.Key] > percentage_to));

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


                return newPacket;

            }

        }

    }
}