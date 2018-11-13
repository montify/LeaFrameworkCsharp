using LeaFramework.Effect;
using LeaFramework.Game.Properties;
using LeaFramework.Graphics;
using LeaFramework.Graphics.VertexStructs;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;

using System.Linq;


namespace LeaFramework.Game.SpriteBatch
{
	public class SpriteBatcher : IDisposable
	{
		private int maxBatchSize;

		public readonly List<SpriteInfo> spriteList;
		public readonly List<RenderBatchInfo> renderBatches;

		private readonly GraphicsDevice graphicsDevice;
		private readonly FontVertex[] fontVertex;
		private readonly VertexBuffer vertexBuffer;
		private readonly LeaSamplerState sampler;
		private LeaEffect effect;
		private Matrix MVP;
		private BlendState bs, bs1;
	


		public SpriteBatcher(GraphicsDevice graphicsDevice, int maxBatchSize)
		{
			this.graphicsDevice = graphicsDevice;
			this.maxBatchSize = maxBatchSize;

			fontVertex = new FontVertex[maxBatchSize];
			spriteList = new List<SpriteInfo>(maxBatchSize);

			renderBatches = new List<RenderBatchInfo>();

			vertexBuffer = new VertexBuffer(graphicsDevice, BufferType.Dynamic);
			vertexBuffer.SetData(fontVertex);

			sampler = new LeaSamplerState();
			sampler.GenerateSamplers(graphicsDevice);

			CreateEffect();
			CreateBlendSates();

			effect.SetVariable("textureAtlasResWidthHeight", "startUp", 512, ShaderType.GeometryShader);
			effect.SetSampler(sampler, 0, ShaderType.PixelShader);

		}

		private void CreateEffect()
		{
			var vertexShaderSource = Resources.vertexShaderFont;
			var geometryShaderSource = Resources.geometryShaderFont;
			var pixelShaderSource = Resources.pixelShaderFont;

			var createInfo = new EffectCreateInfo
			{
				VertexShaderBlob = vertexShaderSource,
				GeometryShaderBlob = geometryShaderSource,
				PixelShaderBlob = pixelShaderSource,

				VertexShaderEntryPoint = "VSMain",
				GeometryShaderEntryPoint = "GSMain",
				PixelShaderEntryPoint = "PSMain"
			};

			effect = new LeaEffect(graphicsDevice, createInfo);
		}

		private void CreateBlendSates()
		{
			var desc1 = new BlendStateDescription();
			desc1.RenderTarget[0].IsBlendEnabled = true;

			desc1.RenderTarget[0].BlendOperation = BlendOperation.Add;
			desc1.RenderTarget[0].AlphaBlendOperation = BlendOperation.Add;

			desc1.RenderTarget[0].SourceBlend = BlendOption.One;
			desc1.RenderTarget[0].SourceAlphaBlend = BlendOption.One;

			desc1.RenderTarget[0].DestinationBlend = BlendOption.InverseSourceAlpha;
			desc1.RenderTarget[0].DestinationAlphaBlend = BlendOption.Zero;
			desc1.RenderTarget[0].RenderTargetWriteMask = ColorWriteMaskFlags.All;

			bs = new BlendState(graphicsDevice.NatiDevice1.D3D11Device, desc1);

			desc1.RenderTarget[0].DestinationBlend = BlendOption.Zero;

			bs1 = new BlendState(graphicsDevice.NatiDevice1.D3D11Device, desc1);

			
		}

	
	
		public void PrepareForRendering()
		{
			MVP = Matrix.OrthoOffCenterLH(0, graphicsDevice.ViewPort.Width, graphicsDevice.ViewPort.Height, 0, 0, 1);
			MVP = Matrix.Transpose(Matrix.Identity * MVP);

			spriteList.Clear();
			renderBatches.Clear();
		}


		public void AddSpriteInfo(SpriteInfo spriteInfo)
		{
			spriteList.Add(spriteInfo);

			if (spriteList.Count >= maxBatchSize)
			{
				Draw();
				PrepareForRendering();
			
				//Array.Clear(fontVertex, 0, fontVertex.Length);
			}

			
		}

		private void CreateRenderBatches()
		{
			if (spriteList.Count == 0)
				return;


			renderBatches.Add(new RenderBatchInfo(spriteList[0].srv, 0, 1));

			fontVertex[0] = new FontVertex(spriteList[0].position, spriteList[0].size, spriteList[0].color, spriteList[0].offset, spriteList[0].isFont);

			int offset = 0;

			offset++;

			for (int i = 1; i < spriteList.Count; i++)
			{
				if (spriteList[i].textureID != spriteList[i - 1].textureID)
					renderBatches.Add(new RenderBatchInfo(spriteList[i].srv, offset, 1));
				else
					renderBatches.Last().numVertices += 1;

				fontVertex[i] = new FontVertex(spriteList[i].position, spriteList[i].size, spriteList[i].color, spriteList[i].offset, spriteList[i].isFont);
				offset++;
			}

			vertexBuffer.UpdateBuffer(fontVertex, 0);
		}

		private void DrawBatches(Matrix MVP)
		{
			graphicsDevice.SetTopology(PrimitiveTopology.PointList);

			graphicsDevice.IsDepthEnable(false);
			graphicsDevice.SetblendState(bs);

			graphicsDevice.SetVertexBuffer(vertexBuffer);

			effect.SetVariable("ProjMatrix", "perFrame", MVP, ShaderType.GeometryShader);

			foreach (var rb in renderBatches)
			{
				effect.SetTexture(rb.texture, 0, ShaderType.PixelShader);

				effect.Apply();
				graphicsDevice.Draw(rb.numVertices, rb.offset);
			}


			graphicsDevice.SetblendState(bs1);
			graphicsDevice.IsDepthEnable(true);
		}

		public void Draw()
		{
			if(spriteList.Count > 0)
			{
				CreateRenderBatches();
				DrawBatches(MVP);
			}
			
		}

		public void Dispose()
		{
			
		}
	}
}
