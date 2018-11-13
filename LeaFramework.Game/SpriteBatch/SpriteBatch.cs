// This is a personal academic project. Dear PVS-Studio, please check it.

// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using LeaFramework.Graphics;
using SharpDX;


namespace LeaFramework.Game.SpriteBatch
{
	public class SpriteBatch : IDisposable
	{
		private GraphicsDevice graphicsDevice;
	
		private SortMode sortMode;
		private readonly SpriteBatcher spriteBatcher;

		public SpriteBatch(GraphicsDevice graphicsDevice, int maxBatchSize = 1024)
		{
			this.graphicsDevice = graphicsDevice;

			spriteBatcher = new SpriteBatcher(graphicsDevice, 20);
		}

		public void Begin(Matrix scale, SortMode sortMode = SortMode.Texture)
		{	
			this.sortMode = sortMode;
			spriteBatcher.PrepareForRendering();
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

			spriteBatcher.AddSpriteInfo(tmpSprite);
		}

		public void SubmitString(SpriteFont spriteFont, string str, Vector2 position, Color color)
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
				if (str[i] == '\n')
				{
					i++;
					// new Line Space, which value is right??!?
					currentPos.Y += highestGlyph;
					currentPos.X = position.X;
					continue;
				}

				// WhiteSpace
				if (str[i] == ' ')
				{
					currentPos.X += character.metrics.HorizontalAdvance.ToInt32();
					continue;
				}

				var metrics = character.metrics;
				var xpos = currentPos.X + metrics.HorizontalBearingX.ToSingle();
				var ypos = currentPos.Y - metrics.HorizontalBearingY.ToSingle();

				var w = metrics.Width.ToSingle();
				var h = metrics.Height.ToSingle();

			
				
				var tmpSprite = new SpriteInfo
				{
					position = new Vector2(xpos, ypos),
					size = new Vector2(w, h),
					offset = new Vector2(character.offset.X / spriteFont.TextureAtlas.Width, character.offset.Y / spriteFont.TextureAtlas.Height),
					color = color.ToVector4(),
					srv = spriteFont.textureAtlasSRV,
					isFont = true
				};

			
				spriteBatcher.AddSpriteInfo(tmpSprite);

				currentPos.X += metrics.HorizontalAdvance.ToInt32();
			}
		}

		public void End()
		{
			spriteBatcher.Draw();
		}

		public void Dispose()
		{
			spriteBatcher.Dispose();
			
		}
	}
}
