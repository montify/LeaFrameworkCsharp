using System;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

namespace LeaFramework.Graphics
{
	public class LeaTexture2D : TextureBase, IDisposable
	{
		internal Texture2DDescription desc;
		private Texture2D texture;

		public LeaTexture2D(int width, int height, DataRectangle data, GraphicsDevice graphicsDevice, BindFlags bindFlags)
		{
			base.Width = width;
			base.Height = height;

			CreateTextureDescription(width, height, bindFlags);
			
			texture = new Texture2D(graphicsDevice.NatiDevice1.D3D11Device, desc, data);

			shaderResourceView = new ShaderResourceView(graphicsDevice.NatiDevice1.D3D11Device, texture);
		}

		private void CreateTextureDescription(int width, int height, BindFlags bindFlags)
		{
			desc.Width = width;
			desc.Height = height;
			desc.ArraySize = 1;
			desc.BindFlags = bindFlags;
			desc.Usage = ResourceUsage.Default;
			desc.CpuAccessFlags = CpuAccessFlags.None;
			desc.Format = Format.R8G8B8A8_UNorm;
			desc.MipLevels = 1;
			desc.OptionFlags = ResourceOptionFlags.None;
			desc.SampleDescription.Count = 1;
			desc.SampleDescription.Quality = 0;
		}

		public void Dispose()
		{
			Utilities.Dispose(ref texture);
			Utilities.Dispose(ref shaderResourceView);
		}
	}
}
