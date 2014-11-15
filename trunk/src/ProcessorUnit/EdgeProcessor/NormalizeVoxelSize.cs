using libsimple;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorUnit
{
    /// <summary>
    /// This is a filter class. 
    /// It normalizes voxel size values 
    /// to the values between user supplied m-n.
    /// </summary>
    public class NormalizeVoxelSize : IProcessor
    {
        private double startOfTtheRange;
        private double endOfTtheRange;

        private string[,] arguments;

        /// <summary>
        /// Returns canonical name for the Processor.
        /// </summary>
        /// <returns>string</returns>
        string IProcessor.GetProcessorName()
        {
            return "Normalize Voxel Size";
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

        /// <summary>
        /// This should return an n-by-2 array
        /// [i,0] name of argument
        /// [i,1] short description
        /// </summary>
        /// <returns>string[i,2]</returns>
        string[,] IProcessor.GetArgs()
        {
            arguments = new string[2, 2];
            arguments[0, 0] = "Enter the desired minimum voxel size in units: ";// arguments[0, 1] = "min size value wanted.";
            arguments[1, 0] = "Enter the desired maximum voxel size in units: ";// arguments[1, 1] = "max size value wanted.";
            return arguments;
        }

        void IProcessor.FromArray(string[] args)
        {
            startOfTtheRange = Convert.ToDouble(args[0]);
            endOfTtheRange = Convert.ToDouble(args[1]);
        }
        /// <summary>
        /// This should process the given Packet. All calculations should happen here.
        /// </summary>
        /// <param name="p">Input Packet</param>
        /// <returns>New Packet</returns>
        Packet IProcessor.Process(Packet p)
        {
            Packet newPacket = p.Copy();
            
            newPacket.VoxelSizeRange[0] = (float)startOfTtheRange;
            newPacket.VoxelSizeRange[1] = (float)endOfTtheRange;

            return newPacket;
        }
    }
}
