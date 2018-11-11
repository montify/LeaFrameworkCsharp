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
		LeaTexture2D texture;
		private float x, y = 10;
		float xPos;
	
		SpriteFont sf;
		EComputeShader computeShader;
		//GuiManager gManager;

		private bool dirRight = true;
		public float speed = 2.0f;
		string t;
		bool test = true;

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
			
			sf = new SpriteFont(GraphicsDevice, "orange.ttf", 30);
			
			computeShader = new EComputeShader(GraphicsDevice);
			computeShader.Load("Content//computeShaderTest.hlsl");

			triangle = new Triangle(GraphicsDevice);
			
			spriteBatch = new SpriteBatch(GraphicsDevice);

			Vector3[] test = { new Vector3(1, 1, 1), new Vector3(2, 2, 2) };

			texture = ContentManager.Instance.Load<LeaTexture2D>(GraphicsDevice, "Content//button.png");

			//gManager = new GuiManager(GraphicsDevice, sf);

			Button b = new Button("Button", new Vector2(130, 40), new Vector2(200, 100), texture, false, GraphicsDevice);
			Button b1 = new Button("Button", new Vector2(130, 40), new Vector2(200, 150), texture, false, GraphicsDevice);
			Button b2 = new Button("A * -", new Vector2(130, 40), new Vector2(200, 200), texture, false, GraphicsDevice);
			Button b3 = new Button("Button", new Vector2(130, 40), new Vector2(200, 250), texture, false, GraphicsDevice);
		
			//gManager.AddWidget(b);
			//gManager.AddWidget(b1);
			//gManager.AddWidget(b2);
			//gManager.AddWidget(b3);


			Task.Factory.StartNew(() =>
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
			triangle.Dispose();
			spriteBatch.Dispose();
		
			ContentManager.Instance.Dispose();
		}
		
		public override void Update(GameTimer gameTime)
		{
			//gManager.Update();
			//triangle.Update(gameTime, new Vector3(-3, 1, 0));

			//if (InputManager.Instance.IsKeyDown(Key.A))
			//	gManager.IsVisible = false;
			//else
			//	gManager.IsVisible = true;

		}

		public override void Render(GameTimer gameTime)
		{
			GraphicsDevice.Clear(ClearFlags.RenderTarget | ClearFlags.DepthBuffer, new Color(37,37,38));

			

			#region Input
			float screenscaleW = (float)GraphicsDevice.ViewPort.Width / 1280;
			float screenscaleH = (float)GraphicsDevice.ViewPort.Height / 720;
			var scale = Matrix.Scaling(screenscaleW, screenscaleH, 1);

			if (InputManager.Instance.IsKeyDown(Key.ESC)) IsRunning = false;

			//if (InputManager.Instance.IsKeyDown(Key.W))
			//	y -= 300 * gameTime.DeltaTime;

			//if (InputManager.Instance.IsKeyDown(Key.S))
			//	y += 300 * gameTime.DeltaTime;

			//if (InputManager.Instance.IsKeyDown(Key.A))
			//	x -= 300 * gameTime.DeltaTime;
			//if (InputManager.Instance.IsKeyDown(Key.D))
			//	x += 300 * gameTime.DeltaTime;


			//RectangleF player = new RectangleF(x, y, 100, 100);
			//RectangleF st = new RectangleF(xPos, 300, 100, 100);


			//if (dirRight)
			//	xPos += 400 * gameTime.DeltaTime;
			//else
			//	xPos -= 400 * gameTime.DeltaTime;


			//if (xPos >= GraphicsDevice.ViewPort.Width-100)
			//{
			//	dirRight = false;
			//}
			//if (xPos <= 0)
			//{
			//	dirRight = true;
			//}
			#endregion


		
			GraphicsDevice.SetRasterizerState(rs);

			spriteBatch.Begin(scale, SortMode.Immediate);

			spriteBatch.Submit(texture, new Vector2(100, 100), new Vector2(100, 100), Color.White);
			
			spriteBatch.SubmitString(sf,
				"Purple Haze all in my brain |n lately things don't seem the same, |n |n actin' funny but I don't know why |n 'scuse me while I kiss the sky." +
				"Purple Haze all in my brain |n lately things don't seem the same, |n |n actin' funny but I don't know why |n |n'scuse me while I kiss the sky." +
				"Purple Haze all in my brain |n lately things don't seem the same, |n |n actin' funny but I don't know why |n |n 'scuse me while I kiss the sky." +
				"|n + / * - , 0 1234 567890",
				new Vector2(400, 200), Color.ForestGreen);

			////spriteBatch.SubmitString(sf, "TEST", new Vector2(200, 200), Color.Whbi);

			spriteBatch.SubmitString(sf, "int main(string[] args...) |n { |n   " +  CurrentFps +"|n };", new Vector2(600,50), Color.Peru);
			///
			//spriteBatch.SubmitString(sf, CurrentFps.ToString(), new Vector2(200, 200), Color.White);

			spriteBatch.End();



			//triangle.Render(Color.White.ToVector3());




			//gManager.Draw(scale);
		}

	}
}
