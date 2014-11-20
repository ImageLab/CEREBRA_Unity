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
    /// It normalizes voxel intensity values 
    /// to the values between user supplied m-n.
    /// </summary>
    public class NormalizeIntensityWithinARange : IProcessor
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
            return "Normalize Intensity Values";
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
            arguments[0, 0] = "Enter the desired minimum intensity value: ";// arguments[0, 1] = "min intensity value wanted.";
            arguments[1, 0] = "Enter the desired maximum intensity value: ";// arguments[1, 1] = "max intensity value wanted.";
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
            
            // Check if Intensities not empty
            if (p.Intensities == null || p.Intensities.GetLength(0) == 0 || p.Intensities.GetLength(1) == 0)
            {
                return p;
            }

            double min = p.Intensities[0, 0], max = p.Intensities[0, 0];

            foreach (double intensity in p.Intensities)
            {
                if (intensity < min) min = intensity;
                if (intensity > max) max = intensity;
            }

            for (int i = 0; i < newPacket.Intensities.GetLength(0); i++)
            {
                for (int j = 0; j < newPacket.Intensities.GetLength(1); j++)
                {
                    newPacket.Intensities[i, j] = ((newPacket.Intensities[i, j] - min) / (max - min)); // 0-1 araligi
                    newPacket.Intensities[i, j] = newPacket.Intensities[i, j] * (endOfTtheRange - startOfTtheRange) + startOfTtheRange; // min - max araligi. User belirleyecek.
                }
            }

            return newPacket;
        }
    }
}
