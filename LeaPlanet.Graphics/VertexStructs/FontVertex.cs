﻿using SharpDX;


namespace LeaFramework.Graphics.VertexStructs
{
	public struct FontVertex
	{
		public Vector2 Position;
		public Vector2 Size;
		public Vector4 Color;
		public Vector2 Offset;
		

		public FontVertex(Vector2 position, Vector2 size, Vector4 color, Vector2 offset)
		{
			Position = position;
			Size = size;
			Color = color;
			Offset = offset;
			
		}
	}
}
