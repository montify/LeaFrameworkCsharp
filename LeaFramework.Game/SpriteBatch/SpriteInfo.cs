using SharpDX;
using SharpDX.Direct3D11;

namespace LeaFramework.Game.SpriteBatch
{
	public struct SpriteInfo
	{
		public Vector2 position;
		public Vector2 size;
		public Vector4 color;
		public ShaderResourceView srv;
		public int textureID;
		public Vector2 offset;
	}
}
