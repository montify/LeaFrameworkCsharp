using LeaFramework.Game.SpriteBatch;
using LeaFramework.Graphics;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaFramework.GUI.Widgets
{
	public class Button : Widget
	{
		public Button(string text, Vector2 size, Vector2 position, LeaTexture2D texture, bool isHover, GraphicsDevice graphicsDevice)
		{
			base.graphicsDevice = graphicsDevice;
			base.text = text;
			base.size = size;
			base.position = position;
			base.texture = texture;
			base.color = Color.White;
			base.isHover = isHover;
			base.isMovable = false;
		}

		public override void Update(Vector2 partenPosition)
		{
			if (Intersect(partenPosition, graphicsDevice))
				color = Color.Wheat;
			else
				color = Color.White;
			

			base.Update(partenPosition);
		}

		public override void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont, Vector2 partenPosition)
		{
			spriteBatch.Submit(texture, new Vector2((int)partenPosition.X + (int)position.X, (int)partenPosition.Y + (int)position.Y), new Vector2((int)size.X, (int)size.Y), color);

			
			if (!string.IsNullOrEmpty(text))
			{
				var metrics = spriteFont.MeasureString(text);

				var finalPos = new Vector2
				{
					X = (partenPosition.X + position.X) + (size.X/2) - metrics.X / 2,
					Y = (partenPosition.Y + position.Y) + (size.Y / 2) + metrics.Y / 2,
				};

				spriteBatch.SubmitString(spriteFont, text, finalPos, color);
			}
				

			base.Draw(spriteBatch, spriteFont, partenPosition);
		}
	}
}
