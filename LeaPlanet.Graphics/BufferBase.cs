using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace LeaFramework.Graphics
{
	public abstract class BufferBase
	{
		protected Buffer buffer;
		protected GraphicsDevice graphicsDevice;
		protected string debugName;
		protected bool isDynamic;
		public Buffer Buffer => buffer;


	}
}
