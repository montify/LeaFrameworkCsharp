using System;
using System.Collections.Generic;
using LeaFramework.Graphics;
using SharpDX;

namespace LeaFramework.Effect
{
	public class EConstantBuffer : IDisposable
	{
		internal GraphicsDevice graphicsDevice;
		internal string Name;
		internal int Size;
		internal DataBuffer backBuffer;
		internal ConstantBuffer constantBuffer;
		internal Dictionary<string, ConstantBufferVariable> constantBufferVariables = new Dictionary<string, ConstantBufferVariable>();
		
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

			constantBufferVariables.Add(name, variable);
		}

		internal void CreateBuffers()
		{
			constantBuffer = new ConstantBuffer(graphicsDevice);
			constantBuffer.Create(Size);

			backBuffer = new DataBuffer(Size);
			Utilities.ClearMemory(backBuffer.DataPointer, 0x0, Size);
		}

		public void SetVariable(string name, Vector3 value)
		{
			var x = constantBufferVariables[name];

			if (value != backBuffer.Get<Vector3>(x.Offset))
			{
				IsDirty = true;

				backBuffer.Set(x.Offset, value);
			}
		}

		public void SetVariable(string name, Matrix value)
		{
			var x = constantBufferVariables[name];

			if (value != backBuffer.Get<Matrix>(x.Offset))
			{
				IsDirty = true;
				backBuffer.Set(x.Offset, value);
			}
		}

		public void SetVariable(string name, float value)
		{
			var x = constantBufferVariables[name];

			if (value != backBuffer.Get<float>(x.Offset))
			{
				IsDirty = true;

				backBuffer.Set(x.Offset, value);
			}
		}

		public void SetVariable(string name, int value)
		{
			var x = constantBufferVariables[name];

			if (value != backBuffer.Get<int>(x.Offset))
			{
				IsDirty = true;

				backBuffer.Set(x.Offset, value);
			}
		}

		public void Dispose()
		{
			//Utilities.Dispose(ref constantBuffer);
			Utilities.Dispose(ref backBuffer);
			constantBuffer.Dispose();
		}
	}
}
