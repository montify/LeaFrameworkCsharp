using LeaFramework.Content;
using LeaFramework.Effect;
using LeaFramework.Game;
using LeaFramework.Game.SpriteBatch;
using LeaFramework.Graphics;
using LeaFramework.Input;
using LeaFramework.PlayGround.Misc;
using LeaFramework.PlayGround.TerrainSrc;

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
	    private Terrain terrain;

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
        
            cam = new SpaceCam(new Vector3(51.78922f, 6007.563f, 21.74149f), true, GraphicsDevice, 11);
            Globals.cam = cam;
            terrain = new Terrain(GraphicsDevice);

            //ComputeShaderHelper CSHelper = new ComputeShaderHelper(GraphicsDevice.NatiDevice1.D3D11Device, "Content//PlanetShaders//noiseCs.hlsl");

            //HeightData[] data = new HeightData[65 * 65];
            //int index = CSHelper.SetData<HeightData>(data);
            
            //CSHelper.Execute(50);
            //var height = CSHelper.GetData<HeightData>(0);


        }

		public override void Unload()
		{
			ContentManager.Instance.Dispose();
		}
		
		public override void Update(GameTimer gameTime)
		{
		   cam.Update(gameTime.DeltaTime);
           terrain.Update();
		}

		public override void Render(GameTimer gameTime)
		{
		   GraphicsDevice.Clear(ClearFlags.RenderTarget | ClearFlags.DepthBuffer, Color.Black);

           GraphicsDevice.SetRasterizerState(rs);
		   terrain.Draw(cam);
		}
	}
}
