using System.IO;
using LeaFramework.Graphics;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;

namespace LeaFramework.Effect
{
	public class EVertexShader : ShaderBase
	{
		public VertexShader vertexShader;
		public InputLayout inputLayout;
		

		public EVertexShader(GraphicsDevice graphicsDevice, EffectCreateInfo createInfo, string debugName = "")
		{
			base.graphicsDevice = graphicsDevice;
			
			shaderType = ShaderType.VertexShader;

#if DEBUG
			base.debugName = debugName;
#endif

			// if Blob is 0, try to use File instead
			if (string.IsNullOrEmpty(createInfo.VertexShaderBlob))
			{
				shaderByteCode = GetShaderByteCode(createInfo.VertexShaderPath, createInfo.VertexShaderEntryPoint, "vs_5_0");
			}
			else
			{
				shaderByteCode = GetShaderByteCodeFromResource(createInfo.VertexShaderBlob, createInfo.VertexShaderEntryPoint, "vs_5_0");
			}

			shaderReflection = ReflectShaderByteCode();


			vertexShader = new VertexShader(graphicsDevice.NatiDevice1.D3D11Device, shaderByteCode);

			constantBuffers = ReflectConstantBuffers();

			GenerateInputLayout();
		}

		
		private void GenerateInputLayout()
		{
			var numberOfInputElements = shaderReflection.Description.InputParameters;

			var inputElements = new InputElement[numberOfInputElements];

			for (int i = 0; i < numberOfInputElements; i++)
			{
				var name = shaderReflection.GetInputParameterDescription(i).SemanticName;
				var index = shaderReflection.GetInputParameterDescription(i).SemanticIndex;
				var componentType = shaderReflection.GetInputParameterDescription(i).ComponentType;
				var mask = shaderReflection.GetInputParameterDescription(i).UsageMask;
				var format = Helpers.DetermineDXGIFormat(componentType, mask);

				inputElements[i] = new InputElement(name, index, format, InputElement.AppendAligned, 0);
			}

			inputLayout = new InputLayout(graphicsDevice.NatiDevice1.D3D11Device, shaderByteCode, inputElements);
		}

		internal override void CleanUp()
		{
			Utilities.Dispose(ref vertexShader);
			Utilities.Dispose(ref inputLayout);
		}
	}
}
