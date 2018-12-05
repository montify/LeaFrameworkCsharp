using System;
using LeaFramework.Effect;
using LeaFramework.Game;
using LeaFramework.Graphics;
using LeaFramework.Graphics.VertexStructs;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.DXGI;

namespace PlayGround
{
	public class Triangle : IDisposable
	{
		VertexBuffer vertexBuffer;
		IndexBuffer indexBuffer;
		GraphicsDevice graphicsDevice;
		public LeaEffect effect;
		Matrix viewProj;
		private Matrix world;
		float rotVal;
		
		uint[] indices;
		private float colorIntensity;

		public Triangle(GraphicsDevice graphicsDevice)
		{
			this.graphicsDevice = graphicsDevice;
			
			var createInfo = new EffectCreateInfo
			{
				PixelShaderPath = "Content//TriangleShaders//pixelShader.hlsl",
				VertexShaderPath = "Content//TriangleShaders//vertexShader.hlsl",
				VertexShaderEntryPoint = "VSMain",
				PixelShaderEntryPoint = "PSMain"
			};


			effect = new LeaEffect(graphicsDevice, createInfo);

			SetUp();

		}

		private void SetUp()
		{
			var vertices = new VertexPositionColor[3];

			vertices[0].Position = new Vector3(-1.0f, 1.0f, 1.0f);
			vertices[1].Position = new Vector3(0.0f, 1.0f, 1.0f);
			vertices[2].Position = new Vector3(-1.0f, -1.0f, 1.0f);
		

			vertices[0].Color = Color.Orange.ToVector4();
			vertices[1].Color = Color.Orange.ToVector4();
			vertices[2].Color = Color.Orange.ToVector4();
		

			indices = new uint[] 
			{
				0, 1, 2
			};
		
			vertexBuffer = new VertexBuffer(graphicsDevice, BufferUsage.Normal);
			vertexBuffer.SetData(vertices);


			indexBuffer = new IndexBuffer(graphicsDevice, Format.R32_UInt, "XDTT");
			indexBuffer.SetData(indices);

			
		}

		public void Update(GameTimer gameTime, Vector3 position)
		{
			world = Matrix.Translation(position);

			var proj = Matrix.PerspectiveFovLH((float)Math.PI / 3f, (float)graphicsDevice.ViewPort.Width / (float)graphicsDevice.ViewPort.Height, 0.5f, 100f);

			var view = Matrix.LookAtLH(new Vector3(0, 0, -5), Vector3.Zero, Vector3.UnitY);
			

			viewProj = world * view * proj;

			//	Scale * Rotation * translation
		
			viewProj = Matrix.Transpose(viewProj);
		}

		public void Render(Vector3 color)
		{
			graphicsDevice.SetTopology(PrimitiveTopology.TriangleList);

			graphicsDevice.SetVertexBuffer(vertexBuffer);
			graphicsDevice.SetIndexBuffer(indexBuffer, 0);
		
			effect.SetVariable("ViewProj", "TestX", viewProj, ShaderType.VertexShader);
			effect.Apply();

			graphicsDevice.DrawIndexed(indices.Length, 0, 0);
			
		}

		public void Dispose()
		{
			Utilities.Dispose(ref vertexBuffer);
			Utilities.Dispose(ref indexBuffer);
			Utilities.Dispose(ref effect);
		}
	}
}
