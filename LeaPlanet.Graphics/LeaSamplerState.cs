// This is a personal academic project. Dear PVS-Studio, please check it.

// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;

namespace LeaFramework.Graphics
{
	public  class LeaSamplerState : IDisposable
	{
		private SamplerState nativeSamplerState;
		public SamplerState NativeSampler => nativeSamplerState;


		public  void GenerateSamplers(GraphicsDevice graphicsDevice)
		{
			var samplerStateDescription = new SamplerStateDescription
			{
				AddressU = TextureAddressMode.Wrap,
				AddressV = TextureAddressMode.Wrap,
				AddressW = TextureAddressMode.Wrap,
				Filter = Filter.MinMagMipLinear
			};

			nativeSamplerState = new SamplerState(graphicsDevice.NatiDevice1.D3D11Device, samplerStateDescription);
		}

		public void Dispose()
		{
			Utilities.Dispose(ref nativeSamplerState);
		}
	}
}
