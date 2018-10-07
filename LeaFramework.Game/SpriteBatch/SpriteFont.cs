using System;
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
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace LeaFramework.Game.SpriteBatch
{
	public class SpriteFont : IDisposable
	{
		private GraphicsDevice graphicsDevice;
		private Face face;
		private Dictionary<uint, Glyph> GlyphList = new Dictionary<uint, Glyph>();
		private Bitmap textureAtlas;
		private LeaEffect effect;
		private Matrix MVP;
		private VertexBuffer vertexBuffer;
		private LeaSamplerState sampler;
		private ShaderResourceView textureAtlasSRV;
		private List<SpriteInfo> spriteInfoList = new List<SpriteInfo>();
		private BlendState blendState, blendStateNormal;
		private FontVertex[] vertices;

		public SpriteFont(GraphicsDevice graphicsDevice,string fontName, int fontSize)
		{
			this.graphicsDevice = graphicsDevice;
			textureAtlas = new Bitmap(512, 512);
			CreateGlyphs(fontName, fontSize);
		
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

		}


		public void CreateGlyphs(string fontName, int fontSize)
		{
			Library library = new Library();
			face = new Face(library, fontName);
			face.SetCharSize(0, fontSize, 72, 72);

			string allChars = "!#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";

			int offsetX = 0;
			int offsetY = 0;

			for (int i = 0; i < allChars.Length; i++)
			{
				var x = face.GetCharIndex(allChars[i]);
				face.LoadGlyph(x, LoadFlags.Render, LoadTarget.Normal);
				face.Glyph.RenderGlyph(RenderMode.Normal);

				FTBitmap bm = face.Glyph.Bitmap;


					var gdiBitmap = bm.ToGdipBitmap(Color.White);

					CopyRegionIntoImage(gdiBitmap,
						new System.Drawing.Rectangle(0, 0, (int) face.Glyph.Metrics.Width,
							(int) face.Glyph.Metrics.Height), ref textureAtlas,
						new System.Drawing.Rectangle(offsetX, offsetY, (int) face.Glyph.Metrics.Width,
							(int) face.Glyph.Metrics.Height));


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

					GlyphList.Add(x,
						new Glyph(gdiBitmap.Width, gdiBitmap.Height, face.Glyph.Metrics,
							new Vector2(offsetX, offsetY)));

					//IF roughly rach the Bitmap border, make a new Line
					if (offsetX > 450)
					{
						offsetY += 70;
						offsetX = 0;
					}

					// +4 because Spacing between Characters in the Bitmap
					offsetX += face.Glyph.Advance.X.ToInt32() + 4;
			}
		}

		public static void CopyRegionIntoImage(Bitmap srcBitmap, System.Drawing.Rectangle srcRegion,ref Bitmap destBitmap, System.Drawing.Rectangle destRegion)
    {
        using (System.Drawing.Graphics grD = System.Drawing.Graphics.FromImage(destBitmap))            
        {
            grD.DrawImage(srcBitmap, destRegion, srcRegion, GraphicsUnit.Pixel);                
        }
    }


		public void Begin()
		{
			MVP = Matrix.OrthoOffCenterLH(0, graphicsDevice.ViewPort.Width, graphicsDevice.ViewPort.Height, 0, 0, 1);
			MVP = Matrix.Transpose(MVP);
		}


		public void SubmitString(string str, Vector2 position)
		{
			for (int i = 0; i < str.Length; i++)
			{
				var character = face.GetCharIndex(str[i]);
			
				//if Character != WhiteSpace
				if (character != face.GetCharIndex(' '))
				{
					var metrics = GlyphList[character].metrics;
					var xpos = position.X + metrics.HorizontalBearingX.ToSingle();
					var ypos = position.Y - metrics.HorizontalBearingY.ToSingle();

					var w = metrics.Width.ToSingle();
					var h = metrics.Height.ToSingle();


					SpriteInfo spriteInfo = new SpriteInfo
					{
						position = new Vector2(xpos, ypos),
						size = new Vector2(w, h),
						offset = new Vector2(GlyphList[character].offset.X / 512, GlyphList[character].offset.Y / 512)


					};

					spriteInfoList.Add(spriteInfo);
					position.X += metrics.HorizontalAdvance.ToInt32();
				}
				else
				{
					position.X += face.Glyph.Advance.X.ToInt32();
				}
			}
		}


		public void Draw()
		{
			graphicsDevice.SetTopology(PrimitiveTopology.PointList);

		
			graphicsDevice.SetblendState(blendState);

			//TODO: MAX BATCH SIZE NOT * 2
			FontVertex[] vertices = new FontVertex[spriteInfoList.Count * 2];

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
			face.Dispose();
		}
	}
}
