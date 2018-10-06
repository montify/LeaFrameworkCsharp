using SharpDX;

namespace LeaFramework.Graphics.VertexStructs
{
	public struct VertexPostitionColorTextureNormal
	{
		// Must match with the VS_Input struct!
		public Vector3 Position;
		public Vector4 Color;
		public Vector2 TexCoord;
		public Vector3 Normal;

		public VertexPostitionColorTextureNormal(Vector3 position, Vector4 color, Vector2 texCoord, Vector3 normal)
		{
			this.Position = position;
			this.Color = color;
			this.TexCoord = texCoord;
			this.Normal = normal;
		}
	}
}
