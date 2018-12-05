// This is a personal academic project. Dear PVS-Studio, please check it.

// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Runtime.InteropServices;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace LeaFramework.Graphics
{
	public class VertexBuffer : BufferBase
	{
		public VertexBufferBinding VertexBufferBinding { get; private set; }
	
		public VertexBuffer(GraphicsDevice graphicsDevice, BufferUsage bufferType)
		{
			base.bufferType = bufferType;
			base.graphicsDevice = graphicsDevice;
			binfFlags = BindFlags.VertexBuffer;
		}

		public void SetData<T>(T[] vertices) where T : struct
		{
			base.SizeInBytes = Utilities.SizeOf(vertices);
		

			if (bufferType == BufferUsage.Normal)
			{
				buffer = Buffer.Create(graphicsDevice.NatiDevice1.D3D11Device, binfFlags, vertices);
			}
			else
			{
				buffer = Buffer.Create(graphicsDevice.NatiDevice1.D3D11Device, binfFlags, vertices,
					Utilities.SizeOf(vertices), ResourceUsage.Dynamic,
					CpuAccessFlags.Write);
			}

			VertexBufferBinding = new VertexBufferBinding(buffer, Utilities.SizeOf<T>(), 0);
		}

	}
}
