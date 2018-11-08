using System;
using System.Collections.Generic;
using LeaFramework.Graphics;
using SharpDX;
using SharpDX.Direct3D11;

namespace LeaFramework.Effect
{
	public class LeaEffect : IDisposable
	{
		private readonly List<ShaderBase> shaders = new List<ShaderBase>();
		private readonly GraphicsDevice graphicsDevice;


		public LeaEffect(GraphicsDevice device, EffectCreateInfo createInfo, string vsDebugName = "", string psDebugName = "", string gsDebugName = "")
		{
			graphicsDevice = device;

			if (!string.IsNullOrEmpty(createInfo.VertexShaderPath)  || !string.IsNullOrEmpty(createInfo.VertexShaderBlob))
			{
				var vertexShader = new EVertexShader(graphicsDevice, createInfo, vsDebugName);
				shaders.Add(vertexShader);
			}

			if (!string.IsNullOrEmpty(createInfo.PixelShaderPath) || !string.IsNullOrEmpty(createInfo.PixelShaderBlob))
			{
				var pixelShader = new EPixelShader(graphicsDevice, createInfo, psDebugName);
				shaders.Add(pixelShader);
			}

			if (!string.IsNullOrEmpty(createInfo.GeometryShaderPath) || !string.IsNullOrEmpty(createInfo.GeometryShaderBlob))
			{
				var geometryShader = new EGeometryShader(graphicsDevice, createInfo, gsDebugName);
				shaders.Add(geometryShader);
			}

			if (!string.IsNullOrEmpty(createInfo.ComputeShaderPath))
			{
				Console.WriteLine();
			}
		}

		private EVertexShader GetVertexShader()
		{
			return shaders.Find(s => s.shaderType == ShaderType.VertexShader) as EVertexShader;
		}

		private EPixelShader GetPixelShader()
		{
			return shaders.Find(s => s.shaderType == ShaderType.PixelShader) as EPixelShader;
		}

		private EGeometryShader GetGeometryShader()
		{
			return shaders.Find(s => s.shaderType == ShaderType.GeometryShader) as EGeometryShader;
		}

		public void SetVariable(string name, string constanBuffer, Matrix value,  ShaderType shaderType)
		{
			switch (shaderType)
			{
				case ShaderType.VertexShader:
					GetVertexShader().GetConstantBuffer(constanBuffer).SetVariable(name, value);
					break;
				case ShaderType.PixelShader:
					GetPixelShader().GetConstantBuffer(constanBuffer).SetVariable(name, value);
					break;
				case ShaderType.GeometryShader:
					GetGeometryShader().GetConstantBuffer(constanBuffer).SetVariable(name, value);
					break;
				default:
					throw new Exception("ShaderStage not supporter yet");
			}
		}

		public void SetVariable(string name,  string constanBuffer, Vector3 value,  ShaderType shaderType)
		{
			switch (shaderType)
			{
				case ShaderType.VertexShader:
					GetVertexShader().GetConstantBuffer(constanBuffer).SetVariable(name, value);
					break;
				case ShaderType.PixelShader:
					GetPixelShader().GetConstantBuffer(constanBuffer).SetVariable(name, value);
					break;
				case ShaderType.GeometryShader:
					GetGeometryShader().GetConstantBuffer(constanBuffer).SetVariable(name, value);
					break;
				default:
					throw new Exception("ShaderStage not supporter yet");
			}
		}

		public void SetVariable(string name, string constanBuffer, float value, ShaderType shaderType)
		{
			switch (shaderType)
			{
				case ShaderType.VertexShader:
					GetVertexShader().GetConstantBuffer(constanBuffer).SetVariable(name, value);
					break;
				case ShaderType.PixelShader:
					GetPixelShader().GetConstantBuffer(constanBuffer).SetVariable(name, value);
					break;
				case ShaderType.GeometryShader:
					GetGeometryShader().GetConstantBuffer(constanBuffer).SetVariable(name, value);
					break;
				default:
					throw new Exception("ShaderStage not supporter yet");
			}
		}

		public void SetVariable(string name, string constanBuffer, int value, ShaderType shaderType)
		{
			switch (shaderType)
			{
				case ShaderType.VertexShader:
					GetVertexShader().GetConstantBuffer(constanBuffer).SetVariable(name, value);
					break;
				case ShaderType.PixelShader:
					GetPixelShader().GetConstantBuffer(constanBuffer).SetVariable(name, value);
					break;
				case ShaderType.GeometryShader:
					GetGeometryShader().GetConstantBuffer(constanBuffer).SetVariable(name, value);
					break;
				default:
					throw new Exception("ShaderStage not supporter yet");
			}
		}

		public void SetSampler(LeaSamplerState sampler, int slot, ShaderType shaderType)
		{
			switch (shaderType)
			{
				case ShaderType.VertexShader:
					GetVertexShader().SetTextureSampler(sampler.NativeSampler, slot);
					break;
				case ShaderType.PixelShader:
					GetPixelShader().SetTextureSampler(sampler.NativeSampler, slot);
					break;
				default:
					throw new Exception("ShaderStage not supporter yet");
			}
		}

		public void SetTexture(ShaderResourceView texture, int slot, ShaderType shaderType)
		{
			switch (shaderType)
			{
				case ShaderType.VertexShader:
					GetVertexShader().SetTexture(texture, slot, shaderType);
					break;
				case ShaderType.PixelShader:
					GetPixelShader().SetTexture(texture, slot, shaderType);
					break;
				case ShaderType.GeometryShader:
					GetGeometryShader().SetTexture(texture, slot, shaderType);
					break;
				default:
					throw new Exception("ShaderStage not supporter yet");
			}
		}


		public void Apply()
		{			
			ResetShaders();
			SetShaders();
			UpdateAndSetConstantBuffers();
		}

		private void UpdateAndSetConstantBuffers()
		{
			foreach (var shader in shaders)
			{
				shader.UpdateConstantBuffers();
				
				shader.SetConstantBuffers();
			}
		}

		private void ResetShaders()
		{
			if(GetVertexShader() == null)
				graphicsDevice.SetVertexShader(null);
			if (GetGeometryShader() == null)
				graphicsDevice.SetGeometryShader(null);
			if (GetPixelShader() == null)
				graphicsDevice.SetPixelShader(null);
		}

		private void SetShaders()
		{
			foreach (var shader in shaders)
			{			
				switch (shader.shaderType)
				{
					case ShaderType.VertexShader:
					{
						if (shader is EVertexShader vertexShader)
						{
							graphicsDevice.SetVertexShader(vertexShader.vertexShader);
							graphicsDevice.SetInputLayout(vertexShader.inputLayout);
						}
						break;
					}

					case ShaderType.PixelShader:
					{
						if (shader is EPixelShader pixelShader)
							graphicsDevice.SetPixelShader(pixelShader.PixelShader);
						break;
					}
					case ShaderType.GeometryShader:
					{
						if (shader is EGeometryShader geometryShader)
							graphicsDevice.SetGeometryShader(geometryShader.GeometryShader);		
						break;
					}
				}
			}
		}

		public void Dispose()
		{
			foreach (var shader in shaders)
			{
				shader.Dispose();
			}
		}
	}
}
