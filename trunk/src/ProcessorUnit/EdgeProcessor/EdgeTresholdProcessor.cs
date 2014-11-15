using libsimple;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorUnit
{
    class EdgeTresholdProcessor:IProcessor
    {
        private double thresholdValue;
        private string[,] arguments;

        /// <summary>
        /// Returns canonical name for the Processor.
        /// </summary>
        /// <returns>string</returns>
        string IProcessor.GetProcessorName()
        {
            return "Treshold Edges";
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

        void IProcessor.FromArray(string[] args)
        {
            if (args.GetLength(0) != 0)
            {
                thresholdValue = Convert.ToDouble(args[0]);
            }
        }
        /// <summary>
        /// This should return an n-by-2 array
        /// [i,0] name of argument
        /// [i,1] short description
        /// </summary>
        /// <returns>string[i,2]</returns>
        string[,] IProcessor.GetArgs()
        {
            arguments = new string[1, 2];
            arguments[0, 0] = "Enter the treshold value: ";
            arguments[0, 1] = "is the treshold value, if edge's value is less than the treshold it won't be rendered.";
            return arguments;
        }

        /// <summary>
        /// This should process the given Packet. All calculations should happen here.
        /// </summary>
        /// <param name="p">Input Packet</param>
        /// <returns>New Packet</returns>
        Packet IProcessor.Process(Packet p)
        {
            Packet newPacket = p.Copy();

            // To be tested
            for (int i = 0; i < newPacket.Edges.GetLength(0); i++)
            {
                for (int j = 0; j < newPacket.Edges.GetLength(1); j++)
                {
                    for (int k = 0; k < newPacket.Edges[i, j].Length; )
                    {
                        if (newPacket.Edges[i, j][k].Value > thresholdValue)
                        {
                            KeyValuePair<int, double>[] newEdges = new KeyValuePair<int, double>[newPacket.Edges[i, j].Length - 1];

                            System.Array.Copy(newPacket.Edges[i, j], 0, newEdges, 0, k);
                            // TODO: check if this is the last element and dont try to copy beyond last!
                            System.Array.Copy(newPacket.Edges[i, j], k + 1, newEdges, k, newPacket.Edges[i, j].Length - k - 1);
                            newPacket.Edges[i, j] = newEdges;
                        }
                        else k++;
                    }

                }
            }

            return newPacket;
        }
    }
}
