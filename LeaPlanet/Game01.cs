using System.Diagnostics;
using System.Threading.Tasks;
using LeaFramework.Content;
using LeaFramework.Effect;
using LeaFramework.Game;
using LeaFramework.Game.SpriteBatch;
using LeaFramework.Graphics;
using LeaFramework.GUI;
using LeaFramework.GUI.Widgets;
using LeaFramework.Input;
using PlayGround;
using SharpDX;
using SharpDX.Direct3D11;

using Color = SharpDX.Color;

namespace LeaFramework.PlayGround
{
	public class Game01 : LeaPlanet.src.Game
	{
		private Triangle triangle;
		private RasterizerState1 rs;
		private SpriteBatch spriteBatch;
		LeaTexture2D texture, texture1;
		private float x, y = 10;
		float xPos;

		SpriteFont sf;
		EComputeShader computeShader;
		//GuiManager gManager;

		private bool dirRight = true;
		public float speed = 2.0f;
		string t;
		int aasd;

		Stopwatch sw = new Stopwatch();

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
			sf = new SpriteFont(GraphicsDevice, "OpenSans-Regular.ttf", 30);

			//computeShader = new EComputeShader(GraphicsDevice);
			//computeShader.Load("Content//computeShaderTest.hlsl");

			triangle = new Triangle(GraphicsDevice);

			spriteBatch = new SpriteBatch(GraphicsDevice);

			

			texture = ContentManager.Instance.Load<LeaTexture2D>(GraphicsDevice, "Content//test.png");
			texture1 = ContentManager.Instance.Load<LeaTexture2D>(GraphicsDevice, "Content//uvTestTexSmall.png");
			//gManager = new GuiManager(GraphicsDevice, sf);

			Button b = new Button("Button", new Vector2(130, 40), new Vector2(200, 100), texture, false, GraphicsDevice);
			Button b1 = new Button("Button", new Vector2(130, 40), new Vector2(200, 150), texture, false, GraphicsDevice);
			Button b2 = new Button("A * -", new Vector2(130, 40), new Vector2(200, 200), texture, false, GraphicsDevice);
			Button b3 = new Button("Button", new Vector2(130, 40), new Vector2(200, 250), texture, false, GraphicsDevice);

			//gManager.AddWidget(b);
			//gManager.AddWidget(b1);
			//gManager.AddWidget(b2);
			//gManager.AddWidget(b3);


			var inputThreadHandle = Task.Factory.StartNew(() =>
				{
					InputManager.Instance.Listen(RenderForm);
				});

			
			var desc = new RasterizerStateDescription1
			{
				CullMode = CullMode.Back,
				FillMode = FillMode.Solid
			};

			rs = new RasterizerState1(GraphicsDevice.NatiDevice1.D3D11Device, desc);



		}

		public override void Unload()
		{

			rs.Dispose();
			//triangle.Dispose();
			spriteBatch.Dispose();
		
			ContentManager.Instance.Dispose();
		}
		
		public override void Update(GameTimer gameTime)
		{
			//gManager.Update();
			//triangle.Update(gameTime, new Vector3(-4, 1, 0));

			//if (InputManager.Instance.IsKeyDown(Key.A))
			//	gManager.IsVisible = false;
			//else
			//	gManager.IsVisible = true;

			//System.Console.WriteLine(InputManager.Instance.GetMousePosition().X);

		}

		public override void Render(GameTimer gameTime)
		{
			GraphicsDevice.Clear(ClearFlags.RenderTarget | ClearFlags.DepthBuffer, Color.CornflowerBlue);


			float screenscaleW = (float)GraphicsDevice.ViewPort.Width / 1280;
			float screenscaleH = (float)GraphicsDevice.ViewPort.Height / 720;
			var scale = Matrix.Scaling(screenscaleW, screenscaleH, 1);

			
			spriteBatch.Begin(scale, SortMode.Immediate);

			//for (float i = 0; i < 1000; i += 25)
			//	for (float x = 0; x < 1100; x += 25)
			//	{
			//		spriteBatch.Submit(texture, new Vector2(i, x), Vector2.Zero, Color.White);
			//	}

			spriteBatch.SubmitString(sf, "Althea Told me, i was feeling lost \n Lacking in some Direcktiion\n" +
										 "Althea Told me, i was feeling lost \n Lacking in some Direcktiion\n" +
										 "Althea Told me, i was feeling lost \n Lacking in some Direcktiion\n" +
										 "Althea Told me, i was feeling lost \n Lacking in some Direcktiion  gj GJ ij fh fi",
										 new Vector2(200, 200), Color.White);
			spriteBatch.End();

		}

	}
}
