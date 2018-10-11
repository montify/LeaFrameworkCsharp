using System;
using System.Drawing;
using LeaFramework.Content;
using LeaFramework.Game;
using LeaFramework.Game.SpriteBatch;
using LeaFramework.Graphics;
using PlayGround;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.Mathematics;
using Color = SharpDX.Color;

namespace LeaFramework.PlayGround
{
	public class Game01 : LeaPlanet.src.Game
	{
		private Triangle triangle;

		private RasterizerState1 rs, rs1;
		private RenderTarget2D rt;
		private ShaderResourceView tex, tex1 ;
		private SpriteBatch spriteBatch;
		private SpriteFont spriteFont, spriteFont1;

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
			//	Image i = new Bitmap("Content//uvTestTex.png");


			triangle = new Triangle(GraphicsDevice);

			spriteBatch = new SpriteBatch(GraphicsDevice);
			spriteFont = new SpriteFont(GraphicsDevice, "orange.ttf", 25);
			//spriteFont1 = new SpriteFont(GraphicsDevice, "orange.ttf", 25);
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
			//triangle.Update(gameTime, new Vector3(3, 0, 0));
		}

		public override void Render(GameTimer gameTime)
		{
			float r = 15f / 255;
			float g = 40f / 255;
			float b = 48f / 255;

			GraphicsDevice.Clear(ClearFlags.RenderTarget | ClearFlags.DepthBuffer, new Color(new Vector3(r, g, b)));

			GraphicsDevice.SetRasterizerState(rs);

			x += 0.1f;
			

			float screenscaleW = (float)GraphicsDevice.ViewPort.Width / 1280;
			float screenscaleH = (float)GraphicsDevice.ViewPort.Height / 720;
			var scale = Matrix.Scaling(screenscaleW, screenscaleH, 1);



			spriteFont.Begin(scale);
			spriteFont.SubmitString(x.ToString("##.###"), new Vector2(90, 90));
			spriteFont.SubmitString("GgHh", new Vector2(90, 150));
			spriteFont.SubmitString("How the raid is going?   {|}", new Vector2(90, 200));
			//spriteFont.SubmitString("How the raid is going?9987&%721", new Vector2(90, 200));
		//	spriteFont.SubmitString("SCAV BOSS IS SO aweSome Thats ghurt asuda sdasda 646", new Vector2(90, 250));
			spriteFont.Draw();

			spriteBatch.Begin(Matrix.Identity);
			spriteBatch.Submit(tex1, new Vector2(0, 0), new Vector2(GraphicsDevice.ViewPort.Width, 60), Color.White.ToVector4());
			spriteBatch.End();


		}

	}
}
