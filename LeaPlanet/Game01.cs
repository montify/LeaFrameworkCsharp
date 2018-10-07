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

		private float x;

		public Game01()
		{
			WindowTitle = "Framework ALEX 0.01";
			IsVSyncEnable = false;
			
			WindowWidth = 1280;
			WindowHeight = 720;
			
			base.Init();
		}

		public override void Load()
		{

			triangle = new Triangle(GraphicsDevice);

			spriteBatch = new SpriteBatch(GraphicsDevice);
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

			triangle.Render(Color.Red.ToVector3());
			x += 0.01f;

			spriteBatch.Begin();
			spriteBatch.Submit(tex, new Vector2(0, 0), new Vector2(32,36), Color.Red.ToVector4());
			//spriteBatch.Submit(tex, new Vector2(0, 0), new Vector2(200, 200), Color.Green.ToVector4());
			spriteBatch.SubmitString("hallo-wasgehtab<>?", new Vector2(100,100));
			spriteBatch.SubmitString("geiler", new Vector2(100, 200));
			spriteBatch.End();
		}

	}
}
