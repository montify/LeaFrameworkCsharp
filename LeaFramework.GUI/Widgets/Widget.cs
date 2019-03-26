using LeaFramework.Game.SpriteBatch;
using LeaFramework.Graphics;
using LeaFramework.Input;
using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaFramework.GUI.Widgets
{
	public abstract class Widget
	{
		public GraphicsDevice graphicsDevice;
		public Vector2 position;
		public LeaTexture2D texture;
		public Vector2 size;
		public string text { get; set; }
		public Color color;
		public bool isHover;
		
		public bool isMovable;
		public bool isCLicked;

		private Vector2 previousMoiseMouseState;


		public bool Intersect(Vector2 partenPosition, GraphicsDevice graphicsDevice)
		{
			this.graphicsDevice = graphicsDevice;


			Matrix ScaleMatrix = Matrix.Scaling(
				(float)graphicsDevice.ViewPort.Width / 1280,
				(float)graphicsDevice.ViewPort.Height / 720,
				1f
			);

			Vector2 Scale = new Vector2(ScaleMatrix.M11, ScaleMatrix.M22);

			var mouseState = InputManager.MousePosition / Scale;

			var widgetRectangle = new Rectangle((int)partenPosition.X + (int)position.X, (int)partenPosition.Y + (int)position.Y, (int)size.X, (int)size.Y);

			return widgetRectangle.Contains(mouseState.X, mouseState.Y);

		}


		//public bool IsIntersectAndMouseDown(Vector2 partenPosition)
		//{
		//	var mouseState = Mouse.GetState();

		//	bool isClick = false;

		//	var widgetRectangle = new Rectangle((int)partenPosition.X + (int)position.X, (int)partenPosition.Y + (int)position.Y, (int)size.X, (int)size.Y);

		//	if (widgetRectangle.Contains(mouseState.X, mouseState.Y) && mouseState.LeftButton == ButtonState.Pressed &&
		//		previousMoiseMouseState.LeftButton == ButtonState.Released)
		//		isClick = true;


		//	previousMoiseMouseState = mouseState;

		//	return isClick;
		//}


		public virtual void Update(Vector2 partenPosition) { }

		public virtual void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont, Vector2 partenPosition) { }

	}

}

