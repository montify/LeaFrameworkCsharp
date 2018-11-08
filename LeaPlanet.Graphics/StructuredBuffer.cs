using SharpDX;
using SharpDX.Direct3D11;
using System;
using Buffer = SharpDX.Direct3D11.Buffer;


namespace LeaFramework.Graphics
{
	public class StructuredBuffer : BufferBase
	{
		private UnorderedAccessView uav;

		public UnorderedAccessView UAV => uav;

		public StructuredBuffer(GraphicsDevice graphicsDevice)
		{
			base.graphicsDevice = graphicsDevice;
			bufferType = BufferType.Dynamic;
		}

		public void Create(int sizeInBytes, int stride)
		{
			base.SizeInBytes = sizeInBytes;

			base.buffer = new Buffer(graphicsDevice.NatiDevice1.D3D11Device, sizeInBytes, ResourceUsage.Default,
				BindFlags.ShaderResource | BindFlags.UnorderedAccess, CpuAccessFlags.None,
				ResourceOptionFlags.BufferStructured, stride);

			CreateUAV(stride);
		}

		private void CreateUAV(int stride)
		{
			uav = new UnorderedAccessView(graphicsDevice.NatiDevice1.D3D11Device, buffer);
		}

	
	}
}
