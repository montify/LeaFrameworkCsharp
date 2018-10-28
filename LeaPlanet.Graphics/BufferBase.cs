using System;
using SharpDX;
using SharpDX.Direct3D11;
using Buffer = SharpDX.Direct3D11.Buffer;
using MapFlags = SharpDX.Direct3D11.MapFlags;

namespace LeaFramework.Graphics
{
	public abstract class BufferBase : IDisposable
	{
		protected Buffer buffer;
		protected GraphicsDevice graphicsDevice;
		protected BufferDescription bufferDesc = new BufferDescription();
		public int SizeInBytes { get; protected set; }
		protected string debugName;
		protected BindFlags binfFlags;

		protected BufferType bufferType;
		public Buffer NativeBuffer => buffer;

		public void UpdateBuffer<T>(T[] data, int offset, int count) where T : struct
		{
			if (bufferType == BufferType.Normal)
				throw new Exception("Buffer is not dynamic!");

			graphicsDevice.NatiDevice1.D3D11Device.ImmediateContext1.MapSubresource(buffer, MapMode.WriteDiscard, MapFlags.None, out var dataBox);

			Utilities.Write(dataBox.DataPointer, data, offset, count);

			graphicsDevice.NatiDevice1.D3D11Device.ImmediateContext1.UnmapSubresource(buffer, 0);
		}
		public void UpdateBuffer<T>(T[] data, int offset) where T : struct
		{
			if (bufferType == BufferType.Normal)
				throw new Exception("Buffer is not dynamic!");

			graphicsDevice.NatiDevice1.D3D11Device.ImmediateContext1.MapSubresource(buffer, MapMode.WriteDiscard, MapFlags.None, out var dataBox);

			Utilities.Write(dataBox.DataPointer, data, offset, data.Length);

			graphicsDevice.NatiDevice1.D3D11Device.ImmediateContext1.UnmapSubresource(buffer, 0);
		}

		public void UpdateBuffer(IntPtr ptr) 
		{
			if (bufferType == BufferType.Normal)
				throw new Exception("Buffer is not dynamic!");

			var mappedConstantBuffer = graphicsDevice.NatiDevice1.D3D11Device.ImmediateContext1.MapSubresource(buffer, 0, MapMode.WriteDiscard, MapFlags.None);

			Utilities.CopyMemory(mappedConstantBuffer.DataPointer, ptr, SizeInBytes);

			graphicsDevice.NatiDevice1.D3D11Device.ImmediateContext1.UnmapSubresource(buffer, 0);
		}

		public void Dispose()
		{
			Utilities.Dispose(ref buffer);
		}
	}
}
