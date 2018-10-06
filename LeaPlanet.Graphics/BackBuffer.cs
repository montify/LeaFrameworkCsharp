using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Windows;
using Resource = SharpDX.Direct3D11.Resource;
using Device1 = SharpDX.Direct3D11.Device1;

namespace LeaFramework.Graphics
{
	public class BackBuffer : IDisposable
	{
		public SwapChain1 SwapChain { get; private set; }
		public Texture2D BackBufferTexture { get; private set; }
		private SwapChainDescription1 swapChainDescription1;

	

		public BackBuffer(Factory2 factory, Device1 device1, RenderForm renderForm)
		{

			swapChainDescription1 = new SwapChainDescription1()
			{
				Width = renderForm.ClientSize.Width,
				Height = renderForm.ClientSize.Height,
				Format = Format.R8G8B8A8_UNorm,
				Stereo = false,
				SampleDescription = new SampleDescription(1, 0),
				Usage = Usage.RenderTargetOutput,
				BufferCount = 1,
				SwapEffect = SwapEffect.Discard,
				Flags = SwapChainFlags.AllowModeSwitch,

			};

			SwapChain = new SwapChain1(factory,
				device1,
				renderForm.Handle,
				ref swapChainDescription1,
				new SwapChainFullScreenDescription()
				{
					RefreshRate = new Rational(60, 1),
					Scaling = DisplayModeScaling.Centered,
					Windowed = true
				}, null);

			BackBufferTexture = Resource.FromSwapChain<Texture2D>(SwapChain, 0);


		}

		public void Resize(int width, int height)
		{
			BackBufferTexture.Dispose();
			SwapChain.ResizeBuffers(swapChainDescription1.BufferCount, width, height, Format.Unknown, SwapChainFlags.None);
			BackBufferTexture = Resource.FromSwapChain<Texture2D>(SwapChain, 0);
		}

		public void Dispose()
		{
			SwapChain.Dispose();
			BackBufferTexture.Dispose();
		
		}
	}
}
