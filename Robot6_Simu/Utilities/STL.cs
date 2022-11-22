using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robot6_Simu.Utilities
{
    public class STL
    {
        public float[][,] vertex { get; set; }
        public float[][] normal { get; set; }
        public System.Drawing.Color Color { get; set; }
        public string Name { get; set; }
        public long TriangleInt { get; set; }
        public float[] ParallelVertex { get; set; }
        public bool LoadSTL_Binary(string filename)
        {
            BinaryReader br = null;
            Stream aaa = File.Open(filename, FileMode.Open);
            br = new BinaryReader(aaa);
            byte[] data = new byte[80];
            data = br.ReadBytes(80); // read the header
            TriangleInt = br.ReadUInt32();
            vertex = new float[TriangleInt][,];
            normal = new float[TriangleInt][];

            for (long c = 0; c < TriangleInt; c++)
            {
                normal[c] = new float[3];
                normal[c][0] = br.ReadSingle();
                normal[c][1] = br.ReadSingle();
                normal[c][2] = br.ReadSingle();

                vertex[c] = new float[3, 3];
                for (int pc = 0; pc < 3; pc++) //iterate through the points
                {
                    for (int pn = 0; pn < 3; pn++)
                    {
                        vertex[c][pc, pn] = br.ReadSingle();
                    }

                }
                uint attr = br.ReadUInt16(); // not used attribute             
            }

            br.Close();
            return true;
        }
        public bool Tranlate(float x, float y)
        {
            System.Threading.Tasks.Parallel.For(0, TriangleInt, new ParallelOptions { MaxDegreeOfParallelism = 4 }, c =>
            {

                for (int pc = 0; pc < 3; pc++) //iterate through the points
                {
                    vertex[c][pc, 0] += x;
                    vertex[c][pc, 1] += y;
                }

            });
            return true;
        }

    }
}