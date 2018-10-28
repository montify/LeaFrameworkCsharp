using SharpDX.Direct3D11;

namespace LeaFramework.Graphics
{
	public class ConstantBuffer : BufferBase
	{
		public ConstantBuffer(GraphicsDevice graphicsDevice)
		{
			base.graphicsDevice = graphicsDevice;
			base.bufferType = BufferType.Dynamic;
			base.binfFlags = BindFlags.ConstantBuffer;
		}

		public void Create(int sizeInBytes) 
		{
			base.SizeInBytes = sizeInBytes;

			base.bufferDesc.BindFlags = binfFlags;
			base.bufferDesc.SizeInBytes = sizeInBytes;
			base.bufferDesc.Usage = ResourceUsage.Dynamic;
			base.bufferDesc.CpuAccessFlags = CpuAccessFlags.Write;

			base.buffer = new Buffer(graphicsDevice.NatiDevice1.D3D11Device, bufferDesc);
		}

	}
}
