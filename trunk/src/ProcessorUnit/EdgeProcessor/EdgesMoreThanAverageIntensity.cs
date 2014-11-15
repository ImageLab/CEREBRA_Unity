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
    /// It filters out edges that have less arclength values than the average.
    /// </summary>
    public class EdgesMoreThanAverageIntensity : IProcessor
    {
        private string[,] arguments;


        /// <summary>
        /// Returns canonical name for the Processor.
        /// </summary>
        /// <returns>string</returns>
        string IProcessor.GetProcessorName()
        {
            return "Treshold Edges With Avarage Intensity Value";
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
            arguments = new string[0, 0];
           // arguments[0, 0] = "From"; arguments[0, 1] = "min intensity value wanted.";
           // arguments[1, 0] = "To"; arguments[1, 1] = "max intensity value wanted.";
            return arguments;
        }

        void IProcessor.FromArray(string[] args)
        {
          //  startOfTtheRange = Convert.ToDouble(args[0]);
          //  endOfTtheRange = Convert.ToDouble(args[1]);
        }


        private double getAverageOfEdges(Packet newPacket)
        {
            double avg = 0; int totEdge = 0;
            for (int i = 0; i < newPacket.Edges.GetLength(0); i++)
            {
                for (int j = 0; j < newPacket.Edges.GetLength(1); j++)
                {
                    for (int k = 0; k < newPacket.Edges[i, j].Length; k++)
                    {
                        avg += newPacket.Edges[i, j][k].Value; totEdge++;
                    }

                }
            }

            avg /= totEdge;
            return avg;
        }

        /// <summary>
        /// This should process the given Packet. All calculations should happen here.
        /// </summary>
        /// <param name="p">Input Packet</param>
        /// <returns>New Packet</returns>
        Packet IProcessor.Process(Packet p)
        {
            Packet newPacket = p.Copy();
            double avg = getAverageOfEdges(newPacket);
            
            // To be tested
            for (int i = 0; i < newPacket.Edges.GetLength(0); i++)
            {
                for (int j = 0; j < newPacket.Edges.GetLength(1); j++)
                {
                    int numberEdges = newPacket.Edges[i, j].Length;
                    for (int k = 0; k < numberEdges; )
                    {
                        if (newPacket.Edges[i, j][k].Value < avg)
                        {
                            KeyValuePair<int, double>[] newEdges = new KeyValuePair<int, double>[newPacket.Edges[i, j].Length - 1];

                            System.Array.Copy(newPacket.Edges[i, j], 0, newEdges, 0, k);
                            // TODO: check if this is the last element and dont try to copy beyond last!
                            System.Array.Copy(newPacket.Edges[i, j], k + 1, newEdges, k, newPacket.Edges[i, j].Length - k - 1);
                            newPacket.Edges[i, j] = newEdges;
                            numberEdges--;
                        }
                        else k++;
                    }

                }
            }

            return newPacket;
        }

    }
}
