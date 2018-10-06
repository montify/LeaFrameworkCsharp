using SharpDX;

namespace LeaFramework.Graphics.VertexStructs
{
	public struct VertexPositionColor
	{
		// Must match with the VS_Input struct!
		public Vector3 Position;
		public Vector4 Color;
	
		public VertexPositionColor(Vector3 position, Vector4 color)
		{
			this.Position = position;
			this.Color = color;
			
		}

	}
}
