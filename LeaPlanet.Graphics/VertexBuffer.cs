using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace LeaFramework.Graphics
{
	public class VertexBuffer : BufferBase, IDisposable
	{
		public VertexBufferBinding VertexBufferBinding { get; private set; }
		private BufferDescription bufferDesc;
	

		public VertexBuffer(GraphicsDevice graphicsDevice, bool isDynamic,  string debugName = "")
		{
			base.isDynamic = isDynamic;
			base.graphicsDevice = graphicsDevice;
#if DEBUG
			base.debugName = debugName;
#endif
		}

		public void CreateAndSetData<T>(T[] vertices) where T : struct
		{
			if (buffer == null)
			{
				if (!isDynamic)
				{
					buffer = Buffer.Create(graphicsDevice.NatiDevice1.D3D11Device, BindFlags.VertexBuffer, vertices);
				}
				else
				{
					buffer = Buffer.Create(graphicsDevice.NatiDevice1.D3D11Device, BindFlags.VertexBuffer, vertices,
						Utilities.SizeOf(vertices) , ResourceUsage.Dynamic,
						CpuAccessFlags.Write);
				}

				buffer.DebugName = debugName;
				VertexBufferBinding = new VertexBufferBinding(buffer, Utilities.SizeOf<T>(), 0);
			}
		}

		public void UpdateBuffer<T>(T[] vertices, int offset) where T : struct
		{
			if (!isDynamic)
				throw new Exception("Buffer is not dynamic!");

			graphicsDevice.NatiDevice1.D3D11Device.ImmediateContext1.MapSubresource(buffer, MapMode.WriteDiscard, MapFlags.None, out var dataBox);

			Utilities.Write(dataBox.DataPointer, vertices, offset, vertices.Length);
		
			graphicsDevice.NatiDevice1.D3D11Device.ImmediateContext1.UnmapSubresource(buffer, 0);
		}

		public void Dispose()
		{
			Utilities.Dispose(ref buffer);
		}
	}
}
