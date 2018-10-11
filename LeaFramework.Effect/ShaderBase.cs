using System;
using System.Collections.Generic;
using System.IO;
using LeaFramework.Graphics;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;

namespace LeaFramework.Effect
{
	public abstract class ShaderBase : IDisposable
	{
		internal GraphicsDevice graphicsDevice;
		internal ShaderBytecode shaderByteCode;
		internal ShaderReflection shaderReflection;
		internal Dictionary<string, EConstantBuffer> constantBuffers = new Dictionary<string, EConstantBuffer>();
		internal string debugName;
		internal ShaderType shaderType;
		private SamplerState currentSamplerState;

		internal ShaderBytecode GetShaderByteCode(string path, string entryPoint, string profile)
		{
			ShaderFlags flags = ShaderFlags.None;
#if DEBUG
			 flags = ShaderFlags.Debug | ShaderFlags.SkipOptimization;
#endif
			
			return ShaderBytecode.CompileFromFile(path, entryPoint, profile, flags);
		}


		internal ShaderBytecode GetShaderByteCodeFromResource(string shaderBlob, string entryPoint, string profile)
		{
			ShaderFlags flags = ShaderFlags.None;
#if DEBUG
			flags = ShaderFlags.Debug | ShaderFlags.SkipOptimization;
#endif
			return ShaderBytecode.Compile(shaderBlob, entryPoint, profile, flags);
		}

		internal ShaderReflection ReflectShaderByteCode()
		{
			return new ShaderReflection(shaderByteCode);
		}

		internal Dictionary<string, EConstantBuffer> ReflectConstantBuffers()
		{
			// MARK BUFFER IS DIRTY, SO ONLY CHANGED BUFFERS/VARIABLE GET UPDATED
			var numberOfConstantBuffers = shaderReflection.Description.ConstantBuffers;

			var resList = new Dictionary<string, EConstantBuffer>();

			for (int i = 0; i < numberOfConstantBuffers; i++)
			{
				var constantBuffer = shaderReflection.GetConstantBuffer(i);
				
				EConstantBuffer c = new EConstantBuffer(constantBuffer.Description.Name, graphicsDevice);

				for (int x = 0; x < constantBuffer.Description.VariableCount; x++)
				{
					var name = constantBuffer.GetVariable(x).Description.Name;
					var size = constantBuffer.GetVariable(x).Description.Size;
					var offset = constantBuffer.GetVariable(x).Description.StartOffset;

					c.AddConstantBufferVariable(name, new ConstantBufferVariable(name, size, offset));
				}

				resList.Add(constantBuffer.Description.Name, c);
			}

			foreach (var cbuffers in resList)
			{
				cbuffers.Value.CreateMemoryAndConstantBuffer();
			}

			return resList;
		}


		public EConstantBuffer GetConstantBuffer(string name)
		{
			return constantBuffers[name];
		}


		public void SetTexture(ShaderResourceView srv, int slot, ShaderType shaderType)
		{
			switch (shaderType)
			{
				case ShaderType.VertexShader:
					graphicsDevice.NatiDevice1.D3D11Device.ImmediateContext1.VertexShader.SetShaderResource(slot, srv);
					break;
				case ShaderType.PixelShader:
					graphicsDevice.NatiDevice1.D3D11Device.ImmediateContext1.PixelShader.SetShaderResource(slot, srv);
					break;
				case ShaderType.GeometryShader:
					graphicsDevice.NatiDevice1.D3D11Device.ImmediateContext1.GeometryShader.SetShaderResource(slot, srv);
					break;
				default:
					throw new Exception("ShaderStage not supporter yet");
			}

		}

		public void SetTextureSampler(SamplerState samplerState, int slot)
		{
			if (samplerState != currentSamplerState)
			{
				graphicsDevice.NatiDevice1.D3D11Device.ImmediateContext1.PixelShader.SetSampler(slot, samplerState);
				currentSamplerState = samplerState;
			}
			
		}

		internal void UpdateConstantBuffers()
		{
			foreach (var constantBuffer in constantBuffers)
			{
				if (constantBuffer.Value.IsDirty)
				{
					var mappedConstantBuffer = graphicsDevice.NatiDevice1.D3D11Device.ImmediateContext1.MapSubresource(constantBuffer.Value.constantBuffer, 0, MapMode.WriteDiscard, MapFlags.None);

					// Copy Memory from BackBuffer to the mappedConstantBuffer
					Utilities.CopyMemory(mappedConstantBuffer.DataPointer, constantBuffer.Value.backBuffer.DataPointer, constantBuffer.Value.Size);

					graphicsDevice.NatiDevice1.D3D11Device.ImmediateContext1.UnmapSubresource(constantBuffer.Value.constantBuffer, 0);

				}				
			}
		}

		internal void SetConstantBuffers()
		{
			int shaderSlot = 0;

			// TODO: Find out if Constanbuffer in slots are differen/changes and only Update when needed

			
			foreach (var constantBuffer in constantBuffers)
			{
				if (constantBuffer.Value.IsDirty || graphicsDevice.IsShaderSwitchHappen)
				{
					if (shaderType == ShaderType.VertexShader)
						graphicsDevice.NatiDevice1.D3D11Device.ImmediateContext1.VertexShader.SetConstantBuffer(shaderSlot, constantBuffer.Value.constantBuffer);


					if (shaderType == ShaderType.PixelShader)
						graphicsDevice.NatiDevice1.D3D11Device.ImmediateContext1.PixelShader.SetConstantBuffer(shaderSlot, constantBuffer.Value.constantBuffer);


					if (shaderType == ShaderType.GeometryShader)
						graphicsDevice.NatiDevice1.D3D11Device.ImmediateContext1.GeometryShader.SetConstantBuffer(shaderSlot, constantBuffer.Value.constantBuffer);
				}

				constantBuffer.Value.IsDirty = false;

				shaderSlot++;
			}
		}

		internal abstract void CleanUp();

		public void Dispose()
		{

			foreach (var effectConstantBuffer in constantBuffers)
			{
				effectConstantBuffer.Value.Dispose();
			}

			Utilities.Dispose(ref shaderByteCode);
			Utilities.Dispose(ref shaderReflection);
			

			CleanUp();
		}
	}
}
