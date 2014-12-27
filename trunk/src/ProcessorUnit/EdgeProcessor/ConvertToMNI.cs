using libsimple;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorUnit
{
    class ConvertToMNI : IProcessor
    {
    
        private string[,] arguments;

        /// <summary>
        /// Returns canonical name for the Processor.
        /// </summary>
        /// <returns>string</returns>
        string IProcessor.GetProcessorName()
        {
            return "MNI Converter";
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
            arguments[0, 0] = "Choose the .mat file that contains transition matrix !: ";
            arguments[0, 1] = "";
            return arguments;
        }

        public float[] matrixMultiplication(float[] voxel, float[,] transitionMatrix)
        {
            float[] result = voxel;

            result[0] = voxel[0] * transitionMatrix[0, 0] + voxel[1] * transitionMatrix[1, 0] + voxel[2] * transitionMatrix[2, 0];
            result[1] = voxel[0] * transitionMatrix[0, 1] + voxel[1] * transitionMatrix[1, 1] + voxel[2] * transitionMatrix[2, 1];
            result[2] = voxel[0] * transitionMatrix[0, 2] + voxel[1] * transitionMatrix[1, 2] + voxel[2] * transitionMatrix[2, 2];
            return result;
        }
        public float[,] matrixTranspose(double[,] transitionMatrix)
        {
            float[,] result = new float[4,4];
            result[0, 0] = (float)transitionMatrix[0, 0];
            result[0, 1] = (float)transitionMatrix[1, 0];
            result[0, 2] = (float)transitionMatrix[2, 0];
            result[0, 3] = (float)transitionMatrix[3, 0];

            result[1, 1] = (float)transitionMatrix[1, 1];
            result[1, 0] = (float)transitionMatrix[0, 1];
            result[1, 2] = (float)transitionMatrix[2, 1];
            result[1, 3] = (float)transitionMatrix[3, 1];

            result[2, 2] = (float)transitionMatrix[2, 2];
            result[2, 0] = (float)transitionMatrix[0, 2];
            result[2, 1] = (float)transitionMatrix[1, 2];
            result[2, 3] = (float)transitionMatrix[3, 2];

            result[3, 3] = (float)transitionMatrix[3, 3];
            result[3, 0] = (float)transitionMatrix[0, 3];
            result[3, 1] = (float)transitionMatrix[1, 3];
            result[3, 2] = (float)transitionMatrix[2, 3];
            
            return result;
        }
        /// <summary>
        /// This should process the given Packet. All calculations should happen here.
        /// </summary>
        /// <param name="p">Input Packet</param>
        /// <returns>New Packet</returns>
        Packet IProcessor.Process(Packet p)
        {

            Packet newPacket = p.Copy();
            
            float[,] matrix = matrixTranspose(p.MNITransitionMatrix);
            //float[,] matrix = matrixTranspose(temp);
            //TODO: apply transition.
            float[] mnicoord;
            float[] coord = new float[3];
            // Console.Write("In MNI debug:");
            for (int i = 0; i < newPacket.vXYZ.Length; i++ )
            {
                coord[0] = newPacket.vXYZ[i].x;
                coord[1] = newPacket.vXYZ[i].y;
                coord[2] = newPacket.vXYZ[i].z;
                // Console.WriteLine("{0} {1} {2}",newPacket.vXYZ[i].x , newPacket.vXYZ[i].y , newPacket.vXYZ[i].z);
                mnicoord = matrixMultiplication(coord, matrix);
                newPacket.vXYZ[i].x = mnicoord[0];
                newPacket.vXYZ[i].y = mnicoord[1];
                newPacket.vXYZ[i].z = mnicoord[2];

                //newPacket.vXYZ
            }
                
            
            return newPacket;
        }
    }
}
