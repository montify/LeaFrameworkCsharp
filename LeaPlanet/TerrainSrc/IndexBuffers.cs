using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeaFramework.Graphics;
using SharpDX.DXGI;

namespace LeaFramework.PlayGround.TerrainSrc
{
    public class IndexBuffers
    {
        public static IndexBuffer[] indexBufferCombinations;
        public static ushort[] indices1;

        private static ushort[] SolveBottom(ushort[] ind)
        {
            int row = (int)Math.Sqrt(ind.Length / 6);
            int start = (row - 1) * (row * 6) + 4;
            int end = row * row * 6;
            for (int i = start; i < end; i += 12)
            {
                ind[i] = ind[i + 6];
                ind[i + 4] = ind[i + 6];
                ind[i + 7] = ind[i + 6];
            }

            return ind;
        }
        private static ushort[] SolveTop(ushort[] ind)
        {
            int start = 1;
            int end = (int)Math.Sqrt(ind.Length * 6);
            for (int x = start; x < end; x += 12)
            {
                ind[x] = ind[x - 1];
                ind[x + 2] = ind[x - 1];
                ind[x + 5] = ind[x - 1];
            }

            return ind;
        }
        private static ushort[] SolveLeft(ushort[] ind)
        {
            int row = (int)Math.Sqrt(ind.Length / 6);
            for (int i = 2; i < row * 6 * row; i += (row * 6 * 2))
            {
                ind[i] = ind[i - 2];
                ind[i + 3] = ind[i - 2];
                ind[i + ((6 * row) - 2)] = ind[i - 2];
            }

            return ind;
        }
        private static ushort[] SolveRight(ushort[] ind)
        {
            int row = (int)Math.Sqrt(ind.Length / 6);
            int start = (row * 6) * 2;
            for (int i = start; i <= ind.Length; i += (row * 6) * 2)
            {
                ind[i - 3] = ind[i - (row * 6) - 3];
                ind[i - 5] = ind[i - (row * 6) - 3];
                ind[i - (row * 6) - 2] = ind[i - (row * 6) - 3];
            }

            return ind;
        }

        public static void CreateIndexBuffers(GraphicsDevice device, int nodeSize)
        {
            if (indexBufferCombinations != null)
                return;

            ushort[] indices = new ushort[(nodeSize - 1) * (nodeSize - 1) * 6];
            int i = 0;
            for (int z = 0; z < nodeSize - 1; z++)
            {
                for (int x = 0; x < nodeSize - 1; x++)
                {
                    ushort upperleft = (ushort)(z * nodeSize + x);
                    ushort upperright = (ushort)(upperleft + 1);
                    ushort lowerleft = (ushort)(upperleft + nodeSize);
                    ushort lowerright = (ushort)(lowerleft + 1);

                    indices[i++] = upperleft;
                    indices[i++] = upperright;
                    indices[i++] = lowerleft;

                    indices[i++] = upperright;
                    indices[i++] = lowerright;
                    indices[i++] = lowerleft;
                }
            }
            indices1 = indices;

            indexBufferCombinations = new IndexBuffer[11];




            indexBufferCombinations[0] = new IndexBuffer(device, Format.R16_UInt);
            indexBufferCombinations[0].SetData(indices);

            ////einzelne seiten gefixt
            //indexBufferCombinations[1] = new IndexBuffer(device, IndexElementSize.SixteenBits, indices.Length, BufferUsage.None);//l
            //indexBufferCombinations[2] = new IndexBuffer(device, IndexElementSize.SixteenBits, indices.Length, BufferUsage.None);//r
            //indexBufferCombinations[4] = new IndexBuffer(device, IndexElementSize.SixteenBits, indices.Length, BufferUsage.None);//u
            //indexBufferCombinations[8] = new IndexBuffer(device, IndexElementSize.SixteenBits, indices.Length, BufferUsage.None);//d

            ////zwei seiten gefixt
            //indexBufferCombinations[5] = new IndexBuffer(device, IndexElementSize.SixteenBits, indices.Length, BufferUsage.None);//lu
            //indexBufferCombinations[9] = new IndexBuffer(device, IndexElementSize.SixteenBits, indices.Length, BufferUsage.None);//ld
            //indexBufferCombinations[6] = new IndexBuffer(device, IndexElementSize.SixteenBits, indices.Length, BufferUsage.None);//ru
            //indexBufferCombinations[10] = new IndexBuffer(device, IndexElementSize.SixteenBits, indices.Length, BufferUsage.None);//rd



            //ushort[] tmp = new ushort[indices.Length];

            ////left
            //indices.CopyTo(tmp, 0);
            //tmp = SolveLeft(tmp);
            //indexBufferCombinations[1].SetData<ushort>(tmp);


            ////LeftUp
            //tmp = SolveBottom(tmp);
            //indexBufferCombinations[5].SetData<ushort>(tmp);

            ////Right
            //indices.CopyTo(tmp, 0);
            //tmp = SolveRight(tmp);
            //indexBufferCombinations[2].SetData<ushort>(tmp);

            ////RightDown
            //tmp = SolveTop(tmp);
            //indexBufferCombinations[10].SetData<ushort>(tmp);

            ////Up
            //indices.CopyTo(tmp, 0);
            //tmp = SolveBottom(tmp);
            //indexBufferCombinations[4].SetData<ushort>(tmp);

            ////RightUp
            //tmp = SolveRight(tmp);
            //indexBufferCombinations[6].SetData<ushort>(tmp);

            ////Down
            //indices.CopyTo(tmp, 0);
            //tmp = SolveTop(tmp);
            //indexBufferCombinations[8].SetData<ushort>(tmp);

            ////LeftDown
            //tmp = SolveLeft(tmp);
            //indexBufferCombinations[9].SetData<ushort>(tmp);
        }

    }
}
