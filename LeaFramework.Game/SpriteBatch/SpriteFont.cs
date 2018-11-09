﻿using LeaFramework.Graphics;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpFont;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaFramework.Game.SpriteBatch
{
	public class SpriteFont
	{
		public const int TextureAtlasWidthHeight = 512;
		private readonly GraphicsDevice graphicsDevice;
		public Bitmap TextureAtlas;
		private Library library = new Library();
		public readonly Dictionary<char, GlyphInfo> glyphList = new Dictionary<char, GlyphInfo>(sizeof(char));
		public ShaderResourceView textureAtlasSRV;

		public SpriteFont(GraphicsDevice graphicsDevice, string fontName, int fontSize)
		{
			this.graphicsDevice = graphicsDevice;
			TextureAtlas = new Bitmap(TextureAtlasWidthHeight, TextureAtlasWidthHeight);
			
			var collectedGlyphBitmaps = CollectGlyphs(fontName, fontSize);

			CreateTextureAtlas(collectedGlyphBitmaps);

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

				// If not WhiteSpace == Write texture
				if (currentGlyph.Bitmap.Width > 0)
				{
					var gdiBitmap = currentGlyph.Bitmap.ToGdipBitmap(System.Drawing.Color.White);

					BitmapHelper.CopyRegionIntoImage(gdiBitmap,
						new System.Drawing.Rectangle(0, 0, (int)currentGlyph.Metrics.Width,
							(int)currentGlyph.Metrics.Height), ref TextureAtlas,
						new System.Drawing.Rectangle(offsetX, offsetY, (int)currentGlyph.Metrics.Width,
							(int)currentGlyph.Metrics.Height));
				}

				//IF roughly reach the Bitmap border, make a new Line
				if (offsetX + currentGlyph.Metrics.Width >= TextureAtlasWidthHeight - 60)
				{
					offsetY += currentGlyph.Metrics.Height.ToInt32() + 25;
					offsetX = 0;
				}

				glyphList.Add(glyph.Key,
					new GlyphInfo(currentGlyph.Metrics, new Vector2(offsetX, offsetY)));


				offsetX += currentGlyph.Bitmap.Width;
			}
		}

		private void CreateTexture2D()
		{
			var data = TextureAtlas.LockBits(
				new System.Drawing.Rectangle(0, 0, TextureAtlas.Width, TextureAtlas.Height),
				ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

			var ret = new Texture2D(graphicsDevice.NatiDevice1.D3D11Device, new Texture2DDescription()
			{
				Width = TextureAtlas.Width,
				Height = TextureAtlas.Height,
				ArraySize = 1,
				BindFlags = BindFlags.ShaderResource,
				Usage = ResourceUsage.Immutable,
				CpuAccessFlags = CpuAccessFlags.None,
				Format = Format.B8G8R8A8_UNorm,
				MipLevels = 1,
				OptionFlags = ResourceOptionFlags.None,
				SampleDescription = new SampleDescription(1, 0),
			}, new DataRectangle(data.Scan0, data.Stride));

			TextureAtlas.UnlockBits(data);

			textureAtlasSRV = new ShaderResourceView(graphicsDevice.NatiDevice1.D3D11Device, ret);


		}

	}
}
