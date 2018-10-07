using System;
using LeaFramework.Content;
using LeaFramework.Game;
using LeaFramework.Game.SpriteBatch;
using LeaFramework.Graphics;
using PlayGround;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.Mathematics;

namespace LeaFramework.PlayGround
{
	public class Game01 : LeaPlanet.src.Game
	{
		private Triangle triangle;

		private RasterizerState1 rs, rs1;
		private RenderTarget2D rt;
		private ShaderResourceView tex, tex1 ;
		private SpriteBatch spriteBatch;
		private SpriteFont spriteFont;

		private float x;

		public Game01()
		{
			WindowTitle = "Framework ALEX 0.01";
			IsVSyncEnable = true;
			
			WindowWidth = 1280;
			WindowHeight = 720;
			
			base.Init();
		}

		public override void Load()
		{

			triangle = new Triangle(GraphicsDevice);

			spriteBatch = new SpriteBatch(GraphicsDevice);
			spriteFont = new SpriteFont(GraphicsDevice);
			ContentManager.Instance.rootDictionary = "Content";

			ContentManager.Instance.LoadTexture(GraphicsDevice, "Content//uvTestTex.png");
			ContentManager.Instance.LoadTexture(GraphicsDevice, "Content//tt.png");

			tex = ContentManager.Instance.GetTexture("uvTestTex.png");
			tex1 = ContentManager.Instance.GetTexture("tt.png");

			var desc = new RasterizerStateDescription1
			{
				CullMode = CullMode.None,
				FillMode = FillMode.Solid
			};

			rs = new RasterizerState1(GraphicsDevice.NatiDevice1.D3D11Device, desc);


		
		}

		public override void Unload()
		{

			rs.Dispose();
			triangle.Dispose();
			spriteBatch.Dispose();
			spriteFont.Dispose();
			ContentManager.Instance.Dispose();
		}

		public override void Update(GameTimer gameTime)
		{
			triangle.Update(gameTime, new Vector3(3, 0, 0));
		}

		public override void Render(GameTimer gameTime)
		{
			float r = 15f / 255;
			float g = 40f / 255;
			float b = 48f / 255;

			GraphicsDevice.Clear(ClearFlags.RenderTarget | ClearFlags.DepthBuffer, new Color(new Vector3(r,g,b)));
			GraphicsDevice.NatiDevice1.D3D11Device.ImmediateContext1.Rasterizer.State = rs;

			x += 1;
			var sd = x.ToString();
			spriteFont.Begin();
			spriteFont.SubmitString(sd, new Vector2(100, 500));
			spriteFont.SubmitString("LeaFramework v00000 xD", new Vector2(100, 100));
			spriteFont.SubmitString(":D hehe", new Vector2(100, 200));
			spriteFont.SubmitString("!#$%&'()*+,-./0123456789:;<=>?@", new Vector2(100, 300));
			spriteFont.SubmitString("[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~", new Vector2(100, 400));
			spriteFont.Draw();
		}

	}
}
