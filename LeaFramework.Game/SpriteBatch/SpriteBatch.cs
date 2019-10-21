// This is a personal academic project. Dear PVS-Studio, please check it.

// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LeaFramework.Graphics;
using SharpDX;


namespace LeaFramework.Game.SpriteBatch
{
	public class SpriteBatch : IDisposable
	{
		private GraphicsDevice graphicsDevice;
		private SortMode sortMode;
		private readonly SpriteBatcher spriteBatcher;
		SpriteInfo[] spriteInfoList;
		int ptr;

		public SpriteBatch(GraphicsDevice graphicsDevice, int maxBatchSize = 1024)
		{
			this.graphicsDevice = graphicsDevice;
			spriteInfoList = new SpriteInfo[maxBatchSize];

			for (int i = 0; i < spriteInfoList.Length; i++)
			{
				spriteInfoList[i] = new SpriteInfo();
			}
			spriteBatcher = new SpriteBatcher(graphicsDevice, maxBatchSize);

		
		}

		public void Begin(Matrix scaleMatrix, SortMode sortMode = SortMode.Texture)
		{
			this.sortMode = sortMode;

			spriteBatcher.ScaleMatrix = scaleMatrix;
		
			spriteBatcher.PrepareForRendering();
			spriteBatcher.InternalBegin();
		}
		
		public void Submit(LeaTexture2D tex, Vector2 position, Vector2 size, Color color)
		{
			spriteInfoList[ptr].position = position;
			spriteInfoList[ptr].size = new Vector2(tex.Width, tex.Height);
			spriteInfoList[ptr].offset = Vector2.Zero;
			spriteInfoList[ptr].color = color.ToVector4();
			spriteInfoList[ptr].srv = tex.ShaderResourceView;
			spriteInfoList[ptr].textureHashCode = tex.GetHashCode();

			//var tmpSprite = new SpriteInfo
			//{
			//	position = position,
			//	size = new Vector2(tex.Width, tex.Height),
			//	offset = Vector2.Zero,
			//	color = color.ToVector4(),
			//	srv = tex.ShaderResourceView,
			//	textureHashCode = tex.GetHashCode()
			//};


			spriteBatcher.AddSpriteInfo(spriteInfoList[ptr], ref ptr);

			ptr++;
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

				//// Find Highest Glyph Y Value for newLine
				//if (character.metrics.Height.ToSingle() > highestGlyph)
				//	highestGlyph = character.metrics.Height.ToSingle();

				// new Line
				if (str[i] == '\n')
				{
					i++;
					// new Line Space, which value is right??!?
					currentPos.Y += spriteFont.LineHight;
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


				spriteInfoList[ptr].position = new Vector2(xpos, ypos + spriteFont.LineHight);
				spriteInfoList[ptr].size = new Vector2(w, h);
				spriteInfoList[ptr].offset = new Vector2(character.offset.X / spriteFont.TextureAtlas.Width, character.offset.Y / spriteFont.TextureAtlas.Height);
				spriteInfoList[ptr].color = color.ToVector4();
				spriteInfoList[ptr].srv = spriteFont.textureAtlasSRV;
				spriteInfoList[ptr].textureHashCode = spriteFont.textureAtlasSRV.GetHashCode();


				spriteBatcher.AddSpriteInfo(spriteInfoList[ptr], ref ptr);

				ptr++;
				currentPos.X += metrics.HorizontalAdvance.ToInt32();
			}
		}

		public void End()
		{
			spriteBatcher.Draw();
			spriteBatcher.End();
			ptr = 0;

		}

		public void Dispose()
		{
			spriteBatcher.Dispose();
			
		}
	}
}
