using LeaFramework.Content;
using LeaFramework.Effect;
using LeaFramework.Game;
using LeaFramework.Game.SpriteBatch;
using LeaFramework.Graphics;
using LeaFramework.Input;
using LeaFramework.PlayGround.Misc;


using SharpDX;
using SharpDX.Direct3D11;
using Color = SharpDX.Color;


namespace LeaFramework.PlayGround
{
   public struct HeightData
    {
        public float Data;

        public HeightData(float data)
        {
            Data = data;
        }
    }

    public class Game01 : LeaPlanet.src.Game
	{
		private SpriteBatch spriteBatch;
	    private SpaceCam cam;
	    private RasterizerState rs;
	 
        private SpriteFont sf;
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
            InputManager.Listen(RenderForm);

            var desc = new RasterizerStateDescription
            {
                CullMode = CullMode.Front,
                FillMode = FillMode.Wireframe,
                IsMultisampleEnabled = true
            };

            rs = new RasterizerState(GraphicsDevice.NatiDevice1.D3D11Device, desc);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            sf = new SpriteFont(GraphicsDevice, "Roboto-Black.ttf", 24);
         

        }

		public override void Unload()
		{
			ContentManager.Instance.Dispose();
		}
		
		public override void Update(GameTimer gameTime)
		{
		 
          
		}

		public override void Render(GameTimer gameTime)
		{
		   GraphicsDevice.Clear(ClearFlags.RenderTarget | ClearFlags.DepthBuffer, Color.Black);
           spriteBatch.Begin(Matrix.Identity);
           spriteBatch.SubmitString(sf, "Test", new Vector2(100,100),Color.White );
           spriteBatch.End();
       
		}
	}
}
