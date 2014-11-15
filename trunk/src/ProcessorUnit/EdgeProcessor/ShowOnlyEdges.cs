using libsimple;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorUnit
{
    class ShowOnlyEdges : IProcessor
    {
        private string[,] arguments;

        /// <summary>
        /// Returns canonical name for the Processor.
        /// </summary>
        /// <returns>string</returns>
        string IProcessor.GetProcessorName()
        {
            return "Hide Voxels";
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
            arguments[0, 0] = "Shows only edges and hides the voxels.";
            arguments[0, 1] = ""; 
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
            /* packet'a yeni field eklenecek (bool hideVoxels) ve bu field burda set edilecek. Render edilirken bu değere bakıp voxeller cizilecek*/

            newPacket.hideVoxels = true;

            return newPacket;
        }
    }
}
