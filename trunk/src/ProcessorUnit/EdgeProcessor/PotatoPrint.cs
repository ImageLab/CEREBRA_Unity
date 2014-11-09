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
            return arguments;
        }
        void IProcessor.FromArray(string[] args)
        { 
            
        }
        Packet IProcessor.Process(Packet p)
        {
            Packet newPacket = p.Copy();
            return newPacket;
        }
    }
}
