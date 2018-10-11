﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeaFramework.Effect;
using LeaFramework.Game.Properties;
using LeaFramework.Graphics;
using LeaFramework.Graphics.VertexStructs;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.WIC;
using SharpFont;
using Bitmap = System.Drawing.Bitmap;
using Color = System.Drawing.Color;
using Encoding = SharpFont.Encoding;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace LeaFramework.Game.SpriteBatch
{
	public class SpriteFont : IDisposable
	{
		private const int TextureAtlasWidthHeight = 512;
		private GraphicsDevice graphicsDevice;
		private Bitmap textureAtlas;
		private LeaEffect effect;
		private Matrix MVP;
		private VertexBuffer vertexBuffer;
		private LeaSamplerState sampler;
		private ShaderResourceView textureAtlasSRV;
		private List<SpriteInfo> spriteInfoList = new List<SpriteInfo>();
		private BlendState blendState, blendStateNormal;
		private FontVertex[] vertices;
		private Dictionary<char, Glyph> GlyphList = new Dictionary<char, Glyph>();
		Library library = new Library();

		public SpriteFont(GraphicsDevice graphicsDevice,string fontName, int fontSize)
		{
			this.graphicsDevice = graphicsDevice;
			textureAtlas = new Bitmap(TextureAtlasWidthHeight, TextureAtlasWidthHeight);
		
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

			sampler = new LeaSamplerState();
			sampler.GenerateSamplers(graphicsDevice);

			var desc = new BlendStateDescription();
			desc.RenderTarget[0].IsBlendEnabled = true;
			desc.RenderTarget[0].SourceBlend = BlendOption.One;
			desc.RenderTarget[0].DestinationBlend = BlendOption.InverseSourceAlpha;
			desc.RenderTarget[0].BlendOperation = BlendOperation.Add;
			desc.RenderTarget[0].SourceAlphaBlend = BlendOption.One;
			desc.RenderTarget[0].DestinationAlphaBlend = BlendOption.Zero;
			desc.RenderTarget[0].AlphaBlendOperation = BlendOperation.Add;
			desc.RenderTarget[0].RenderTargetWriteMask = ColorWriteMaskFlags.All;

			blendState = new BlendState(graphicsDevice.NatiDevice1.D3D11Device, desc);
			desc.RenderTarget[0].DestinationBlend = BlendOption.Zero;

			blendStateNormal = new BlendState(graphicsDevice.NatiDevice1.D3D11Device, desc);

			effect.SetVariable("textureAtlasResWidthHeight", "startUp", 512, ShaderType.GeometryShader);

			var glyphBitMapList = CollectGlyphs(fontName, fontSize);

			CreateTextureAtlas(glyphBitMapList);


			textureAtlas.Save("AR.bmp");
			CreateTexture2D();
		}

		private Dictionary<char, GlyphSlot> CollectGlyphs(string fontName, int fontSize)
		{
			var glyphBitmapList = new Dictionary<char, GlyphSlot>();

			for (int i = 0; i < 128; i++)
			{
				var face = new Face(library, fontName);
				face.SetCharSize(0, fontSize, 72, 72);

				var x = face.GetCharIndex((char)i);
				face.LoadGlyph(x, LoadFlags.Render, LoadTarget.Normal);
				face.Glyph.RenderGlyph(RenderMode.Normal);
				
				glyphBitmapList.Add((char)i, face.Glyph);
			}

			
			return glyphBitmapList;
		}

		private void CreateTextureAtlas(Dictionary<char, GlyphSlot> glyphBitmapList)
		{
			// SORT LIST 
			int offsetX = 0;
			int offsetY = 0;

			foreach (var glyph in glyphBitmapList)
			{
				var currentGlyph = glyph.Value;
				
				// If not WhiteSpace == Create texture
				if (currentGlyph.Bitmap.Width > 0)
				{
					var gdiBitmap = currentGlyph.Bitmap.ToGdipBitmap(Color.White);

					BitmapHelper.CopyRegionIntoImage(gdiBitmap,
						new System.Drawing.Rectangle(0, 0, (int) currentGlyph.Metrics.Width,
							(int) currentGlyph.Metrics.Height), ref textureAtlas,
						new System.Drawing.Rectangle(offsetX, offsetY, (int) currentGlyph.Metrics.Width,
							(int) currentGlyph.Metrics.Height));
				}
			
			

				//IF roughly reach the Bitmap border, make a new Line
				if (offsetX + currentGlyph.Metrics.Width >= TextureAtlasWidthHeight-60)
				{
					offsetY += currentGlyph.Metrics.Height.ToInt32() + 20;
					offsetX = 0;
				}

				GlyphList.Add(glyph.Key,
					new Glyph(currentGlyph.Metrics.Width.ToInt32(), currentGlyph.Metrics.Height.ToInt32(), currentGlyph.Metrics, new Vector2(offsetX, offsetY)));

				offsetX += currentGlyph.Bitmap.Width;
			}
		}

		private void CreateTexture2D()
		{
			var data = textureAtlas.LockBits(
				new System.Drawing.Rectangle(0, 0, textureAtlas.Width, textureAtlas.Height),
				ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

			var ret = new Texture2D(graphicsDevice.NatiDevice1.D3D11Device, new Texture2DDescription()
			{
				Width = textureAtlas.Width,
				Height = textureAtlas.Height,
				ArraySize = 1,
				BindFlags = BindFlags.ShaderResource,
				Usage = ResourceUsage.Immutable,
				CpuAccessFlags = CpuAccessFlags.None,
				Format = Format.B8G8R8A8_UNorm,
				MipLevels = 1,
				OptionFlags = ResourceOptionFlags.None,
				SampleDescription = new SampleDescription(1, 0),
			}, new DataRectangle(data.Scan0, data.Stride));

			textureAtlas.UnlockBits(data);

			textureAtlasSRV = new ShaderResourceView(graphicsDevice.NatiDevice1.D3D11Device, ret);


		}


		public void Begin(Matrix scale)
		{
			MVP = Matrix.OrthoOffCenterLH(0, graphicsDevice.ViewPort.Width, graphicsDevice.ViewPort.Height, 0, 0, 1);
			MVP = Matrix.Transpose(scale * MVP);
		}

		public void SubmitString(string str, Vector2 position)
		{
			for (int i = 0; i < str.Length; i++)
			{
				var character = GlyphList[str[i]];

				//if Character != WhiteSpace
				if (str[i] != ' ')
				{
					var metrics = GlyphList[str[i]].metrics;
					var xpos = position.X + metrics.HorizontalBearingX.ToSingle();
					var ypos = position.Y - metrics.HorizontalBearingY.ToSingle();

					var w = metrics.Width.ToSingle();
					var h = metrics.Height.ToSingle();

					var spriteInfo = new SpriteInfo
					{
						position = new Vector2(xpos, ypos),
						size = new Vector2(w, h),
						offset = new Vector2(GlyphList[str[i]].offset.X / TextureAtlasWidthHeight, GlyphList[str[i]].offset.Y / TextureAtlasWidthHeight)
					};

					spriteInfoList.Add(spriteInfo);
					position.X += metrics.HorizontalAdvance.ToInt32();
				}
				else
				{
					position.X += character.metrics.HorizontalAdvance.ToInt32();
				}
			}
		}


		public void Draw()
		{
			graphicsDevice.SetTopology(PrimitiveTopology.PointList);

		
			graphicsDevice.SetblendState(blendState);

			//TODO: MAX BATCH SIZE NOT * 2
			var vertices = new FontVertex[spriteInfoList.Count * 2];

			for (int i = 0; i < spriteInfoList.Count; i++)
			{
				vertices[i] = new FontVertex(spriteInfoList[i].position, spriteInfoList[i].size, spriteInfoList[i].color, spriteInfoList[i].offset);
			}

			if (vertexBuffer == null)
			{
				vertexBuffer = new VertexBuffer(graphicsDevice, true);
				vertexBuffer.CreateAndSetData(vertices);
			}

			graphicsDevice.SetVertexBuffer(vertexBuffer);
			vertexBuffer.UpdateBuffer(vertices, 0);

			graphicsDevice.SetVertexBuffer(vertexBuffer);

			effect.SetVariable("ProjMatrix", "perFrame", MVP, ShaderType.GeometryShader);
			effect.SetSampler(sampler, 0 , ShaderType.PixelShader);
			effect.SetTexture(textureAtlasSRV, 0, ShaderType.PixelShader);
			effect.Apply();

			graphicsDevice.Draw(spriteInfoList.Count, 0);
			graphicsDevice.SetblendState(blendStateNormal);

			spriteInfoList.Clear();
		}

		public void Dispose()
		{
		
		}
	}
}
