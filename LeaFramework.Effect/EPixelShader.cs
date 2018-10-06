using LeaFramework.Graphics;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;

namespace LeaFramework.Effect
{
	public class EPixelShader : ShaderBase
	{
		internal PixelShader PixelShader;

		public EPixelShader(GraphicsDevice graphicsDevice, EffectCreateInfo createInfo, string debugName = "")
		{
			base.graphicsDevice = graphicsDevice;
		
			shaderType = ShaderType.PixelShader;

			if (string.IsNullOrEmpty(createInfo.PixelShaderBlob))
			{
				shaderByteCode = GetShaderByteCode(createInfo.PixelShaderPath, createInfo.PixelShaderEntryPoint, "ps_5_0");
			}
			else
			{
				shaderByteCode = GetShaderByteCodeFromResource(createInfo.PixelShaderBlob, createInfo.PixelShaderEntryPoint, "ps_5_0");
			}
				
			shaderReflection = ReflectShaderByteCode();
			
#if DEBUG
			base.debugName = debugName;
#endif
			constantBuffers = ReflectConstantBuffers();
			PixelShader = new PixelShader(graphicsDevice.NatiDevice1.D3D11Device, shaderByteCode);
		}



		internal override void CleanUp()
		{	
			Utilities.Dispose(ref PixelShader);
		}
	}
}
