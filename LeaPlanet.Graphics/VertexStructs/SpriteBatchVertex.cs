using SharpDX;


namespace LeaFramework.Graphics.VertexStructs
{
	public struct SpriteBatchVertex
	{
		public Vector2 Position;
		public Vector2 Size;
		public Vector4 Color;
		public int TextureID;


		public SpriteBatchVertex(Vector2 position, Vector2 size, Vector4 color, int textureID)
		{
			Position = position;
			Size = size;
			Color = color;
			TextureID = textureID;

		}
	}
}
