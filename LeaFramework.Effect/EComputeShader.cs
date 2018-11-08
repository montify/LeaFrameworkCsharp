using LeaFramework.Graphics;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaFramework.Effect
{
	public class EComputeShader : IDisposable
	{
		private GraphicsDevice graphicsDevice;
		private ComputeShader computeShader;
		private ShaderBytecode shaderByteCode;
		ShaderFlags flags;

		public ComputeShader ComputeShader => computeShader;

		public EComputeShader(GraphicsDevice graphicsDevice)
		{
			this.graphicsDevice = graphicsDevice;
			flags = ShaderFlags.None;
#if DEBUG
			flags = ShaderFlags.Debug | ShaderFlags.SkipOptimization;
#endif
		}


		public void Load(string path)
		{
			if (!string.IsNullOrEmpty(path))
			{
				shaderByteCode = ShaderBytecode.CompileFromFile(path, "CSMain", "cs_5_0", flags);
			}

			computeShader = new ComputeShader(graphicsDevice.NatiDevice1.D3D11Device, shaderByteCode);
		}


		public void Dispose()
		{
			Utilities.Dispose(ref computeShader);
		}

	}
}
