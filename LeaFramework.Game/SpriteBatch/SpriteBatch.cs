using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms.VisualStyles;
using LeaFramework.Effect;
using LeaFramework.Game.Properties;
using LeaFramework.Graphics;
using LeaFramework.Graphics.VertexStructs;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpFont;

namespace LeaFramework.Game.SpriteBatch
{
	public class SpriteBatch : IDisposable
	{
		private GraphicsDevice graphicsDevice;
		private LeaEffect effect;
		private LeaSamplerState sampler;
		private Matrix MVP;
		private VertexBuffer vertexBuffer;
		private List<SpriteInfo> spriteList;
		private List<RenderBatch> renderBatches;
		private float t; 
		private Dictionary<uint, GlyphInfo> GlypList = new Dictionary<uint, GlyphInfo>();
		private SortMode sortMode;
		private Face face;
		private BlendState bs, bs1;
		bool tee = true;
		public SpriteBatch(GraphicsDevice graphicsDevice, int maxBatchSize = 1024)
		{
			this.graphicsDevice = graphicsDevice;

			
			spriteList = new List<SpriteInfo>(maxBatchSize);
			renderBatches = new List<RenderBatch>();
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

			sampler = new LeaSamplerState();
			sampler.GenerateSamplers(graphicsDevice);

			effect = new LeaEffect(graphicsDevice, createInfo);

			vertexBuffer = new VertexBuffer(graphicsDevice, BufferType.Dynamic);

			FontVertex[] v = new FontVertex[1024];
			vertexBuffer.SetData(v);

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

			effect.SetVariable("textureAtlasResWidthHeight", "startUp", 512, ShaderType.GeometryShader);
			effect.SetSampler(sampler, 0, ShaderType.PixelShader);
		}

		
		public void Begin(Matrix scale, SortMode sortMode = SortMode.Texture)
		{
			spriteList.Clear();
			renderBatches.Clear();
			this.sortMode = sortMode;
			// Should Calc. MVP here? When yes - when window is maximized the spriteList dont change :(
			MVP = Matrix.OrthoOffCenterLH(0, graphicsDevice.ViewPort.Width, graphicsDevice.ViewPort.Height, 0, 0, 1);
			MVP = Matrix.Transpose(scale * MVP);
		}
		
		public void Submit(LeaTexture2D tex, Vector2 position, Vector2 size, Color color)
		{
			var tmpSprite = new SpriteInfo {
				position = position,
				size = new Vector2(tex.Width, tex.Height),
				offset = Vector2.Zero,
				color = color.ToVector4(),
				srv = tex.ShaderResourceView,
				textureID = tex.GetHashCode(),
				isFont = false,
			};

			spriteList.Add(tmpSprite);
		}

		public void SubmitString(SpriteFont spriteFont, string str, Vector2 position,  Color color)
		{
			if (string.IsNullOrEmpty(str))
				return;
			
			Vector2 currentPos = position;

			float highestGlyph = 0;

			for (int i = 0; i < str.Length; i++)
			{
				var character = spriteFont.glyphList[str[i]];

				// Find Highest Glyph Y Value for newLine
				if (character.metrics.Height.ToSingle() > highestGlyph)
					highestGlyph = character.metrics.Height.ToSingle();

				// new Line
				if (str[i] == '|' && str[i+1] == 'n')
				{
					i++;
					currentPos.Y += highestGlyph;
					currentPos.X = position.X;
					i++;

					continue;
				}
				//if Character != WhiteSpace
				if (str[i] != ' ')
				{
					var metrics = character.metrics;
					var xpos = currentPos.X + metrics.HorizontalBearingX.ToSingle();
					var ypos = currentPos.Y - metrics.HorizontalBearingY.ToSingle();

					var w = metrics.Width.ToSingle();
					var h = metrics.Height.ToSingle();

					var spriteInfo = new SpriteInfo
					{
						position = new Vector2(xpos, ypos),
						size = new Vector2(w, h),
						offset = new Vector2(character.offset.X / spriteFont.TextureAtlas.Width, character.offset.Y / spriteFont.TextureAtlas.Height),
						color = color.ToVector4(),
						srv = spriteFont.textureAtlasSRV,
						isFont = true
					};

					spriteList.Add(spriteInfo);

					currentPos.X += metrics.HorizontalAdvance.ToInt32();
				}
				else
				{
					currentPos.X += character.metrics.HorizontalAdvance.ToInt32();
				}

			}
		}

		public void End()
		{
			if (spriteList.Count == 0) return;

			if (sortMode == SortMode.Texture)
			{
				spriteList.Sort((x, y) => x.textureID.CompareTo(y.textureID));
				spriteList.Reverse();
			}
		
			CreateRenderBatches();	
			RenderBatches();
		}

		private void CreateRenderBatches()
		{
			if (spriteList.Count == 0)
				return;

			var v = new FontVertex[spriteList.Count];

			renderBatches.Add(new RenderBatch(spriteList[0].srv, 0, 1));

			v[0] = new FontVertex(spriteList[0].position, spriteList[0].size, spriteList[0].color, spriteList[0].offset, spriteList[0].isFont);

			int offset = 0;

			offset++;

			for (int i = 1; i < spriteList.Count; i++)
			{
				if (spriteList[i].textureID != spriteList[i - 1].textureID)
					renderBatches.Add(new RenderBatch(spriteList[i].srv, offset, 1));
				else
					renderBatches.Last().numVertices += 1;

				v[i] = new FontVertex(spriteList[i].position, spriteList[i].size, spriteList[i].color, spriteList[i].offset, spriteList[i].isFont); 
				offset++;
			}

			vertexBuffer.UpdateBuffer(v, 0);
		}

		private void RenderBatches()
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

		public void Dispose()
		{
			effect.Dispose();
			sampler.Dispose();
			vertexBuffer.Dispose();
			bs1.Dispose();
			bs.Dispose();
		}
	}
}
