using LeaFramework.Game.SpriteBatch;
using LeaFramework.Graphics;
using LeaFramework.GUI.Widgets;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaFramework.GUI
{
	public class GuiManager
	{
		private readonly SpriteBatch spriteBatch;

		private readonly SpriteFont spriteFont;
		private readonly List<Widget> widgetList = new List<Widget>();
		public bool IsVisible = true;

		public Vector2 position;

		public GuiManager(GraphicsDevice gDevice, SpriteFont spriteFont)
		{
			this.spriteBatch = new SpriteBatch(gDevice);
			this.spriteFont = spriteFont;
		}

		public void AddWidget(Widget widget)
		{
			if (!widgetList.Contains(widget))
				widgetList.Add(widget);
		}

		public void Update()
		{
			if (IsVisible)

				foreach (var w in widgetList)
				{
					w.Update(position);
				}
		}

		public void Draw(Matrix scale )
		{
			spriteBatch.Begin(scale, SortMode.Texture);

			if (IsVisible)
				foreach (var w in widgetList)
				{
					w.Draw(spriteBatch, spriteFont, position);
				}

			spriteBatch.End();
		}
	}
}
