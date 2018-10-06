using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using LeaFramework.Effect;
using LeaFramework.Game.Properties;
using LeaFramework.Graphics;
using LeaFramework.Graphics.VertexStructs;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;

namespace LeaFramework.Game.SpriteBatch
{
	public class SpriteBatch : IDisposable
	{
		private GraphicsDevice graphicsDevice;
		private Matrix MVP;
		private VertexBuffer vertexBuffer;
		private LeaSamplerState sampler;
		private LeaEffect effect;
		private SpriteBatchVertex[] vertices;
		private ShaderResourceView currenTexture;
		private ShaderResourceView previousTexture;

		private List<SpriteInfo> spriteList;
		
		public SpriteBatch(GraphicsDevice graphicsDevice, int maxBatchSize = 1024)
		{
			this.graphicsDevice = graphicsDevice;
			spriteList = new List<SpriteInfo>();
			var vertexShaderSource = Resources.vertexShader;
			var geometryShaderSource = Resources.geometryShader;
			var pixelShaderSource = Resources.pixelShader;

			var createInfo = new EffectCreateInfo
			{
				VertexShaderBlob = vertexShaderSource,
				GeometryShaderBlob  = geometryShaderSource,
				PixelShaderBlob = pixelShaderSource,

				VertexShaderEntryPoint = "VSMain",
				GeometryShaderEntryPoint = "GSMain",
				PixelShaderEntryPoint = "PSMain"
			};

			sampler = new LeaSamplerState();
			sampler.GenerateSamplers(graphicsDevice);

			effect = new LeaEffect(graphicsDevice, createInfo);
			vertexBuffer = new VertexBuffer(graphicsDevice, true);
			
			vertices = new SpriteBatchVertex[maxBatchSize];
		}

	

		public void Begin()
		{
			// Should Calc. MVP here? When yes - when window is maximized the sprites dont change :(
			MVP = Matrix.OrthoOffCenterLH(0, graphicsDevice.ViewPort.Width, graphicsDevice.ViewPort.Height, 0, 0, 1);
			MVP = Matrix.Transpose(MVP);
		}
		
		public void Submit(ShaderResourceView tex, Vector2 position, Vector2 size, Vector4 color)
		{
			var spriteInfo = new SpriteInfo
			{
				textureID = tex.GetHashCode(), color = color, position = position, size = size, srv = tex
			};

			spriteList.Add(spriteInfo);
		}


		public void End()
		{
			

			for (int i = 0; i < spriteList.Count; i++)
			{
				currenTexture = spriteList[i].srv;

				if (currenTexture != previousTexture)
				{
					vertices[i].Position = spriteList[i].position;
					vertices[i].Color = spriteList[i].color;
					vertices[i].Size = spriteList[i].size;
					
				}
				else
				{
					//RenderBatch(currenTexture, vertices);
					previousTexture = currenTexture;
				}

			}


		}

		private void RenderBatch(ShaderResourceView currentTexture, SpriteInfo sprite)
		{


			vertexBuffer.CreateAndSetData(vertices);

			effect.SetTexture(currentTexture, 0, ShaderType.PixelShader);
			effect.SetVariable("ProjMatrix", "perFrame", MVP, ShaderType.GeometryShader);
			effect.SetSampler(sampler, 0, ShaderType.PixelShader);

			effect.Apply();

			graphicsDevice.Draw(spriteList.Count, 0);

			spriteList.Clear();;
		}

		public void Dispose()
		{
			effect.Dispose();
			sampler.Dispose();
			vertexBuffer.Dispose();
		}
	}
}
