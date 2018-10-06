using SharpDX.D3DCompiler;
using SharpDX.DXGI;

namespace LeaFramework.Effect
{
	internal static class Helpers
	{

		internal static Format DetermineDXGIFormat(RegisterComponentType componentType, RegisterComponentMaskFlags mask)
		{
			Format format = Format.Unknown;

			if (mask == RegisterComponentMaskFlags.ComponentX)
			{
				if (componentType == RegisterComponentType.UInt32) format = Format.R32_UInt;
				else if (componentType == RegisterComponentType.SInt32) format = Format.R32_SInt;
				else if (componentType == RegisterComponentType.Float32) format = Format.R32_Float;
			}
			else if (mask == (RegisterComponentMaskFlags.ComponentX | RegisterComponentMaskFlags.ComponentY))
			{
				if (componentType == RegisterComponentType.UInt32) format = Format.R32G32_UInt;
				else if (componentType == RegisterComponentType.SInt32) format = Format.R32G32_SInt;
				else if (componentType == RegisterComponentType.Float32) format = Format.R32G32_Float;
			}
			else if (mask == (RegisterComponentMaskFlags.ComponentX | RegisterComponentMaskFlags.ComponentY | RegisterComponentMaskFlags.ComponentZ))
			{
				if (componentType == RegisterComponentType.UInt32) format = Format.R32G32B32_UInt;
				else if (componentType == RegisterComponentType.SInt32) format = Format.R32G32B32_SInt;
				else if (componentType == RegisterComponentType.Float32) format = Format.R32G32B32_Float;
			}
			else if (mask == RegisterComponentMaskFlags.All)
			{
				if (componentType == RegisterComponentType.UInt32) format = Format.R32G32B32A32_UInt;
				else if (componentType == RegisterComponentType.SInt32) format = Format.R32G32B32A32_SInt;
				else if (componentType == RegisterComponentType.Float32) format = Format.R32G32B32A32_Float;
			}


			return format;
		}
	}
}
