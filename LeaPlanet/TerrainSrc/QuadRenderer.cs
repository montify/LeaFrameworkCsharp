using LeaFramework.Graphics;
using System;
using LeaFramework.Effect;
using LeaFramework.Graphics.VertexStructs;
using LeaFramework.PlayGround.Misc;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;

namespace LeaFramework.PlayGround.TerrainSrc
{
    public class QuadRenderer : IDisposable
    {
        private GraphicsDevice graphicsDevice;
        private VertexBuffer vBuffer;
     
        public QuadRenderer(VertexPositionColor[] vertices, GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;

            CreateBuffers(vertices);
        }

        private void CreateBuffers(VertexPositionColor[] vertices)
        {
            vBuffer = new VertexBuffer(graphicsDevice, BufferUsage.Normal);
            vBuffer.SetData(vertices);
        }

        public void Draw(Vector3Double location, int flag)
        {
            graphicsDevice.SetTopology(PrimitiveTopology.TriangleList);
            graphicsDevice.SetIndexBuffer(IndexBuffers.indexBufferCombinations[0], 0);
            graphicsDevice.SetVertexBuffer(vBuffer);

            Globals.effect.SetVariable("World", "perFrameBuffer", GetWorldMatrix(location, Globals.cam.Position), ShaderType.VertexShader);
            Globals.effect.SetVariable("View", "perFrameBuffer", Globals.cam.View, ShaderType.VertexShader);
            Globals.effect.SetVariable("Projection", "perFrameBuffer", Globals.cam.Projection, ShaderType.VertexShader);

            Globals.effect.Apply();
            graphicsDevice.DrawIndexed(IndexBuffers.indices1.Length, 0,0);
        }

        Matrix GetWorldMatrix(Vector3Double location, Vector3Double cameraLocation)
        {
            // The mesh stored in the vertex buffer is centered at the origin in order to take it easy on
            // the float number system.  The camera view matrix is also generated as though the camera were
            // at the origin.  In order to correctly render the mesh we translate it away from the origin
            // by the same vector that the mesh (in double space) is displaced from the camera (in double space).

            // We also translate and scale distant meshes to bring them inside the far clipping plane.  For
            // every mesh that's further than the start of the scaled space, we calcuate a new distance
            // using an exponential downscale function to make it fall in the view frustum.  We also scale
            // it down proportionally so that it appears perspective-wise to be identical to the original
            // location.  See the Interactive Visualization paper, page 24.

            Matrix scaleMatrix;
            Matrix translationMatrix;

            var locationRelativeToCamera = location - cameraLocation;
            var distanceFromCamera = locationRelativeToCamera.Length();
            var unscaledViewSpace = Globals.FarClippingPlaneDistance * 0.25;

            if (distanceFromCamera > unscaledViewSpace)
            {
                var scaledViewSpace = Globals.FarClippingPlaneDistance - unscaledViewSpace;
                double scaledDistanceFromCamera = unscaledViewSpace + (scaledViewSpace * (1.0 - Math.Exp((scaledViewSpace - distanceFromCamera) / 1000000000)));
                Vector3Double scaledLocationRelativeToCamera = Vector3Double.Normalize(locationRelativeToCamera) * scaledDistanceFromCamera;

                scaleMatrix = Matrix.Scaling((float)(scaledDistanceFromCamera / distanceFromCamera));
                translationMatrix = Matrix.Translation(scaledLocationRelativeToCamera);
            }
            else
            {
                scaleMatrix = Matrix.Identity;
                translationMatrix = Matrix.Translation(locationRelativeToCamera);
            }

            return scaleMatrix * translationMatrix;
        }

        public void Dispose()
        {
            vBuffer.Dispose();

        }
    }
}
