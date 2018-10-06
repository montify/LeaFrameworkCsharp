using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeaFramework.Graphics;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;

namespace LeaFramework.Effect
{
	public class EGeometryShader : ShaderBase
	{

		internal GeometryShader GeometryShader;

		public EGeometryShader(GraphicsDevice graphicsDevice, EffectCreateInfo createInfo, string debugName = "")
		{
			base.graphicsDevice = graphicsDevice;

			shaderType = ShaderType.GeometryShader;

			if (string.IsNullOrEmpty(createInfo.GeometryShaderBlob))
			{
				shaderByteCode = GetShaderByteCode(createInfo.GeometryShaderPath, createInfo.GeometryShaderEntryPoint, "gs_5_0");
			}
			else
			{
				shaderByteCode = GetShaderByteCodeFromResource(createInfo.GeometryShaderBlob, createInfo.GeometryShaderEntryPoint, "gs_5_0");
			}


			shaderReflection = ReflectShaderByteCode();

#if DEBUG
			base.debugName = debugName;
#endif
			constantBuffers = ReflectConstantBuffers();

			GeometryShader = new GeometryShader(graphicsDevice.NatiDevice1.D3D11Device, shaderByteCode);
		}

		internal override void CleanUp()
		{
			Utilities.Dispose(ref GeometryShader);
		}
	}
}
