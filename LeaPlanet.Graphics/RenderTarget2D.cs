//using SharpDX;
//using SharpDX.Direct3D11;
//using SharpDX.DXGI;


//namespace LeaFramework.Graphics
//{
//	public class RenderTarget2D : LeaTexture2D
//	{
		

//		public RenderTargetView RenderTargetTextureView { get; private set; }
//		public ShaderResourceView RenderTargetSRV { get; private set; }

//	//	public RenderTarget2D(int width, int height, DataRectangle data, GraphicsDevice graphicsDevice) 
//	//		: base(width, height, data, graphicsDevice, BindFlags.ShaderResource| BindFlags.RenderTarget)
//	//	{
			
//	//	}
//	//}

//	/*	public RenderTarget2D(GraphicsDevice graphicsDevice, Format format, int width, int height)
//		{

//		renderTargetTextureDesc = new Texture2DDescription
//				{
//					Width = width,
//					Height = height,
//					MipLevels = 1,
//					ArraySize = 1,
//					Format = format,
//					SampleDescription = new SampleDescription(1, 0),
//					Usage = ResourceUsage.Default,
//					BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
//					CpuAccessFlags = CpuAccessFlags.None,

//				};

//				renderTargetTexture = new Texture2D(graphicsDevice.NatiDevice1.D3D11Device, renderTargetTextureDesc);

//				var renderTargetViewDesc = new RenderTargetViewDescription
//				{
//					Format = renderTargetTextureDesc.Format,
//					Dimension = RenderTargetViewDimension.Texture2D,
//				};

//				RenderTargetTextureView = new RenderTargetView(graphicsDevice.NatiDevice1.D3D11Device, renderTargetTexture, renderTargetViewDesc);

//				RenderTargetSRV = new ShaderResourceView(graphicsDevice.NatiDevice1.D3D11Device, renderTargetTexture);
//		}*/

////}
