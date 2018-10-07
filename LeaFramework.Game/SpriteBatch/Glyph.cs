using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using SharpFont;

namespace LeaFramework.Game.SpriteBatch
{
	public struct Glyph
	{
		public int width;
		public int height;
		
		public GlyphMetrics metrics;
		public Vector2 offset;

		public Glyph( int width, int height, GlyphMetrics metrics, Vector2 offset)
		{
			this.width = width;
			this.height = height;
			this.metrics = metrics;
			this.offset = offset;
		}
	}
}
