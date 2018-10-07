using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct3D11;
using SharpFont;

namespace LeaFramework.Game.SpriteBatch
{
	public struct Glyph
	{
		public ShaderResourceView texture;
		public int width;
		public int height;
		
		public GlyphMetrics metrics;

		public Glyph(ShaderResourceView texture, int width, int height, GlyphMetrics metrics)
		{
			this.texture = texture;
			this.width = width;
			this.height = height;
			this.metrics = metrics;
		}
	}
}
