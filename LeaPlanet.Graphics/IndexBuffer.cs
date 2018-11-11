// This is a personal academic project. Dear PVS-Studio, please check it.

// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
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
		
		public IndexBuffer(GraphicsDevice graphicsDevice, Format format, string debugName = "")
		{
			base.graphicsDevice = graphicsDevice;
			this.Format = format;
			binfFlags = BindFlags.IndexBuffer;
			bufferType = BufferType.Normal;
#if DEBUG
			base.debugName = debugName;
#endif

		}

		public void SetData<T>(T[] indices) where T : struct
		{
			buffer = Buffer.Create(graphicsDevice.NatiDevice1.D3D11Device, binfFlags, indices);
			NativeBuffer.DebugName = debugName;
		}

	
		public void Dispose()
		{
			Utilities.Dispose(ref buffer);
		}
	}
}
