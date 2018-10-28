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
		public GlyphMetrics metrics;
		public Vector2 offset;

		public Glyph(GlyphMetrics metrics, Vector2 offset)
		{
			this.metrics = metrics;
			this.offset = offset;
			
		}
	}
}
