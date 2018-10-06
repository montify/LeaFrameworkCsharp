using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct3D11;

namespace LeaFramework.Graphics
{
	public abstract class TextureBase
	{
		protected GraphicsDevice graphicsDevice;

		public int Width { get; protected set; }
		public int Height { get; protected set; }
		protected ShaderResourceView shaderResourceView;

		public ShaderResourceView ShaderResourceView => shaderResourceView;


	}
}
