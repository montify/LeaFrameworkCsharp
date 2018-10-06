using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

namespace LeaFramework.Graphics
{
	public class DepthBuffer : IDisposable
	{
		private Texture2D depthBufferTexture;
		private DepthStencilView depthStencilView;

		public Texture2D DepthBufferTexture => depthBufferTexture;
		public DepthStencilView DepthStencilView => depthStencilView;

		public DepthBuffer(NativeDevice nativeDevice, int width, int height)
		{
			depthBufferTexture = new Texture2D(nativeDevice.D3D11Device, new Texture2DDescription()
			{
				Format = Format.D32_Float_S8X24_UInt,
				ArraySize = 1,
				MipLevels = 1,
				Width = width,
				Height = height,
				SampleDescription = new SampleDescription(1, 0),
				Usage = ResourceUsage.Default,
				BindFlags = BindFlags.DepthStencil,
				CpuAccessFlags = CpuAccessFlags.None,
				OptionFlags = ResourceOptionFlags.None
			});

			depthStencilView = new DepthStencilView(nativeDevice.D3D11Device, depthBufferTexture);
		}

		public void Dispose()
		{
			Utilities.Dispose(ref depthBufferTexture);
			Utilities.Dispose(ref depthStencilView);
		}
	}
}
