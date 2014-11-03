using libsimple;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorUnit
{
    class ShowVoxelsAboveAnIntensityValue:IProcessor
    {
        private string[,] arguments;
        private int percentage;

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

            arguments = new string[1, 2];
            arguments[0, 0] = "Enter an intensity percentage (%): ";
            arguments[0, 1] = "All the voxels which are under the specified intensity percentage will be hidden.";
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

                percentage = Convert.ToInt32( args[0]);
            }
        }

        /// <summary>
        /// This should process the given Packet. All calculations should happen here.
        /// </summary>
        /// <param name="p">Input Packet</param>
        /// <returns>New Packet</returns>
        Packet IProcessor.Process(Packet p)
        {

            return new Packet();
        }
    }
}
