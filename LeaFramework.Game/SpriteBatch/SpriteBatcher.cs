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

		//public  List<SpriteInfo> spriteList;
		
		public readonly List<RenderBatchInfo> renderBatches;
		public readonly List<SpriteInfo> spriteList;
		private readonly GraphicsDevice graphicsDevice;
		private readonly FontVertex[] fontVertex;
		private readonly VertexBuffer vertexBuffer;
		private readonly LeaSamplerState sampler;
		private LeaEffect effect;
		public Matrix ScaleMatrix { get; set; }
		private Matrix MVP;
		private BlendState bs, bs1;
		private ShaderResourceView oldTexture, currentTexture;
		bool once;
		

		public SpriteBatcher(GraphicsDevice graphicsDevice, int maxBatchSize)
		{
			this.graphicsDevice = graphicsDevice;
			this.maxBatchSize = maxBatchSize;

			spriteList = new List<SpriteInfo>();
			fontVertex = new FontVertex[maxBatchSize];
		
			renderBatches = new List<RenderBatchInfo>();

			vertexBuffer = new VertexBuffer(graphicsDevice, BufferUsage.Dynamic);
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
			MVP = Matrix.Transpose(ScaleMatrix * MVP);

			spriteList.Clear();
			renderBatches.Clear();
		}

		public void AddSpriteInfo(SpriteInfo spriteInfo, ref int ptr)
		{
			spriteList.Add(spriteInfo);

			if (spriteList.Count >= maxBatchSize)
			{
				Draw();
				PrepareForRendering();
				ptr = 0;
			}
		}

		private void CreateRenderBatches()
		{
			//if (spriteList.Count == 0)
			//	return;

			//spriteList = spriteList.OrderBy(o => o.srv.GetHashCode()).ToList();

			renderBatches.Add(new RenderBatchInfo(spriteList[0].srv, 0, 1));

			fontVertex[0] = new FontVertex(spriteList[0].position, spriteList[0].size, spriteList[0].color, spriteList[0].offset);

			int offset = 1;

			for (int i = 1; i < spriteList.Count; i++)
			{
				if (spriteList[i].srv == null)
					break;

				var currentSprite = spriteList[i];

				if (currentSprite.textureHashCode != spriteList[i - 1].textureHashCode)
					renderBatches.Add(new RenderBatchInfo(currentSprite.srv, offset, 1));
				else
					renderBatches.Last().numVertices += 1;
				
				fontVertex[i].Position = currentSprite.position;
				fontVertex[i].Size = currentSprite.size;
				fontVertex[i].Color = currentSprite.color;
				fontVertex[i].Offset = currentSprite.offset;
				
				offset++;
			}
				vertexBuffer.UpdateBuffer(fontVertex, 0);
		}

		private void DrawBatches()
		{

			foreach (var rb in renderBatches)
			{
				if(rb.texture != currentTexture)
				{
					effect.SetTexture(rb.texture, 0, ShaderType.PixelShader);
					currentTexture = rb.texture;
				}
							
				effect.Apply();
				graphicsDevice.Draw(rb.numVertices, rb.offset);
				
			}
		}

		public void Draw()
		{
			if (spriteList.Count > 0)
			{
				CreateRenderBatches();
				DrawBatches();				
			}
		}

		public void InternalBegin()
		{
			graphicsDevice.SetTopology(PrimitiveTopology.PointList);
			effect.SetVariable("ProjMatrix", "perFrame", MVP, ShaderType.GeometryShader);
		
			graphicsDevice.IsDepthEnable(false);
			graphicsDevice.SetblendState(bs);
			graphicsDevice.SetVertexBuffer(vertexBuffer);
		}

		public void End()
		{
			graphicsDevice.SetblendState(bs1);
			graphicsDevice.IsDepthEnable(true);
		}

		public void Dispose()
		{
			
		}
	}
}