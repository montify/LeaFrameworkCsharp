using System;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace LeaFramework.Graphics
{
	public class IndexBuffer : BufferBase, IDisposable
	{
		public Format Format { get; }
		
		public IndexBuffer(GraphicsDevice graphicsDevice, IndexBufferFormat format, string debugName = "")
		{
			base.graphicsDevice = graphicsDevice;

#if DEBUG
			base.debugName = debugName;
#endif
			switch (format)
			{
				case IndexBufferFormat.DXGI_FORMAT_R16_UINT:
					Format = Format.R16_UInt;
					break;
				case IndexBufferFormat.DXGI_FORMAT_R32_UINT:
					Format = Format.R32_UInt;
					break;
				default:
					throw new Exception("Format not supported!");
			}
			
		}

		public void SetData(uint[] indices)
		{
			buffer = Buffer.Create(graphicsDevice.NatiDevice1.D3D11Device, BindFlags.IndexBuffer, indices);
			buffer.DebugName = debugName;
		}

	
		public void Dispose()
		{
			Utilities.Dispose(ref buffer);
		}
	}
}
