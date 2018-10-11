using System;
using System.Collections.Generic;
using LeaFramework.Graphics;
using SharpDX;
using SharpDX.Direct3D11;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace LeaFramework.Effect
{
	public class EConstantBuffer : IDisposable
	{
		internal GraphicsDevice graphicsDevice;
		internal string Name;
		internal int Size;
		internal DataBuffer backBuffer;
		internal Buffer constantBuffer;
		internal Dictionary<string, ConstantBufferVariable> constantBufferVariable = new Dictionary<string, ConstantBufferVariable>();
		
		internal bool IsDirty;

		public EConstantBuffer(string name, GraphicsDevice graphicsDevice)
		{
			this.graphicsDevice = graphicsDevice;
			this.Name = name;
		}

		// Used Intern for Adding the ShaderVariable (ShaderProgramm.cs) generated due Reflection
		internal void AddConstantBufferVariable(string name, ConstantBufferVariable variable)
		{
			Size += variable.Size;

			constantBufferVariable.Add(name, variable);
		}

		internal void CreateMemoryAndConstantBuffer()
		{
			backBuffer = new DataBuffer(Size);
			Utilities.ClearMemory(backBuffer.DataPointer, 0x0, Size);
			constantBuffer = new Buffer(graphicsDevice.NatiDevice1.D3D11Device, Size, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
		}

		public void SetVariable(string name, Vector3 value)
		{
			var x = constantBufferVariable[name];

			if (value != backBuffer.Get<Vector3>(x.Offset))
			{
				IsDirty = true;

				backBuffer.Set(x.Offset, value);
			}
		}

		public void SetVariable(string name, Matrix value)
		{
			var x = constantBufferVariable[name];

			if (value != backBuffer.Get<Matrix>(x.Offset))
			{
				IsDirty = true;
				backBuffer.Set(x.Offset, value);
			}
		}

		public void SetVariable(string name, float value)
		{
			var x = constantBufferVariable[name];

			if (value != backBuffer.Get<float>(x.Offset))
			{
				IsDirty = true;

				backBuffer.Set(x.Offset, value);
			}
		}

		public void SetVariable(string name, int value)
		{
			var x = constantBufferVariable[name];

			if (value != backBuffer.Get<int>(x.Offset))
			{
				IsDirty = true;

				backBuffer.Set(x.Offset, value);
			}
		}

		public void Dispose()
		{
			Utilities.Dispose(ref constantBuffer);
			Utilities.Dispose(ref backBuffer);
		}
	}
}
