using System.Threading.Tasks;
using LeaFramework.Content;
using LeaFramework.Effect;
using LeaFramework.Game;
using LeaFramework.Game.SpriteBatch;
using LeaFramework.Graphics;
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

		private RasterizerState1 rs, rs1;
		//private RenderTarget2D rt;
		private ShaderResourceView tex, tex1 ;
		private SpriteBatch spriteBatch;
		
		LeaTexture2D texture;
		int track;
		private float x, y = 10;
		float xPos;
		string text ;
		SpriteFont sf;
		EComputeShader computeShader;

		private bool dirRight = true;
		public float speed = 2.0f;

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

			sf = new SpriteFont(GraphicsDevice, "OpenSans-Regular.ttf", 25);
			computeShader = new EComputeShader(GraphicsDevice);

			computeShader.Load("Content//computeShaderTest.hlsl");

			triangle = new Triangle(GraphicsDevice);
			
			spriteBatch = new SpriteBatch(GraphicsDevice);
			


			Vector3[] test = { new Vector3(1, 1, 1), new Vector3(2, 2, 2) };

			//StructuredBuffer sb = new StructuredBuffer(GraphicsDevice);
			//sb.Create(Utilities.SizeOf(test), Utilities.SizeOf<Vector3>());
			//sb.UpdateBuffer(test, 0);
			//GraphicsDevice.SetUAV(0, sb.UAV);

			texture = ContentManager.Instance.Load<LeaTexture2D>(GraphicsDevice, "Content//uvTestTexSmall.png");

		
			
			


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
		//	triangle.Dispose();
		//	spriteBatch.Dispose();
		
			ContentManager.Instance.Dispose();
		}
		
		public override void Update(GameTimer gameTime)
		{
			//triangle.Update(gameTime, new Vector3(3, 0, 0));
		}

		public override void Render(GameTimer gameTime)
		{
			GraphicsDevice.Clear(ClearFlags.RenderTarget | ClearFlags.DepthBuffer, Color.CornflowerBlue);

			

			#region Input
			float screenscaleW = (float)GraphicsDevice.ViewPort.Width / 1280;
			float screenscaleH = (float)GraphicsDevice.ViewPort.Height / 720;
			var scale = Matrix.Scaling(screenscaleW, screenscaleH, 1);

			if (InputManager.Instance.IsKeyDown(Key.ESC)) IsRunning = false;

			if (InputManager.Instance.IsKeyDown(Key.W))
				y -= 300 * gameTime.DeltaTime;

			if (InputManager.Instance.IsKeyDown(Key.S))
				y += 300 * gameTime.DeltaTime;

			if (InputManager.Instance.IsKeyDown(Key.A))
				x -= 300 * gameTime.DeltaTime;
			if (InputManager.Instance.IsKeyDown(Key.D))
				x += 300 * gameTime.DeltaTime;


			RectangleF player = new RectangleF(x, y, 100, 100);
			RectangleF st = new RectangleF(xPos, 300, 100, 100);


			if (dirRight)
				xPos += 400 * gameTime.DeltaTime;
			else
				xPos -= 400 * gameTime.DeltaTime;


			if (xPos >= GraphicsDevice.ViewPort.Width-100)
			{
				dirRight = false;
			}
			if (xPos <= 0)
			{
				dirRight = true;
			}
			#endregion


		
			GraphicsDevice.SetRasterizerState(rs);
			


			spriteBatch.Begin(Matrix.Identity, SortMode.Immediate);
			
			//spriteBatch.Submit(texture, new Vector2(100, 100), Vector2.Zero, Color.White.ToVector4());
			spriteBatch.Submit(texture, new Vector2(100, 100), Vector2.Zero, Color.Green);
			spriteBatch.SubmitString(sf, "Was |n geht ab? |n Was hast du heute so gemacht? |n Lol ", new Vector2(150, 150), Color.Red);
		
			spriteBatch.End();


			//spriteFont.Begin(Matrix.Identity);
			//spriteFont.SubmitString("TEST ", new Vector2(120, 120), Color.Red);
			//spriteFont.Draw();


			
		



		}

	}
}
