// This is a personal academic project. Dear PVS-Studio, please check it.

// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Drawing;
using System.Drawing.Imaging;

using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

namespace LeaFramework.Graphics
{
	public class LeaTexture2D : TextureBase, IDisposable
	{
		private Texture2D texture;


		private LeaTexture2D(GraphicsDevice graphicsDevice, Image image)
		{
			base.graphicsDevice = graphicsDevice;
			
			var rawImage = new Bitmap(image);
			var data = rawImage.LockBits(
				new System.Drawing.Rectangle(0, 0, rawImage.Width, rawImage.Height),
				ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

			base.Width = rawImage.Width;
			base.Height = rawImage.Height;

			texture = new Texture2D(graphicsDevice.NatiDevice1.D3D11Device, new Texture2DDescription()
			{
				Width = rawImage.Width,
				Height = rawImage.Height,
				ArraySize = 1,
				BindFlags = BindFlags.ShaderResource,
				Usage = ResourceUsage.Immutable,
				CpuAccessFlags = CpuAccessFlags.None,
				Format = Format.B8G8R8A8_UNorm,
				MipLevels = 1,
				OptionFlags = ResourceOptionFlags.None,
				SampleDescription = new SampleDescription(1, 0),
			}, new DataRectangle(data.Scan0, data.Stride));

			rawImage.UnlockBits(data);

			shaderResourceView = new ShaderResourceView(graphicsDevice.NatiDevice1.D3D11Device, texture);

		}

		public static LeaTexture2D Create(GraphicsDevice gDevice, Image image)
		{
			return new LeaTexture2D(gDevice, image);
		}
		
		public void Dispose()
		{
			Utilities.Dispose(ref texture);
			Utilities.Dispose(ref shaderResourceView);

		}
	}
}
