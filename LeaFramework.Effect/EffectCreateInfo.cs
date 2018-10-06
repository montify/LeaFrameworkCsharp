namespace LeaFramework.Effect
{
	public struct EffectCreateInfo
	{
		public string VertexShaderPath;
		public string VertexShaderEntryPoint;

		public string PixelShaderPath;
		public string PixelShaderEntryPoint;

		public string GeometryShaderPath;
		public string GeometryShaderEntryPoint;

		public string ComputeShaderPath;
		public string ComputeShaderEntryPoint;

		public string VertexShaderBlob;
		public string GeometryShaderBlob;
		public string PixelShaderBlob;
	}

}
