using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeaFramework.Effect;
using LeaFramework.Graphics;
using LeaFramework.Graphics.VertexStructs;
using LeaFramework.PlayGround.Misc;
using SharpDX;
using SharpDX.Direct3D11;

namespace LeaFramework.PlayGround.TerrainSrc
{
    public class QuadMesh : IDisposable
    {
        private ushort[] indices;
        private Vector3Double[] _vertexSamples;
        private readonly Vector3Double locationRelativeToPlanet;
        private VertexPositionColor[] vertices;
        private readonly QuadNodeExtents extents;
        private int size = Globals.nodeSize;
        private readonly double meshStride;
      
        private readonly QuadRenderer renderer;
        public double distanceFromCamera;
        public double CameraDistanceToWidthRatio;
        private readonly HeightData[] height;
        public Texture2D shadowMap;

        public QuadMesh(QuadNodeExtents extents, GraphicsDevice graphicsDevice)
        {
            this.extents = extents;


            meshStride = extents.Width / (size - 1);
            locationRelativeToPlanet = extents.upVector + extents.uVector * (extents.North + extents.Width * 0.5f) +
                                     extents.vVector * (extents.West + extents.Width * 0.5f);
            locationRelativeToPlanet = locationRelativeToPlanet.ProjectUnitPlaneToUnitSphere() * extents.Radius;

            ComputeShaderHelper CSHelper = new ComputeShaderHelper(graphicsDevice.NatiDevice1.D3D11Device, "Content//PlanetShaders//noiseCs.hlsl");

            HeightData[] data = new HeightData[256 * 256];
            int index = CSHelper.SetData<HeightData>(data);

            CSHelper.Execute(256, 256, 1);
            height = CSHelper.GetData<HeightData>(0);
          
            CreateMesh();
            CollectMeshSamples();

            renderer = new QuadRenderer(vertices, graphicsDevice);

            vertices = null;
            indices = null;
        }

        private void CreateMesh()
        {
            vertices = new VertexPositionColor[size * size];
            var horizontalTextureStep = 1.0f / (size - 1);
            var verticalTextureStep = 1.0f / (size - 1);
            float v = 0;

            for (var row = 0; row < size; row++)
            {
                float u = 0;
                for (var column = 0; column < size; column++)
                {
                    vertices[row * size + column] = DoMesh(row, column);
                  //  vertices[row * size + column].TextureCoordinate = new Vector2(u, v);
                    u += horizontalTextureStep;

                }
                v += verticalTextureStep;
            }
        }

        private VertexPositionColor DoMesh(int row, int column)
        {
            var unitPlanetVector = ConvertToUnitPlaneVector(row, column);
            var unitSphereVector = unitPlanetVector.ProjectUnitPlaneToUnitSphere();

            var planetSpaceVector = ConvertToPlanetSpace(unitPlanetVector, height[row * size + column].Data);
           
            var relativeToCameraVector = ConvertRelativeToCamera(planetSpaceVector);

            return new VertexPositionColor { Position = relativeToCameraVector };
        }

        private Vector3Double ConvertToUnitPlaneVector(int column, int row)
        {
            var uDelta = extents.uVector * (extents.North + row * meshStride);
            var vDelta = extents.vVector * (extents.West + column * meshStride);
            var convertedVector = extents.upVector + uDelta + vDelta;

            return convertedVector;
        }

        private Vector3Double ConvertToPlanetSpace(Vector3Double unitSphereVector, double h)
        {
           // Console.WriteLine(h);
            return unitSphereVector * (extents.Radius + (h*500));
        }

        private Vector3Double ConvertRelativeToCamera(Vector3Double planetSpaceVector)
        {
            return planetSpaceVector - locationRelativeToPlanet;
        }

        private void CollectMeshSamples()
        {
            // We just take a few samples for now
            _vertexSamples = new Vector3Double[]
            {
                vertices[0].Position, // Upper left
                vertices[size/2].Position, // Upper middle
                vertices[size - 1].Position, // Upper right
                vertices[size/2*size].Position, // Middle left
                vertices[vertices.Length/2].Position, // Middle
                vertices[size/2*size + size - 1].Position, // Middle right
                vertices[size*(size - 1)].Position, // Lower left
                vertices[size*(size - 1) + size/2].Position, // Lower middle
                vertices[size*size - 1].Position // Lower right
            };

            //  Move them back into planet - relative space
            for (var x = 0; x < _vertexSamples.Length; x++)
                _vertexSamples[x] += locationRelativeToPlanet;
        }


        private MeshDistance GetDistanceFrom(Vector3Double location)
        {
            var closestDistanceSquared = double.MaxValue;
            var closestVertex = _vertexSamples[0];

            foreach (var vertex in _vertexSamples)
            {
                var distanceSquared = Vector3Double.DistanceSquared(location, vertex);

                if (distanceSquared < closestDistanceSquared)
                {
                    closestDistanceSquared = distanceSquared;
                    closestVertex = vertex;
                }
            }

            return new MeshDistance
            {
                ClosestDistance = Math.Sqrt(closestDistanceSquared),
                ClosestVertex = closestVertex
            };
        }


        public void Update()
        {
            var meshDistance = GetDistanceFrom(Globals.cam.Position);
            distanceFromCamera = meshDistance.ClosestDistance;
            CameraDistanceToWidthRatio = distanceFromCamera / extents.Width * extents.Radius;
        }


        public void Draw(int flag)
        {
            renderer.Draw(locationRelativeToPlanet, flag);
        }

        private struct MeshDistance
        {
            public Vector3Double ClosestVertex { get; set; }
            public double ClosestDistance { get; set; }
        }

        public void Dispose()
        {
            renderer.Dispose();

            _vertexSamples = null;
        }

    }
}
