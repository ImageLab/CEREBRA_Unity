using libsimple;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorUnit
{
    class SpanningTree : IProcessor
    {
        private string[,] arguments;
        private bool treeType = false;

        class Vertex
        {
            public Vertex(int index)
            {
                this.index = index;
            }
            public int index;

            public override bool Equals(object obj)
            {
                return index == ((Vertex)obj).index;
            }
            public override int GetHashCode()
            {
                return index;
            }

            public override string ToString()
            {
                return index.ToString();
            }
        }

        class Edge
        {
            public Edge(double weight, Vertex v1, Vertex v2)
            {
                this.weight = weight;
                this.v1 = v1;
                this.v2 = v2;
            }
            public double weight;
            public Vertex v1;
            public Vertex v2;

            public override bool Equals(object obj)
            {
                Edge e2 = (Edge)obj;
                return (this.v1.Equals(e2.v1) && this.v2.Equals(e2.v2)) ||
                    (this.v2.Equals(e2.v1) && this.v1.Equals(e2.v2));
            }

            public static int compareByWeight(Edge e1, Edge e2)
            {
                if (e1.weight < e2.weight)
                    return -1;
                else if (e1.weight > e2.weight)
                    return 1;
                else
                    return 0;
            }
        }

        class Tree
        {
            public List<Vertex> vertices = new List<Vertex>();
            public List<Edge> edges = new List<Edge>();
            public static Tree Merge(Tree t1, Tree t2, Edge e)
            {
                Tree newTree = new Tree();
                newTree.vertices.AddRange(t1.vertices);
                newTree.vertices.AddRange(t2.vertices);
                newTree.edges.AddRange(t1.edges);
                newTree.edges.AddRange(t2.edges);
                newTree.edges.Add(e);
                return newTree;
            }
        }

        class Forest
        {
            public List<Tree> trees = new List<Tree>();
        }

        static List<Vertex> createVertices(int n)
        {
            List<Vertex> v = new List<Vertex>();
            for (int i = 0; i < n; i++)
            {
                v.Add(new Vertex(i));
            }
            return v;
        }

        static Packet makePacketFromForest(Packet p, Forest F)
        {
            Packet newPacket = p.SoftCopy();

            newPacket.Intensities = new double[1,p.Intensities.GetLength(1)];
            // Copy intensities
			for (int i = 0; i < p.Intensities.GetLength(1); i++)
			{
                newPacket.Intensities[0, i] = p.Intensities[0, i];
			}

            newPacket.vXYZ = new Packet.Point3D[p.vXYZ.Length];
            // Copy voxel coordinates
            for (int i = 0; i < p.vXYZ.GetLength(0); i++)
			{
				newPacket.vXYZ[i] = new Packet.Point3D(p.vXYZ[i].x, p.vXYZ[i].y, p.vXYZ[i].z);
			}

            // Make edges.
            // First initialize sizes
            newPacket.Edges = new KeyValuePair<int,double>[1,p.vXYZ.Length][];
            List<KeyValuePair<int, double>>[] edges = new List<KeyValuePair<int, double>>[p.vXYZ.Length];
            for (int i = 0; i < p.vXYZ.Length; i++)
            {
                edges[i] = new List<KeyValuePair<int, double>>();
            }

            for (int i = 0; i<F.trees.Count ; i++)
            {
                foreach (Edge edge in F.trees[i].edges)
                {
                    edges[edge.v1.index].Add(new KeyValuePair<int, double>(edge.v2.index,edge.weight)); 
                }
            }

            for (int i = 0; i<p.vXYZ.Length ; i++) 
            {
                newPacket.Edges[0,i] = edges[i].ToArray();
            }
            
            return newPacket;
        }

        Packet solve(Packet p)
        {
            // create a forest F where each vertex in the graph is a separate tree.
            Forest F = new Forest();

            //create a list of vertices
            List<Vertex> vertices = createVertices(p.vXYZ.Length);

            //each vertex is a tree in a forest
            foreach (Vertex v in vertices)
            {
                Tree t1 = new Tree();
                t1.vertices.Add(v);
                F.trees.Add(t1);
            }

            //create a list of all edges
            List<Edge> S = new List<Edge>();

            for (int i = 0; i < p.vXYZ.Length; i++)
            {
                Vertex v1 = vertices.Find(x => x.index == i);

                for (int j = 0; j < p.Edges[0, i].Length; j++)
                {
                    //find vertex to the other end
                    Vertex v2 = vertices.Find(x => x.index == p.Edges[0, i][j].Key);

                    Edge edge = null;
                    //create the edge
                    if (treeType == false) // Min Spanning Tree
                        edge = new Edge(p.Edges[0, i][j].Value, v1, v2);
                    else if (treeType == true)
                        edge = new Edge((-1)*(p.Edges[0, i][j]).Value, v1, v2);

                    //try to find the edge in the list
                    Edge temp = S.Find(x => x.Equals(edge));

                    //if not found, add the add to the list
                    if (temp == null)
                        S.Add(edge);
                    else
                    {
                        // for vertices a,b there are 2 edges: ab, ba. Choose one with the maximum weight.
                        if (temp.weight < edge.weight)
                        {
                            temp.weight = edge.weight;
                        }
                    }
                }
            }

            //sort them by weight. smallest weight first
            S.Sort(Edge.compareByWeight);

            int count = 0;
            while (S.Count > 0)
            {
                //get the first edge from the list
                Edge e = S[count];
                //remove the edge
                S.Remove(e);

                //find the trees that contains the vertices in edge e
                Tree t1 = F.trees.Find(x => x.vertices.Find(y => y.Equals(e.v1)) != null);
                Tree t2 = F.trees.Find(x => x.vertices.Find(y => y.Equals(e.v2)) != null);

                //if the 2 trees found are the same, ignore
                if (t1 == t2)
                    continue;

                //Merge the two tress together by creating a new tree
                //then adding all the vertices and edges including
                //edge e
                Tree tFinal = Tree.Merge(t1, t2, e);

                //remove the 2 trees from the forest and add the new merged tree
                F.trees.Remove(t1);
                F.trees.Remove(t2);
                F.trees.Add(tFinal);
            }

            return makePacketFromForest(p, F);
        }



        /// <summary>
        /// Returns canonical name for the Processor.
        /// </summary>
        /// <returns>string</returns>
        string IProcessor.GetProcessorName()
        {
            return "Spanning Tree Filter (Minimum/Maximum)";
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
            arguments[0, 0] = "Please enter the spanning tree type (Min or Max): ";// arguments[0, 1] = "Spanning Tree Type (Min/Max)";
            return arguments;
        }

        /// <summary>
        /// This method should modify current Processor with given values.
        /// Values follow the order given in GetArgs()
        /// </summary>
        /// <param name="args"></param>
        void IProcessor.FromArray(string[] args)
        {
            if (args[0].Equals("Min"))
                treeType = false;
            else if (args[0].Equals("Max"))
                treeType = true;
        }

        /// <summary>
        /// This should process the given Packet. All calculations should happen here.
        /// </summary>
        /// <param name="p">Input Packet</param>
        /// <returns>New Packet</returns>
        Packet IProcessor.Process(Packet p)
        {
            return solve(p);
        }
    }
}
