using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using LeaFramework.Effect;
using LeaFramework.Graphics;
using LeaFramework.PlayGround.Misc;
using SharpDX;

namespace LeaFramework.PlayGround.TerrainSrc
{
    public class Terrain
    {
        private readonly QuadNode[] rootNodes = new QuadNode[1];
        private GraphicsDevice graphicsDevice;
        private LeaEffect effect;

        public Terrain(GraphicsDevice graphicsDevice)
        {
           this.graphicsDevice = graphicsDevice;
           IndexBuffers.CreateIndexBuffers(graphicsDevice, Globals.nodeSize);

            var createInfo = new EffectCreateInfo
            {
                PixelShaderPath = "Content//PlanetShaders//pixelShader.hlsl",
                VertexShaderPath = "Content//PlanetShaders//vertexShader.hlsl",
                VertexShaderEntryPoint = "VSMain",
                PixelShaderEntryPoint = "PSMain"
            };
            effect = new LeaEffect(graphicsDevice, createInfo);

            Globals.effect = effect;

            rootNodes[0] = new QuadNode(new QuadNodeExtents(6000, -1.0f, 1.0f, -1.0f, 1.0f, Vector3.BackwardLH, Vector3.Right, Vector3Double.Up), this, graphicsDevice, NodeSide.top);
          /*  rootNodes[1] = new QuadNode(new QuadNodeExtents(6000, -1.0f, 1.0f, -1.0f, 1.0f, Vector3.ForwardLH, Vector3.Right, Vector3Double.Down), this, graphicsDevice, NodeSide.bottom);

            rootNodes[2] = new QuadNode(new QuadNodeExtents(6000, -1.0f, 1.0f, -1.0f, 1.0f, Vector3.Down, Vector3.Right, Vector3Double.Forward), this, graphicsDevice, NodeSide.front);
            rootNodes[3] = new QuadNode(new QuadNodeExtents(6000, -1.0f, 1.0f, -1.0f, 1.0f, Vector3.Down, Vector3.Left, Vector3Double.Backward), this, graphicsDevice, NodeSide.back);

            rootNodes[4] = new QuadNode(new QuadNodeExtents(6000, -1.0f, 1.0f, -1.0f, 1.0f, Vector3.Down, Vector3.BackwardLH, Vector3Double.Left), this, graphicsDevice, NodeSide.left);
            rootNodes[5] = new QuadNode(new QuadNodeExtents(6000, -1.0f, 1.0f, -1.0f, 1.0f, Vector3.Down, Vector3.ForwardLH, Vector3Double.Right), this, graphicsDevice, NodeSide.right);
       */ }


        public void Update()
        {
            //foreach (var t in rootNodes)
            //{
            //    t.Update();
            //}
        }

        public void Draw(SpaceCam cam)
        {
            foreach (var t in rootNodes)
            {
                t.Draw();
            }
        }

    }
}
