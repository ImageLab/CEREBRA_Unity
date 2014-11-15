using libsimple;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorUnit
{
    /// <summary>
    /// This is a filter class. 
    /// It finds n max arclength valued edges and filters out rest.
    /// </summary>
    public class NMaxEdgeProcessor : IProcessor
    {
        private int numberOfEdgesKept;
        private string[,] arguments;

        /// <summary>
        /// Returns canonical name for the Processor.
        /// </summary>
        /// <returns>string</returns>
        string IProcessor.GetProcessorName()
        {
            return "Filter Out Edges With N Max Arclength Value";
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

        string[,] IProcessor.GetArgs()
        {
            arguments = new string[1, 2];
            arguments[0, 0] = "Enter the number of edges kept by each voxel: ";// arguments[0, 1] = "number of edges to keep for each voxel";
            return arguments;
        }

        void IProcessor.FromArray(string[] args)
        {
            numberOfEdgesKept = Convert.ToInt32(args[0]);
        }

        /// <summary>
        /// This should process the given Packet. All calculations should happen here.
        /// </summary>
        /// <param name="p">Input Packet</param>
        /// <returns>New Packet</returns>
        Packet IProcessor.Process(Packet p)
        {
            Packet newPacket = p.Copy();
            
            
            for (int i = 0; i < newPacket.Edges.GetLength(0); i++)
            {
                for (int j = 0; j < newPacket.Edges.GetLength(1); j++)
                {
                    Array.Sort(newPacket.Edges[i, j], delegate(KeyValuePair<int, double> kv1, KeyValuePair<int, double> kv2)
                    {
                        return kv1.Value.CompareTo(kv2.Value);
                    });

                    if (newPacket.Edges[i, j].Length > numberOfEdgesKept)
                    {
                        KeyValuePair<int, double>[] newEdges = new KeyValuePair<int, double>[numberOfEdgesKept];
                        System.Array.Copy(newPacket.Edges[i, j], 0, newEdges, 0, numberOfEdgesKept);
                        newPacket.Edges[i, j] = newEdges;
                    }
                }
            }
            
            return newPacket;
        }

    }
}
