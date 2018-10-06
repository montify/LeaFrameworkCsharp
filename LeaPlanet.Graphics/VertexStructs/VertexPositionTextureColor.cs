using SharpDX;

namespace LeaFramework.Graphics.VertexStructs
{
	public struct VertexPositionTextureColor
	{
		// Must match with the VS_Input struct!
		public Vector3 Position;
		public Vector4 Color;
		public Vector2 TexCoord;
		

		public VertexPositionTextureColor(Vector3 position, Vector2 texCoord, Vector4 color)
		{
			this.Position = position;
			this.Color = color;
			this.TexCoord = texCoord;
		}
	}
}
