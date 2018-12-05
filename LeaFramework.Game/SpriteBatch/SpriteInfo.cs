using SharpDX;
using SharpDX.Direct3D11;
using System.Runtime.InteropServices;
namespace LeaFramework.Game.SpriteBatch
{

	public class SpriteInfo
	{
		public Vector2 position;
		public Vector2 size;
		public Vector4 color;
		public Vector2 offset;
		public ShaderResourceView srv;
		public int textureHashCode;
	}
}
